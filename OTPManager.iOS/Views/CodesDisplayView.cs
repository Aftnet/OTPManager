using System;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using OTPManager.Shared.ViewModels;
using UIKit;
using OTPManager.iOS.Views;

namespace OTPManager.iOS
{
    [MvxFromStoryboard("CodesDisplay")]
    [MvxRootPresentation(WrapInNavigationController = true)]
    public partial class CodesDisplayView : MvxViewController<CodesDisplayViewModel>
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

            var source = new MvxSimpleTableViewSource(TableView, typeof(CodesDisplayItemView));

            var set = this.CreateBindingSet<CodesDisplayView, CodesDisplayViewModel>();
            set.Bind(ProgressBar).To(m => m.Progress)
               .WithConversion("CodesDisplayProgress", ViewModel.ProgressScale);
            set.Bind(source).To(m => m.Items);
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