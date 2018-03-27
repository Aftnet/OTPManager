using System;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using OTPManager.Shared.ViewModels;
using UIKit;

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

            this.CreateBinding(IssuerTextField).To<AddGeneratorViewModel>(d => d.Issuer).Apply();
            this.CreateBinding(SecretTextField).To<AddGeneratorViewModel>(d => d.SecretBase32).Apply();
            this.CreateBinding(LabelTextField).To<AddGeneratorViewModel>(d => d.Label).Apply();

            this.CreateBinding(saveButton).To<AddGeneratorViewModel>(d => d.AddGenerator).Apply();
            this.CreateBinding(cancelButton).To<AddGeneratorViewModel>(d => d.Cancel).Apply();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}