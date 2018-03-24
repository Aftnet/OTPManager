using Acr.UserDialogs;
using MvvmCross.Core.Navigation;
using OTPManager.Shared.Models;
using OTPManager.Shared.ViewModels;
using System;
using System.Threading.Tasks;

namespace OTPManager.Shared.Services
{
    public class UriService : IUriService
    {
        private readonly IMvxNavigationService Navigator;
        private readonly IUserDialogs DialogService;

        public UriService(IMvxNavigationService navigator, IUserDialogs dialogService)
        {
            Navigator = navigator;
            DialogService = dialogService;
        }

        public Task CreateGeneratorFromUri(string uri)
        {
            if (!Uri.TryCreate(uri, UriKind.Absolute, out var parsedUri))
            {
                return GenerateErrorUITask();
            }

            return CreateGeneratorFromUri(parsedUri);
        }

        public Task CreateGeneratorFromUri(Uri uri)
        {
            if (uri == null)
            {
                return GenerateErrorUITask();
            }

            var generator = OTPGenerator.FromUri(uri);
            if (generator == null)
            {
                return GenerateErrorUITask();
            }

            return Navigator.Navigate<AddGeneratorViewModel, OTPGenerator>(generator);
        }

        private Task GenerateErrorUITask()
        {
            return DialogService.AlertAsync(Resources.Strings.InvalidUriMessage, Resources.Strings.InvalidUriTitle);
        }
    }
}
