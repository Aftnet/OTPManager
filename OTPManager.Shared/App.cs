using Acr.UserDialogs;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OTPManager.Shared.Services;
using OTPManager.Shared.ViewModels;
using Plugin.FileSystem;
using Plugin.SecureStorage;
using Plugin.Share;

namespace OTPManager.Shared
{
    public class App : MvxApplication
    {
        public App()
        {
            Mvx.RegisterSingleton(CrossSecureStorage.Current);
            Mvx.RegisterSingleton(CrossFileSystem.Current);
            Mvx.RegisterSingleton(CrossShare.Current);
            Mvx.RegisterSingleton(UserDialogs.Instance);
            Mvx.LazyConstructAndRegisterSingleton<IStorageService, StorageService>();
            Mvx.LazyConstructAndRegisterSingleton<IMvxNavigationService, MvxNavigationService>();
            Mvx.LazyConstructAndRegisterSingleton<IUriService, UriService>();
            Mvx.LazyConstructAndRegisterSingleton<IMvxAppStart, MvxNavigationServiceAppStart<CodesDisplayViewModel>>();
        }
    }
}
