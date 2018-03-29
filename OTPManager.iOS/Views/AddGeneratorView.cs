using System;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using OTPManager.Shared.ViewModels;
using UIKit;
using Foundation;

namespace OTPManager.iOS
{
    [MvxFromStoryboard("AddGenerator")]
    public partial class AddGeneratorView : MvxTableViewController<AddGeneratorViewModel>
    {
        public AddGeneratorView (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            var saveButton = new UIBarButtonItem(UIBarButtonSystemItem.Save);
            var cancelButton = new UIBarButtonItem(UIBarButtonSystemItem.Cancel);
            NavigationItem.RightBarButtonItem = saveButton;
            NavigationItem.BackBarButtonItem = cancelButton;

            var set = this.CreateBindingSet<AddGeneratorView, AddGeneratorViewModel>();
            set.Bind(LabelTextField).To(m => m.Label);
            set.Bind(SecretTextField).To(m => m.SecretBase32);
            set.Bind(IssuerTextField).To(m => m.Issuer);
            set.Bind(saveButton).To(m => m.AddGenerator);
            set.Bind(cancelButton).To(m => m.Cancel);
            set.Apply();
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            switch (section)
            {
                case 0:
                    return NSBundle.MainBundle.LocalizedString("AddAccount", string.Empty);
                default:
                    return string.Empty;
            }
        }
    }
}