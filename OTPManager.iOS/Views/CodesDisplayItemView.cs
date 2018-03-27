using System;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using UIKit;
using OTPManager.Shared.ViewModels;

namespace OTPManager.iOS.Views
{
    public partial class CodesDisplayItemView : MvxTableViewCell
    {
        protected CodesDisplayItemView(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<CodesDisplayItemView, OTPDisplayViewModel>();
                set.Bind(Label).To(m => m.Label);
                set.Bind(OTP).To(m => m.OTP);
                set.Bind(Issuer).To(m => m.Issuer);
                set.Apply();
            });
        }
    }
}
