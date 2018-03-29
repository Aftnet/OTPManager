using System;
using Foundation;
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
            set.Bind(SecretTextField).To(m => m).WithConversion("SecretHidden");
            set.Bind(IssuerTextField).To(m => m.Issuer);
            set.Bind(QRDisplay).For(m => m.Image).To(m => m.QRData).WithConversion("BitMatrixToQR");
            set.Bind(deleteButton).To(m => m.DeleteGenerator);
            set.Apply();
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            switch(section)
            {
                case 0:
                    return NSBundle.MainBundle.LocalizedString("AccountDetails", string.Empty);
                default:
                    return string.Empty;
            }
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return UITableView.AutomaticDimension;
        }
    }
}