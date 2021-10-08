using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.TextField;

namespace OTPManager.Android.Properties
{
    [Preserve(AllMembers = true)]
    public sealed class LinkerIncludes
    {
        public void Include(ProgressBar target)
        {
            target.Max = target.Progress;
            target.Visibility = global::Android.Views.ViewStates.Visible;
        }

        public void Include(Button target)
        {
            target.Enabled = !target.Enabled;
        }

        public void Include(TextInputEditText target)
        {
            target.BeforeTextChanged += (d, e) => { target.Text += ""; };
            target.TextChanged += (d, e) => { target.Text += ""; };
            target.AfterTextChanged += (d, e) => { target.Text += ""; };
        }

        public void Include(SwitchCompat target)
        {
            target.CheckedChange += (d, e) => { target.Checked = !target.Checked; };
        }

        public void Include(LinearLayout target)
        {
            target.Visibility = global::Android.Views.ViewStates.Visible;
        }
    }
}
