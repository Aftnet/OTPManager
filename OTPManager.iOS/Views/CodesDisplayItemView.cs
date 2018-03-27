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
        public static readonly NSString Key = new NSString("CodesDisplayItemView");
        public static readonly UINib Nib;

        static CodesDisplayItemView()
        {
            Nib = UINib.FromName("CodesDisplayItemView", NSBundle.MainBundle);
        }

        protected CodesDisplayItemView(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
            this.DelayBind(() =>
            {
                //var set = this.CreateBindingSet<CodesDisplayItemView, OTPDisplayViewModel>();
                //set.Bind(imageViewLoader).To(m => m.Image);
                //set.Bind(nameLabel).To(m => m.Name);
                //set.Bind(originLabel).To(m => m.Location);
                //set.Bind(descriptionLabel).To(m => m.Details);
                //set.Apply();
            });
        }
    }
}
