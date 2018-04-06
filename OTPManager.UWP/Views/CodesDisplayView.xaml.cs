using MvvmCross.Uwp.Views;
using OTPManager.Shared.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OTPManager.UWP.Views
{
    public sealed partial class CodesDisplayPage : MvxWindowsPage
    {
        public CodesDisplayViewModel VM => ViewModel as CodesDisplayViewModel;

        public CodesDisplayPage()
        {
            this.InitializeComponent();
        }

        private void ItemClicked(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            VM.ItemClicked.Execute((OTPDisplayViewModel)e.ClickedItem);
        }
    }
}
