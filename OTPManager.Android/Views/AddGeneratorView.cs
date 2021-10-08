using Android.App;
using Android.OS;
using MvvmCross.Platforms.Android.Views;
using OTPManager.Shared.ViewModels;

namespace OTPManager.Android.Views
{
    [Activity(Label = "@string/Add")]
    public class AddGeneratorView : MvxActivity<AddGeneratorViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.add_generator);
        }
    }
}