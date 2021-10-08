using Microsoft.Extensions.Logging;
using MvvmCross.IoC;
using MvvmCross.Platforms.Ios.Core;
using MvvmCross.ViewModels;
using ZXing.Mobile;

namespace OTPManager.iOS.Platform
{
    public class CustomSetup<TApplication> : MvxIosSetup<TApplication> where TApplication : class, IMvxApplication, new()
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

            iocProvider.RegisterSingleton<IMobileBarcodeScanner>(() => new MobileBarcodeScanner
            {
                //CustomOverlay = new UIView(),
                UseCustomOverlay = false
            });
        }
    }
}
