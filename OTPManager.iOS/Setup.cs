using MvvmCross.Core.ViewModels;
using MvvmCross.iOS.Platform;
using MvvmCross.iOS.Views.Presenters;
using MvvmCross.Platform;
using MvvmCross.Platform.Logging;
using OTPManager.iOS.Services;
using OTPManager.Shared.Services;
using UIKit;
using ZXing.Mobile;

namespace OTPManager.iOS
{
    public class Setup : MvxIosSetup
    {
        public Setup(IMvxApplicationDelegate applicationDelegate, IMvxIosViewPresenter presenter) : base(applicationDelegate, presenter)
        {
        }

        protected override IMvxApplication CreateApp()
        {
            return new Shared.App();
        }

        protected override void InitializeFirstChance()
        {
            Mvx.LazyConstructAndRegisterSingleton<IPlatformService, PlatformService>();
            Mvx.RegisterSingleton<IMobileBarcodeScanner>(() => new MobileBarcodeScanner
            {
                //CustomOverlay = new UIView(),
                UseCustomOverlay = false
            });
        }

        protected override MvxLogProviderType GetDefaultLogProviderType()
        {
            return MvxLogProviderType.None;
        }
    }
}
