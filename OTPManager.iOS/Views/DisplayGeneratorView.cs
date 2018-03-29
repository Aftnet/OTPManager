using System;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using OTPManager.Shared.ViewModels;
using UIKit;

namespace OTPManager.iOS
{
    [MvxFromStoryboard("DisplayGenerator")]
    public partial class DisplayGeneratorView : MvxTableViewController<DisplayGeneratorViewModel>
    {
        public DisplayGeneratorView (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            var deleteButton = new UIBarButtonItem(UIBarButtonSystemItem.Trash);
            NavigationItem.RightBarButtonItem = deleteButton;

            var set = this.CreateBindingSet<DisplayGeneratorView, DisplayGeneratorViewModel>();
            set.Bind(LabelTextField).To(m => m.Label);
            set.Bind(SecretTextField).To(m => m.SecretBase32);
            set.Bind(IssuerTextField).To(m => m.Issuer);
            set.Bind(deleteButton).To(m => m.DeleteGenerator);
            set.Apply();
        }
    }
}