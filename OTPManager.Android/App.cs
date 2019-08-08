using Android.App;
using Android.Runtime;
using MvvmCross.Platforms.Android.Views;
using OTPManager.Android.Platform;
using System;

namespace OTPManager.Android
{
    [Application]
    public class App : MvxAndroidApplication<CustomSetup<Shared.App>, Shared.App>
    {
        public App(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }
    }
}