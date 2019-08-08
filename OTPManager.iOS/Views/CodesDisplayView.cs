using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.Plugin.Visibility;
using ObjCRuntime;
using OTPManager.Shared.ViewModels;
using System;
using UIKit;

namespace OTPManager.iOS
{
    [MvxFromStoryboard("CodesDisplay")]
    public partial class CodesDisplayView : MvxViewController<CodesDisplayViewModel>
    {
        public class TableViewSource : MvxTableViewSource
        {
            private readonly NSString CellIdentifier = new NSString(nameof(CodesDisplayItemView));
            private readonly CodesDisplayViewModel ViewModel;

            public TableViewSource(UITableView tableView, CodesDisplayViewModel viewModel) : base(tableView)
            {
                ViewModel = viewModel;
            }

            protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
            {
                return (CodesDisplayItemView)tableView.DequeueReusableCell(CellIdentifier);
            }

            public override bool ShouldShowMenu(UITableView tableView, NSIndexPath rowAtindexPath)
            {
                return true;
            }

            public override bool CanPerformAction(UITableView tableView, Selector action, NSIndexPath indexPath, NSObject sender)
            {
                return action.Name == "copy:";
            }

            public override void PerformAction(UITableView tableView, Selector action, NSIndexPath indexPath, NSObject sender)
            {
                var target = ViewModel.Items[indexPath.Row];
                target.CopyToClipboard.Execute(null);
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

            var source = new TableViewSource(TableView, ViewModel);

            var set = this.CreateBindingSet<CodesDisplayView, CodesDisplayViewModel>();
            set.Bind(ProgressBar).To(m => m.Progress)
               .WithConversion("CodesDisplayProgress", ViewModel.ProgressScale);
            set.Bind(ProgressBar).For("Visible").To(m => m.GeneratorsAvailable).WithConversion<MvxInvertedVisibilityValueConverter>();
            set.Bind(TableView).For("Visible").To(m => m.GeneratorsAvailable).WithConversion<MvxInvertedVisibilityValueConverter>();
            set.Bind(source).To(m => m.Items);
            set.Bind(source).For(m => m.SelectionChangedCommand).To(m => m.ItemClicked);
            set.Bind(NoGeneratorsStackView).For("Visible").To(m => m.GeneratorsAvailable).WithConversion<MvxVisibilityValueConverter>();
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