using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using OTPManager.Shared.ViewModels;
using System;

namespace OTPManager.iOS
{
    public partial class CodesDisplayItemView : MvxTableViewCell
    {
        protected CodesDisplayItemView(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<CodesDisplayItemView, OTPDisplayViewModel>();
                set.Bind(LabelDisplay).To(m => m.Label);
                set.Bind(OTPDisplay).To(m => m.OTP);
                set.Bind(IssuerDisplay).To(m => m.Issuer);
                set.Apply();
            });
        }
    }
}