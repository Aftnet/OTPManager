using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Views;
using OTPManager.Shared.ViewModels;
using System;
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
            set.Bind(SecretTextField).To(m => m).WithConversion("SecretHidden");
            set.Bind(IssuerTextField).To(m => m.Issuer);
            set.Bind(QRDisplay).For(m => m.Image).To(m => m.QRData).WithConversion("BitMatrixToQR");
            set.Bind(deleteButton).To(m => m.DeleteGenerator);
            set.Apply();
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            switch (section)
            {
                case 0:
                    return NSBundle.MainBundle.GetLocalizedString("AccountDetails", string.Empty);
                case 1:
                    return null;
            }

            return base.TitleForHeader(tableView, section);
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            switch (section)
            {
                case 1:
                    return ViewModel.AllowExporting ? 1 : 0;
            }

            return base.RowsInSection(tableView, section);
        }
    }
}