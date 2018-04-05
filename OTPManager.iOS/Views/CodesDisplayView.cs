using System;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using MvvmCross.iOS.Views;
using OTPManager.Shared.ViewModels;
using UIKit;

namespace OTPManager.iOS
{
    [MvxFromStoryboard("CodesDisplay")]
    public partial class CodesDisplayView : MvxViewController<CodesDisplayViewModel>
    {
        public class TableViewSource : MvxTableViewSource
        {
            private NSString _cellIdentifier = new NSString(nameof(CodesDisplayItemView));

            public TableViewSource(UITableView tableView) : base(tableView)
            {
            }

            protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
            {
                return (CodesDisplayItemView)tableView.DequeueReusableCell(_cellIdentifier);
            }
        }

        public CodesDisplayView(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            NavigationController.NavigationBar.TintColor = View.TintColor;

            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            var addManualButton = new UIBarButtonItem(UIBarButtonSystemItem.Add);
            var addQRImage = new UIBarButtonItem(UIBarButtonSystemItem.Camera);
            NavigationItem.RightBarButtonItems = new[] { addManualButton, addQRImage };

            var source = new TableViewSource(TableView);

            var set = this.CreateBindingSet<CodesDisplayView, CodesDisplayViewModel>();
            set.Bind(ProgressBar).To(m => m.Progress)
               .WithConversion("CodesDisplayProgress", ViewModel.ProgressScale);
            set.Bind(ProgressBar).For("Visible").To(m => m.GeneratorsAvailable);
            set.Bind(TableView).For("Visible").To(m => m.GeneratorsAvailable);
            set.Bind(source).To(m => m.Items);
            set.Bind(source).For(m => m.SelectionChangedCommand).To(m => m.ItemClicked);
            set.Bind(NoGeneratorsStackView).For("Visible").To(m => m.GeneratorsAvailable).WithConversion("BoolInversion");
            set.Bind(NoGeneratorsAddManually).To(m => m.CreateEntryManual);
            set.Bind(addManualButton).To(m => m.CreateEntryManual);
            set.Bind(NoGeneratorsAddQR).To(m => m.CreateEntryQR);
            set.Bind(addQRImage).To(m => m.CreateEntryQR);
            set.Apply();

            TableView.Source = source;
            TableView.ReloadData();
        }
    }
}