using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.Logging;
using MvvmCross.Uwp.Platform;
using OTPManager.Shared.Services;
using OTPManager.UWP.Services;
using Windows.UI.Xaml.Controls;
using ZXing.Mobile;

namespace OTPManager.UWP
{
    public class Setup : MvxWindowsSetup
    {
        public Setup(Frame rootFrame) : base(rootFrame)
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
                CustomOverlay = new Grid(),
                UseCustomOverlay = true
            });
        }

        protected override MvxLogProviderType GetDefaultLogProviderType()
        {
            return MvxLogProviderType.None;
        }
    }
}
