using Acr.UserDialogs;
using Moq;
using MvvmCross.Navigation;
using OTPManager.Shared.Models;
using OTPManager.Shared.Services;
using Plugin.FileSystem.Abstractions;
using Plugin.SecureStorage.Abstractions;
using Plugin.Share.Abstractions;
using System;
using ZXing.Mobile;

namespace OTPManager.Shared.Test
{
    public abstract class TestBase<T>
    {
        protected abstract T GetTarget();

        private readonly Lazy<T> target;
        protected T Target => target.Value;

        protected readonly Mock<IMvxNavigationService> NavigatorMock = new Mock<IMvxNavigationService>();
        protected readonly Mock<IShare> PlatformServiceMock = new Mock<IShare>();
        protected readonly Mock<IStorageService> DataStoreMock = new Mock<IStorageService>();
        protected readonly Mock<IFileSystem> FileSystemMock = new Mock<IFileSystem>();
        protected readonly Mock<ISecureStorage> SecureStorageMock = new Mock<ISecureStorage>();
        protected readonly Mock<IMobileBarcodeScanner> BarcodeScannerMock = new Mock<IMobileBarcodeScanner>();
        protected readonly Mock<IUserDialogs> DialogServiceMock = new Mock<IUserDialogs>();

        private static Random RandomGenerator { get; } = new Random();

        public TestBase()
        {
            target = new Lazy<T>(GetTarget, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
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
