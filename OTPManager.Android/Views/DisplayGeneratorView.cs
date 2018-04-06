using Android.App;
using Android.OS;
using Android.Views;
using MvvmCross.Droid.Support.V7.AppCompat;
using OTPManager.Shared.ViewModels;

namespace OTPManager.Android.Views
{
    [Activity(Label = "@string/Details")]
    public class DisplayGeneratorView : MvxAppCompatActivity<DisplayGeneratorViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.display_generator);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.display_generator, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MenuDeleteGenerator:
                    if (ViewModel.DeleteGenerator.CanExecute())
                    {
                        ViewModel.DeleteGenerator.Execute(null);
                    }

                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}