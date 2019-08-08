using Foundation;
using MvvmCross.Platforms.Ios.Core;
using OTPManager.iOS.Platform;
using UIKit;

namespace OTPManager.iOS
{
    [Register(nameof(AppDelegate))]
    public class AppDelegate : MvxApplicationDelegate<CustomSetup<Shared.App>, Shared.App>
    {
        public override UIWindow Window { get; set; }

        // FinishedLaunching is the very first code to be executed in your app. Don't forget to call base!
        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            var result = base.FinishedLaunching(application, launchOptions);

            return result;
        }
    }
}