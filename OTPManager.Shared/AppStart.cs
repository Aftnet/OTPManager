using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using OTPManager.Shared.ViewModels;
using System;
using System.Threading.Tasks;

namespace OTPManager.Shared
{
    public class AppStart : MvxAppStart
    {
        public AppStart(IMvxApplication application, IMvxNavigationService navigationService) : base(application, navigationService)
        {
        }

        protected override Task NavigateToFirstViewModel(object hint = null)
        {
            return NavigationService.Navigate<CodesDisplayViewModel>();
        }
    }
}
