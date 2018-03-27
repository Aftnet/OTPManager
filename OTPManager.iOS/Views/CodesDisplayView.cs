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

            var set = this.CreateBindingSet<CodesDisplayView, CodesDisplayViewModel>();
            set.Bind(ProgressBar).To(m => m.Progress)
               .WithConversion("CodesDisplayProgress", ViewModel.ProgressScale);
            set.Bind(addManualButton).To(m => m.CreateEntryManual);
            set.Bind(addQRImage).To(m => m.CreateEntryQR);
            set.Apply();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}