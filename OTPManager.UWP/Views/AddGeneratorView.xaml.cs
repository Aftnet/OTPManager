using MvvmCross.Uwp.Views;
using OTPManager.Shared.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OTPManager.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddGeneratorPage : MvxWindowsPage
    {
        public AddGeneratorViewModel VM => ViewModel as AddGeneratorViewModel;

        public AddGeneratorPage()
        {
            this.InitializeComponent();
        }
    }
}
