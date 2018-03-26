using System;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using OTPManager.Shared.ViewModels;
using UIKit;

namespace OTPManager.iOS
{
    [MvxFromStoryboard("AddGenerator")]
    public partial class AddGeneratorView : MvxViewController<AddGeneratorViewModel>
    {
        public AddGeneratorView (IntPtr handle) : base (handle)
        {
        }
    }
}