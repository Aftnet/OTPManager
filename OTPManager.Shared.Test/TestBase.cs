using Acr.UserDialogs;
using Moq;
using MvvmCross.Core.Navigation;
using OTPManager.Shared.Models;
using OTPManager.Shared.Services;
using Plugin.FileSystem.Abstractions;
using Plugin.SecureStorage.Abstractions;
using System;
using ZXing.Mobile;

namespace OTPManager.Shared.Test
{
    public abstract class TestBase<T>
    {
        protected abstract T GetTarget();
        protected T Target { get; private set; }

        protected readonly Mock<IMvxNavigationService> NavigatorMock = new Mock<IMvxNavigationService>();
        protected readonly Mock<IPlatformService> PlatformServiceMock = new Mock<IPlatformService>();
        protected readonly Mock<IStorageService> DataStoreMock = new Mock<IStorageService>();
        protected readonly Mock<IFileSystem> FileSystemMock = new Mock<IFileSystem>();
        protected readonly Mock<ISecureStorage> SecureStorageMock = new Mock<ISecureStorage>();
        protected readonly Mock<IMobileBarcodeScanner> BarcodeScannerMock = new Mock<IMobileBarcodeScanner>();
        protected readonly Mock<IUserDialogs> DialogServiceMock = new Mock<IUserDialogs>();
        protected readonly Mock<IUriService> UriServiceMock = new Mock<IUriService>();

        private static Random RandomGenerator = new Random();

        public TestBase()
        {
            Target = GetTarget();
        }

        protected static OTPGenerator CreateOTPGenerator(int seed)
        {
            var output = new OTPGenerator
            {
                Label = $"Label {seed}",
                Issuer = $"Issuer {seed}",
                Secret = new byte[16]
            };

            RandomGenerator.NextBytes(output.Secret);
            return output;
        }
    }
}
