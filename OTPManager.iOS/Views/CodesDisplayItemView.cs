using System;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using OTPManager.Shared.ViewModels;

namespace OTPManager.iOS
{
    public partial class CodesDisplayItemView : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(CodesDisplayItemView));

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