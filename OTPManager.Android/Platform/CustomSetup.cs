using Android.Views;
using Microsoft.Extensions.Logging;
using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.Platforms.Android.Core;
using MvvmCross.ViewModels;
using ZXing.Mobile;

namespace OTPManager.Android.Platform
{
    public class CustomSetup<TApplication> : MvxAndroidSetup<TApplication> where TApplication : class, IMvxApplication, new()
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
                CustomOverlay = new View(ApplicationContext),
                UseCustomOverlay = true
            });
        }
    }
}
