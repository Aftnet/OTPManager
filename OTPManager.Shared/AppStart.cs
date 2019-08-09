using Acr.UserDialogs;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using OTPManager.Shared.Models;
using OTPManager.Shared.ViewModels;
using System;
using System.Threading.Tasks;

namespace OTPManager.Shared
{
    public class AppStart : MvxAppStart
    {
        private IUserDialogs DialogService { get; }

        public AppStart(IMvxApplication application, IMvxNavigationService navigationService, IUserDialogs dialogService) : base(application, navigationService)
        {
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }

        protected override Task NavigateToFirstViewModel(object hint = null)
        {
            if (hint is string hintString)
            {
                var generator = OTPGenerator.FromString(hintString);
                if (generator != null)
                {
                    return NavigationService.Navigate<AddGeneratorViewModel, AddGeneratorViewModel.Parameter>(new AddGeneratorViewModel.Parameter(generator, true));
                }
                else
                {
                    DialogService.AlertAsync(Resources.Strings.InvalidUriMessage, Resources.Strings.InvalidUriTitle);
                }
            }

            return NavigationService.Navigate<CodesDisplayViewModel>();
        }
    }
}
