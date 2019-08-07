using MvvmCross.Platforms.Uap.Views;
using OTPManager.Shared.ViewModels;

namespace OTPManager.UWP.Views
{
    public sealed partial class AddGeneratorView : MvxWindowsPage
    {
        public AddGeneratorViewModel VM => ViewModel as AddGeneratorViewModel;

        public AddGeneratorView()
        {
            this.InitializeComponent();
        }
    }
}
