using OTPManager.Shared.Services;
using UIKit;

namespace OTPManager.iOS.Services
{
    public class PlatformService : IPlatformService
    {
        private UIPasteboard Pasteboard = UIPasteboard.General;

        public void SetClipboardContent(string content)
        {
            Pasteboard.String = content;
        }
    }
}
