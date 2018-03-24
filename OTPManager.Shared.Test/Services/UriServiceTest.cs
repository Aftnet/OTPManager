using Moq;
using OTPManager.Shared.Models;
using OTPManager.Shared.Services;
using OTPManager.Shared.ViewModels;
using System;
using System.Threading.Tasks;
using Xunit;

namespace OTPManager.Shared.Test.Services
{
    public class UriServiceTest : TestBase<UriService>
    {
        private const string ValidOTPUri = "otpauth://totp/Test:Alice Loller%40test.com?secret=ABABABABABABABAB&algorithm=SHA256&digits=6&issuer=Test";

        protected override UriService GetTarget()
        {
            return new UriService(NavigatorMock.Object, DialogServiceMock.Object);
        }

        public UriServiceTest() : base()
        {
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("notAnUri", false)]
        [InlineData("http://www.google.com", false)]
        [InlineData(ValidOTPUri, true)]
        public async Task CreateGeneratorFromUriWorks(string uriString, bool shouldWork)
        {
            await Target.CreateGeneratorFromUri(uriString);

            if (shouldWork)
            {
                NavigatorMock.Verify(d => d.Navigate<AddGeneratorViewModel, OTPGenerator>(It.IsAny<OTPGenerator>(), null));
                DialogServiceMock.Verify(d => d.AlertAsync(Resources.Strings.InvalidUriMessage, Resources.Strings.InvalidUriTitle, null, null), Times.Never);
            }
            else
            {
                NavigatorMock.Verify(d => d.Navigate<AddGeneratorViewModel, OTPGenerator>(It.IsAny<OTPGenerator>(), null), Times.Never);
                DialogServiceMock.Verify(d => d.AlertAsync(Resources.Strings.InvalidUriMessage, Resources.Strings.InvalidUriTitle, null, null));
            }
        }
    }
}
