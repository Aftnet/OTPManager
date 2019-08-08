using MvvmCross;
using MvvmCross.Platforms.Ios.Core;
using MvvmCross.ViewModels;
using ZXing.Mobile;

namespace OTPManager.iOS.Platform
{
    public class CustomSetup<TApplication> : MvxIosSetup<TApplication> where TApplication : class, IMvxApplication, new()
    {
        protected override void InitializeLastChance()
        {
            Mvx.IoCProvider.RegisterSingleton<IMobileBarcodeScanner>(() => new MobileBarcodeScanner
            {
                //CustomOverlay = new UIView(),
                UseCustomOverlay = false
            });
        }
    }
}
