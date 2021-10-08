using Acr.UserDialogs;
using MvvmCross;
using MvvmCross.ViewModels;
using OTPManager.Shared.Services;
using Plugin.FileSystem;
using Plugin.SecureStorage;

namespace OTPManager.Shared
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            var ioc = Mvx.IoCProvider;
            ioc.RegisterSingleton(CrossSecureStorage.Current);
            ioc.RegisterSingleton(CrossFileSystem.Current);
            ioc.RegisterSingleton(UserDialogs.Instance);
            ioc.RegisterSingleton<IStorageService>(ioc.IoCConstruct<StorageService>());
            RegisterCustomAppStart<AppStart>();
        }
    }
}
