using MvvmCross.Platforms.Uap.Views;
using OTPManager.UWP.Platform;

namespace OTPManager.UWP
{
    sealed partial class App
    {
        public App()
        {
            InitializeComponent();
        }
    }

    public abstract class OTPManagerApp : MvxApplication<CustomSetup<Shared.App>, Shared.App>
    {
        override Sta
    }
}
