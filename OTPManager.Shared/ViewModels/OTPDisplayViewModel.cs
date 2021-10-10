using MvvmCross.Commands;
using MvvmCross.ViewModels;
using OTPManager.Shared.Models;
using System;

namespace OTPManager.Shared.ViewModels
{
    public class OTPDisplayViewModel : MvxViewModel
    {
        public OTPGenerator Generator { get; }

        public string Issuer => Generator.Issuer;
        public string Label => Generator.Label;

        private string otp;
        public string OTP
        {
            get => otp;
            private set => SetProperty(ref otp, value);
        }

        public IMvxCommand CopyToClipboard { get; }

        public OTPDisplayViewModel(OTPGenerator gen)
        {
            Generator = gen;
            UpdateOTP(DateTime.UtcNow);

            CopyToClipboard = new MvxCommand(() =>
            {
                Xamarin.Essentials.Clipboard.SetTextAsync(OTP);
            });
        }

        public void UpdateOTP(DateTime time)
        {
            OTP = Generator.GenerateOTP(time);
        }
    }
}
