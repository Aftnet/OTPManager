using OTPManager.Shared.Services;
using Windows.ApplicationModel.DataTransfer;

namespace OTPManager.UWP.Services
{
    public class PlatformService : IPlatformService
    {
        public void SetClipboardContent(string content)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(content);
            Clipboard.SetContent(dataPackage);
        }
    }
}
