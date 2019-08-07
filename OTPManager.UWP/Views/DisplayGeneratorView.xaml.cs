using MvvmCross.Platforms.Uap.Views;
using OTPManager.Shared.ViewModels;

namespace OTPManager.UWP.Views
{
    public sealed partial class DisplayGeneratorView : MvxWindowsPage
    {
        public DisplayGeneratorViewModel VM => ViewModel as DisplayGeneratorViewModel;

        public DisplayGeneratorView()
        {
            this.InitializeComponent();
        }
    }
}
