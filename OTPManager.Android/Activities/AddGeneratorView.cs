using Android.App;
using Android.OS;
using Android.Views;
using MvvmCross.Droid.Support.V7.AppCompat;
using OTPManager.Shared.ViewModels;

namespace OTPManager.Android.Activities
{
    [Activity(Label = "@string/Add")]
    public class AddGeneratorView : MvxAppCompatActivity<AddGeneratorViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.add_generator);
        }
    }
}