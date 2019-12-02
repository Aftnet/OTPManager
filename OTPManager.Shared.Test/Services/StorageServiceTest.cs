using Moq;
using OTPManager.Shared.Models;
using OTPManager.Shared.Services;
using Plugin.FileSystem;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OTPManager.Shared.Test.Services
{
    public class StorageServiceTest : TestBase<StorageService>, IDisposable
    {
        private static readonly DirectoryInfo LocalFolder = new DirectoryInfo(new System.IO.DirectoryInfo("."));

        protected override StorageService GetTarget()
        {
            FileSystemMock.Setup(d => d.LocalStorage).Returns(LocalFolder);
            return new StorageService(SecureStorageMock.Object, FileSystemMock.Object);
        }

        private static readonly OTPGenerator[] TestData;

        static StorageServiceTest()
        {
            TestData = Enumerable.Range(1, 4).Select(d => CreateOTPGenerator(d)).ToArray();

            var random = new Random();
            foreach (var i in TestData)
            {
                random.NextBytes(i.Secret);
            }
        }

        public void Dispose()
        {
            Target.ClearAsync().Wait();
        }

        [Fact]
        public async Task EncryptionPasswordIsGeneratedIfNoneIsFound()
        {
            SecureStorageMock.Setup(d => d.GetValue(StorageService.AppKeychainId, null)).Returns(null as string);
            var contents = await Target.InsertOrReplaceAsync(CreateOTPGenerator(1));
            SecureStorageMock.Verify(d => d.GetValue(StorageService.AppKeychainId, null));
            SecureStorageMock.Verify(d => d.SetValue(StorageService.AppKeychainId, It.Is<string>(e => !string.IsNullOrEmpty(e))));
        }

        [Fact]
        public async Task DataOperationWork()
        {
            string keychainKey = null;
            SecureStorageMock.Setup(d => d.SetValue(StorageService.AppKeychainId, It.IsAny<string>())).Callback((string d, string e) => keychainKey = e);
            SecureStorageMock.Setup(d => d.GetValue(StorageService.AppKeychainId, null)).Returns(keychainKey);

            await Target.ClearAsync();
            var contents = await Target.GetAllAsync();
            Assert.Empty(contents);

            foreach (var i in TestData)
            {
                await Target.InsertOrReplaceAsync(i);
            }

            contents = await Target.GetAllAsync();
            Assert.Equal(TestData.Length, contents.Count);
            foreach (var i in contents)
            {
                var match = TestData.First(d => d.Uid == i.Uid);
                Assert.Equal(match.AlgorithmName, i.AlgorithmName);
                Assert.Equal(match.AllowExporting, i.AllowExporting);
                Assert.Equal(match.DbEncryptedSecret, i.DbEncryptedSecret);
                Assert.Equal(match.DbEncryptedSecretIV, i.DbEncryptedSecretIV);
                Assert.Equal(match.Issuer, i.Issuer);
                Assert.Equal(match.NumDigits, i.NumDigits);
                Assert.Equal(match.Secret, i.Secret);
                Assert.Equal(match.SecretBase32, i.SecretBase32);

                await Target.DeleteAsync(i);
            }

            contents = await Target.GetAllAsync();
            Assert.Empty(contents);
        }

        [Theory]
        [InlineData(new object[] { true })]
        [InlineData(new object[] { false })]
        public async Task DumpRestoreWorks(bool shouldSucceed)
        {
            await Target.ClearAsync();
            foreach (var i in TestData)
            {
                await Target.InsertOrReplaceAsync(i);
            }

            var password = "encryptionPassword";
            var dumpData = await Target.DumpAsync(password);

            var result = await Target.RestoreAsync(dumpData, shouldSucceed ? password : "wrongPassword");
            Assert.Equal(result, shouldSucceed);
        }
    }
}
