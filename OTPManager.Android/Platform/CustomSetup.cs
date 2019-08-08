using Android.Views;
using MvvmCross;
using MvvmCross.Platforms.Android.Core;
using MvvmCross.ViewModels;
using ZXing.Mobile;

namespace OTPManager.Android
{
    public class CustomSetup<TApplication> : MvxAndroidSetup<TApplication> where TApplication : class, IMvxApplication, new()
    {
        protected override void InitializeLastChance()
        {
            Mvx.IoCProvider.RegisterSingleton<IMobileBarcodeScanner>(() => new MobileBarcodeScanner
            {
                CustomOverlay = new View(ApplicationContext),
                UseCustomOverlay = true
            });
        }
    }
}
