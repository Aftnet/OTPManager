using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using OTPManager.Shared.Models;
using OTPManager.Shared.Services;
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

        private readonly IMvxNavigationService Navigator;
        private readonly IPlatformService PlatformService;
        private readonly IStorageService DataStore;
        private readonly IMobileBarcodeScanner Scanner;
        private readonly IUriService UriService;

        internal const ulong DefaultUpdateTimeCode = 0;
        internal ulong LastUpdateTimeCode = DefaultUpdateTimeCode;

        private const int progressScale = 10000;
        public int ProgressScale { get { return progressScale; } }

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
                    LastUpdateTimeCode = DefaultUpdateTimeCode;
                }
            }
        }

        public bool GeneratorsAvailable { get { return Items.Any(); } }

        public MvxCommand<OTPDisplayViewModel> ItemClicked { get; private set; }
        public MvxCommand CreateEntryManual { get; private set; }
        public MvxCommand CreateEntryQR { get; private set; }

        private Timer BackgroundRefreshTimer;

        public CodesDisplayViewModel(IMvxNavigationService navigator, IPlatformService platformService, IStorageService dataStore, IMobileBarcodeScanner scanner, IUriService uriService)
        {
            Navigator = navigator;
            PlatformService = platformService;
            DataStore = dataStore;
            Scanner = scanner;
            UriService = uriService;

            ItemClicked = new MvxCommand<OTPDisplayViewModel>(d =>
            {
                Navigator.Navigate<DisplayGeneratorViewModel, OTPGenerator>(d.Generator);
            });

            CreateEntryManual = new MvxCommand(() =>
            {
                Navigator.Navigate<AddGeneratorViewModel>();
            });

            CreateEntryQR = new MvxCommand(CreateEntryQRHandler);
        }

        public override void ViewAppearing()
        {
            var task = ViewAppearingAsync();
        }

        internal async Task ViewAppearingAsync()
        {
            var generators = await DataStore.GetAllAsync();
            Items = generators.Select(d => new OTPDisplayViewModel(Navigator, PlatformService, d)).ToList();

            UIRefresh();
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

        private async void CreateEntryQRHandler()
        {
            var result = await Scanner.Scan();
            if (result == null)
                return;

            await UriService.CreateGeneratorFromUri(result.Text);
        }

        private void UIRefresh()
        {
            var currentTime = DateTimeOffset.UtcNow;
            var timeCode = OTPGenerator.GenerateTimeCode(currentTime);

            Progress = ComputeProgress(currentTime);
            if (timeCode != LastUpdateTimeCode)
            {
                foreach (var i in Items)
                {
                    i.UpdateOTP(currentTime);
                }

                LastUpdateTimeCode = timeCode;
            }
        }

        private static int ComputeProgress(DateTimeOffset input)
        {
            var timeStepSeonds = OTPGenerator.TimeStep.Seconds;
            var nowModulo = input.Second % timeStepSeonds;
            var progress = progressScale * (nowModulo * 1000 + input.Millisecond);
            progress = progress / (timeStepSeonds * 1000);
            return progress;
        }
    }
}
