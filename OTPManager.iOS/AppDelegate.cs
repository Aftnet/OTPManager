using Foundation;
using MvvmCross.Platforms.Ios.Core;
using OTPManager.iOS.Platform;
using UIKit;

namespace OTPManager.iOS
{
    [Register(nameof(AppDelegate))]
    public class AppDelegate : MvxApplicationDelegate<CustomSetup<Shared.App>, Shared.App>
    {
        private string LaunchUri { get; set; }

        // FinishedLaunching is the very first code to be executed in your app. Don't forget to call base!
        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            var result = base.FinishedLaunching(application, launchOptions);
            LaunchUri = launchOptions[UIApplication.LaunchOptionsUrlKey].ToString();
            return result;
        }

        public override void OnActivated(UIApplication application)
        {
            base.OnActivated(application);
        }

        protected override object GetAppStartHint(object hint = null) => LaunchUri;
    }
}