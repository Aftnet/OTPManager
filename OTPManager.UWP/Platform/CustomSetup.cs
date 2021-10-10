using Microsoft.Extensions.Logging;
using MvvmCross.IoC;
using MvvmCross.Platforms.Uap.Core;
using MvvmCross.UI;
using MvvmCross.ViewModels;
using Windows.UI.Xaml.Controls;
using ZXing.Mobile;

namespace OTPManager.UWP.Platform
{
    public class CustomSetup<TApplication> : MvxWindowsSetup<TApplication> where TApplication : class, IMvxApplication, new()
    {
        protected override ILoggerProvider CreateLogProvider()
        {
            return default(ILoggerProvider);
        }

        protected override ILoggerFactory CreateLogFactory()
        {
            return default(ILoggerFactory);
        }

        protected override void InitializeLastChance(IMvxIoCProvider iocProvider)
        {
            base.InitializeLastChance(iocProvider);

            iocProvider.RegisterType<IMvxNativeVisibility, MvvmCross.Plugin.Visibility.Platforms.Uap.MvxWinRTVisibility>();
            iocProvider.RegisterSingleton<IMobileBarcodeScanner>(() => new MobileBarcodeScanner
            {
                CustomOverlay = new Grid(),
                UseCustomOverlay = true
            });
        }
    }
}
