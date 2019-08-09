using MvvmCross.Platforms.Uap.Views;
using OTPManager.UWP.Platform;
using Windows.ApplicationModel.Activation;

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
        private string LaunchUri { get; set; }

        protected override void OnActivated(IActivatedEventArgs activationArgs)
        {
            if (activationArgs is ProtocolActivatedEventArgs protocolArgs)
            {
                LaunchUri = protocolArgs.Uri.ToString();
            }

            base.OnActivated(activationArgs);
        }

        protected override object GetAppStartHint(object hint = null) => LaunchUri;
    }
}
