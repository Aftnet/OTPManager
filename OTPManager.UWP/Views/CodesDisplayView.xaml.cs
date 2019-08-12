using MvvmCross.Platforms.Uap.Views;
using OTPManager.Shared.ViewModels;

namespace OTPManager.UWP.Views
{
    public sealed partial class CodesDisplayView : MvxWindowsPage
    {
        public CodesDisplayViewModel VM => ViewModel as CodesDisplayViewModel;

        public CodesDisplayView()
        {
            this.InitializeComponent();
        }

        private void ItemClicked(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            VM.ItemClicked.Execute((OTPDisplayViewModel)e.ClickedItem);
        }
    }
}
