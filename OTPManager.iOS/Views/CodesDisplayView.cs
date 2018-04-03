using System;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using OTPManager.Shared.ViewModels;
using UIKit;

namespace OTPManager.iOS
{
    [MvxFromStoryboard("CodesDisplay")]
    [MvxRootPresentation(WrapInNavigationController = true)]
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
            this.NavigationController.NavigationBar.TintColor = UIColor.FromName("ThemeColorPrimary");

            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            var addManualButton = new UIBarButtonItem(UIBarButtonSystemItem.Add);
            var addQRImage = new UIBarButtonItem(UIBarButtonSystemItem.Camera);
            NavigationItem.RightBarButtonItems = new[] { addManualButton, addQRImage };

            var source = new TableViewSource(TableView);

            var set = this.CreateBindingSet<CodesDisplayView, CodesDisplayViewModel>();
            set.Bind(ProgressBar).To(m => m.Progress)
               .WithConversion("CodesDisplayProgress", ViewModel.ProgressScale);
            set.Bind(source).To(m => m.Items);
            set.Bind(source).For(m => m.SelectionChangedCommand).To(m => m.ItemClicked);
            set.Bind(addManualButton).To(m => m.CreateEntryManual);
            set.Bind(addQRImage).To(m => m.CreateEntryQR);
            set.Apply();

            TableView.Source = source;
            TableView.ReloadData();
        }
    }
}