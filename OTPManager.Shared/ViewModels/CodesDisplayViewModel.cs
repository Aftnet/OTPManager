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

        private readonly IMvxNavigationService Navigator;
        private readonly IShare ShareService;
        private readonly IStorageService DataStore;
        private readonly IMobileBarcodeScanner Scanner;
        private readonly IUriService UriService;

        private DateTime NextUpdateTime { get; set; } = DateTime.Now;

        public static int ProgressScale { get; } = OTPGenerator.TimeStepSeconds * 1000;

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

        public bool GeneratorsAvailable { get { return Items.Any(); } }

        public IMvxCommand<OTPDisplayViewModel> ItemClicked { get; }
        public IMvxCommand CreateEntryManual { get; }
        public IMvxCommand CreateEntryQR { get; }

        private Timer BackgroundRefreshTimer;

        public CodesDisplayViewModel(IMvxNavigationService navigator, IShare shareService, IStorageService dataStore, IMobileBarcodeScanner scanner, IUriService uriService)
        {
            Navigator = navigator;
            ShareService = shareService;
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

        private async void CreateEntryQRHandler()
        {
            var result = await Scanner.Scan();
            if (result == null)
                return;

            await UriService.CreateGeneratorFromUri(result.Text);
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
