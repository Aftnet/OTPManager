using Acr.UserDialogs;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using OTPManager.Shared.Models;
using OTPManager.Shared.Services;
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
        internal static readonly TimeSpan BackgroudRefreshInterval = TimeSpan.FromMilliseconds(50);

        private IMvxNavigationService Navigator { get; }
        private IUserDialogs DialogService { get; }
        private IShare ShareService { get; }
        private IStorageService DataStore { get; }
        private IMobileBarcodeScanner Scanner { get; }

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

        private Timer BackgroundRefreshTimer;

        public CodesDisplayViewModel(IMvxNavigationService navigator, IUserDialogs dialogService, IShare shareService, IStorageService dataStore, IMobileBarcodeScanner scanner)
        {
            Navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            ShareService = shareService ?? throw new ArgumentNullException(nameof(shareService));
            DataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
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
        }

        public override void ViewAppearing()
        {
            var task = ViewAppearingAsync();
        }

        internal async Task ViewAppearingAsync()
        {
            var generators = await DataStore.GetAllAsync();
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
