using Android.App;
using Android.Content;
using OTPManager.Shared.Services;

namespace OTPManager.Android.Services
{
    public class PlatformService : IPlatformService
    {
        public void SetClipboardContent(string content)
        {
            var context = Application.Context;
            var clipboard = (ClipboardManager)context.GetSystemService(Context.ClipboardService);
            var clip = ClipData.NewPlainText("CopiedText", content);
            clipboard.PrimaryClip = clip;
        }
    }
}
