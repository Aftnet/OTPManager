using MvvmCross;
using MvvmCross.Platforms.Uap.Core;
using MvvmCross.ViewModels;
using Windows.UI.Xaml.Controls;
using ZXing.Mobile;

namespace OTPManager.UWP.Platform
{
    public class CustomSetup<TApplication> : MvxWindowsSetup<TApplication> where TApplication : class, IMvxApplication, new()
    {
        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();

            Mvx.IoCProvider.RegisterSingleton<IMobileBarcodeScanner>(() => new MobileBarcodeScanner
            {
                CustomOverlay = new Grid(),
                UseCustomOverlay = true
            });
        }
    }
}
