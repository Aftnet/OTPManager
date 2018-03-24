using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using OTPManager.Shared.ViewModels;
using UIKit;

namespace OTPManager.iOS.Views
{
    [MvxRootPresentation(WrapInNavigationController = true)]
    public partial class CodesDisplayView : MvxViewController<CodesDisplayViewModel>
    {
        public CodesDisplayView() : base("CodesDisplayView", null)
        {
            
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            var addManualButton = new UIBarButtonItem(UIBarButtonSystemItem.Add);
            var addQRImage = new UIBarButtonItem(UIBarButtonSystemItem.Camera);
            NavigationItem.RightBarButtonItems = new[] { addManualButton, addQRImage };

            this.CreateBinding(ProgressBar).To<CodesDisplayViewModel>(d => d).WithConversion("CodesDisplayProgress").Apply();
            this.CreateBinding(addManualButton).To<CodesDisplayViewModel>(d => d.CreateEntryManual).Apply();
            this.CreateBinding(addQRImage).To<CodesDisplayViewModel>(d => d.CreateEntryQR).Apply();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

