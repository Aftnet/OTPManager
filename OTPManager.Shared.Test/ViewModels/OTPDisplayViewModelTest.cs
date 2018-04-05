using OTPManager.Shared.Models;
using OTPManager.Shared.ViewModels;
using Xunit;

namespace OTPManager.Shared.Test.ViewModels
{
    public class OTPDisplayViewModelTest : TestBase<OTPDisplayViewModel>
    {
        private readonly OTPGenerator Generator = CreateOTPGenerator(124);

        protected override OTPDisplayViewModel GetTarget()
        {
            return new OTPDisplayViewModel(NavigatorMock.Object, PlatformServiceMock.Object, Generator);
        }

        [Fact]
        public void CopyWorks()
        {
            Target.CopyToClipboard.Execute();
            PlatformServiceMock.Verify(d => d.SetClipboardText(Target.OTP, null));
        }
    }
}
