using System;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using OTPManager.Shared.ViewModels;
using UIKit;

namespace OTPManager.iOS
{
    [MvxFromStoryboard("CodesDisplay")]
    [MvxRootPresentation(WrapInNavigationController = true)]
    public partial class CodesDisplayView : MvxTableViewController<CodesDisplayViewModel>
    {
        public CodesDisplayView(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            var addManualButton = new UIBarButtonItem(UIBarButtonSystemItem.Add);
            var addQRImage = new UIBarButtonItem(UIBarButtonSystemItem.Camera);
            NavigationItem.RightBarButtonItems = new[] { addManualButton, addQRImage };

            this.CreateBinding(ProgressBar).To<CodesDisplayViewModel>(d => d.Progress)
                .WithConversion("CodesDisplayProgress", ViewModel.ProgressScale).Apply();
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