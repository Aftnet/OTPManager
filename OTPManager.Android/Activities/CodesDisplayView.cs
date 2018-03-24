using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using MvvmCross.Binding.Droid.Views;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Platform;
using MvvmCross.Platform.Droid.Platform;
using OTPManager.Shared.Services;
using OTPManager.Shared.ViewModels;
using System;
using ZXing.Mobile;
using static Android.Widget.AdapterView;

namespace OTPManager.Android.Activities
{
    [Activity(Label = "@string/AppName", MainLauncher = true, LaunchMode = global::Android.Content.PM.LaunchMode.SingleTop)]
    [IntentFilter(new[] { global::Android.Content.Intent.ActionView }, DataScheme = "otpauth", Categories = new[] { global::Android.Content.Intent.CategoryDefault, global::Android.Content.Intent.CategoryBrowsable})]
    public class CodesDisplayView : MvxAppCompatActivity<CodesDisplayViewModel>
    {
        private static Lazy<object> Initializer;

        public CodesDisplayView()
        {
            Initializer = new Lazy<object>(() =>
            {
                UserDialogs.Init(() => Mvx.Resolve<IMvxAndroidCurrentTopActivity>().Activity);
                MobileBarcodeScanner.Initialize(Application);
                return new object();
            }, System.Threading.LazyThreadSafetyMode.PublicationOnly);
        }

        protected override void OnCreate(Bundle bundle)
        {
            var value = Initializer.Value;

            base.OnCreate(bundle);
            SetContentView(Resource.Layout.codes_display);

            var list = FindViewById<MvxListView>(Resource.Id.codes_display_list);
            RegisterForContextMenu(list);

            OnNewIntent(Intent);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            var data = Intent.DataString;
            if (data != null)
            {
                if (Uri.TryCreate(data, UriKind.Absolute, out var qrUri))
                {
                    var uriService = Mvx.Resolve<IUriService>();
                    uriService.CreateGeneratorFromUri(qrUri);
                }
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.codes_display, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            if (v.Id == Resource.Id.codes_display_list)
            {
                MenuInflater.Inflate(Resource.Menu.codes_display_list_item, menu);
            }

            base.OnCreateContextMenu(menu, v, menuInfo);
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.MenuCopyToClipboard)
            {
                var menuInfo = item.MenuInfo as AdapterContextMenuInfo;
                var target = ViewModel.Items[menuInfo.Position];
                var platformService = Mvx.Resolve<IPlatformService>();
                platformService.SetClipboardContent(target.OTP);
                return true;
            }

            return base.OnContextItemSelected(item);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MenuAddCamera:
                    ViewModel.CreateEntryQR.Execute(null);
                    return true;

                case Resource.Id.MenuAddManual:
                    ViewModel.CreateEntryManual.Execute(null);
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}