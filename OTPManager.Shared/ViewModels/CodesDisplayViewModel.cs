using Acr.UserDialogs;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using OTPManager.Shared.Models;
using OTPManager.Shared.Resources;
using OTPManager.Shared.Services;
using Plugin.FileSystem.Abstractions;
using Plugin.Share.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZXing.Mobile;

namespace OTPManager.Shared.ViewModels
{
    public class CodesDisplayViewModel : MvxViewModel
    {
        private const string DumpFileExtension = ".otpm";

        internal static readonly TimeSpan BackgroudRefreshInterval = TimeSpan.FromMilliseconds(50);

        private IMvxNavigationService Navigator { get; }
        private IUserDialogs DialogService { get; }
        private IShare ShareService { get; }
        private IStorageService DataStore { get; }
        private IFileSystem FileSystem { get; }
        
        private IMobileBarcodeScanner Scanner { get; }

        internal TaskCompletionSource<bool> DataLoadedTCS { get; set; }

        private DateTime NextUpdateTime { get; set; } = DateTime.Now;

        public int ProgressScale { get; } = OTPGenerator.TimeStepSeconds * 1000;

        private int progress = 0;
        public int Progress
        {
            get => progress;
            set { SetProperty(ref progress, value); }
        }

        private IReadOnlyList<OTPDisplayViewModel> items = new OTPDisplayViewModel[0];
        public IReadOnlyList<OTPDisplayViewModel> Items
        {
            get => items;
            private set
            {
                if (SetProperty(ref items, value))
                {
                    RaisePropertyChanged(nameof(GeneratorsAvailable));
                    NextUpdateTime = DateTime.Now;
                }
            }
        }

        public bool GeneratorsAvailable => Items.Any();

        public IMvxCommand<OTPDisplayViewModel> ItemClicked { get; }
        public IMvxCommand CreateEntryManual { get; }
        public IMvxCommand CreateEntryQR { get; }
        public IMvxCommand Import { get; }
        public IMvxCommand Export { get; }

        private Timer BackgroundRefreshTimer;

        public CodesDisplayViewModel(IMvxNavigationService navigator, IUserDialogs dialogService, IShare shareService, IStorageService dataStore, IFileSystem fileSystem, IMobileBarcodeScanner scanner)
        {
            Navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            ShareService = shareService ?? throw new ArgumentNullException(nameof(shareService));
            DataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
            FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            Scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));

            ItemClicked = new MvxCommand<OTPDisplayViewModel>(d =>
            {
                Navigator.Navigate<DisplayGeneratorViewModel, OTPGenerator>(d.Generator);
            });

            CreateEntryManual = new MvxCommand(() =>
            {
                Navigator.Navigate<AddGeneratorViewModel>();
            });

            CreateEntryQR = new MvxCommand(async () =>
            {
                var result = await Scanner.Scan();
                if (result != null)
                {
                    var generator = OTPGenerator.FromString(result.Text);
                    if (generator != null)
                    {
                        await Navigator.Navigate<AddGeneratorViewModel, OTPGenerator>(generator);
                    }
                    else
                    {
                        await DialogService.AlertAsync(Resources.Strings.InvalidUriMessage, Resources.Strings.InvalidUriTitle);
                    }
                }
            });

            Import = new MvxCommand(async () =>
            {
                var inFile = await FileSystem.PickFileAsync(new string[] { DumpFileExtension });
                if (inFile == null)
                {
                    return;
                }

                var pwDialogResult = await DialogService.PromptAsync(new PromptConfig { InputType = InputType.Password, Message = Strings.EnterPasswordMessage });
                if (!pwDialogResult.Ok)
                {
                    return;
                }

                using (var inStream = await inFile.OpenAsync(System.IO.FileAccess.Read))
                {
                    var restoreSuccess = await DataStore.RestoreAsync(inStream, pwDialogResult.Value);
                    if (restoreSuccess)
                    {
                        var generators = await DataStore.GetAllAsync();
                        Items = generators.Select(d => new OTPDisplayViewModel(ShareService, d)).ToList();
                    }
                    else
                    {
                        await DialogService.AlertAsync(Strings.ImportFailedMessage, Strings.ImportFailedTitle);
                    }
                }
            });

            Export = new MvxCommand(async () =>
            {
                var pwDialogResult = await DialogService.PromptAsync(new PromptConfig { InputType = InputType.Password, Message = Strings.EnterPasswordMessage });
                if (!pwDialogResult.Ok)
                {
                    return;
                }

                var outFile = await FileSystem.PickSaveFileAsync(DumpFileExtension);
                if (outFile == null)
                {
                    return;
                }

                var payload = await DataStore.DumpAsync(pwDialogResult.Value);
                using (var outStream = await outFile.OpenAsync(System.IO.FileAccess.ReadWrite))
                {
                    outStream.Position = 0;
                    await payload.CopyToAsync(outStream);
                    outStream.SetLength(payload.Length);
                }
            });
        }

        public override async void ViewAppearing()
        {
            DataLoadedTCS = new TaskCompletionSource<bool>();
            var generators = await DataStore.GetAllAsync();
            DataLoadedTCS.SetResult(true);

            Items = generators.Select(d => new OTPDisplayViewModel(ShareService, d)).ToList();
            if (BackgroundRefreshTimer == null)
            {
                BackgroundRefreshTimer = new Timer(d => InvokeOnMainThread(UIRefresh), this, BackgroudRefreshInterval, BackgroudRefreshInterval);
            }
        }

        public override void ViewDisappearing()
        {
            BackgroundRefreshTimer?.Dispose();
            BackgroundRefreshTimer = null;
        }

        private void UIRefresh()
        {
            var currentTime = DateTime.Now;
            Progress = (1000 * (currentTime.Second % OTPGenerator.TimeStepSeconds)) + currentTime.Millisecond;

            if (currentTime.CompareTo(NextUpdateTime) >= 0)
            {
                foreach (var i in Items)
                {
                    i.UpdateOTP(currentTime);
                }

                NextUpdateTime = new DateTime(currentTime.AddSeconds(OTPGenerator.TimeStepSeconds).Ticks % (TimeSpan.TicksPerSecond * OTPGenerator.TimeStepSeconds));
            }
        }
    }
}
