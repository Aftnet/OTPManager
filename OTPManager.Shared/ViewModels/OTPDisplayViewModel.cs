using System;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using OTPManager.Shared.Models;
using Plugin.Share.Abstractions;

namespace OTPManager.Shared.ViewModels
{
    public class OTPDisplayViewModel : MvxViewModel
    {
        private readonly IMvxNavigationService Navigator;
        private readonly IShare ShareService;

        private readonly OTPGenerator generator;
        public OTPGenerator Generator => generator;

        public string Issuer => Generator.Issuer;
        public string Label => Generator.Label;

        private string otp;
        public string OTP
        {
            get => otp;
            private set { SetProperty(ref otp, value); }
        }

        public MvxCommand CopyToClipboard { get; private set; }

        public OTPDisplayViewModel(IMvxNavigationService navigator, IShare shareService, OTPGenerator gen)
        {
            Navigator = navigator;
            ShareService = shareService;

            generator = gen;
            UpdateOTP(DateTimeOffset.UtcNow);

            CopyToClipboard = new MvxCommand(() =>
            {
                ShareService.SetClipboardText(OTP);
            });
        }

        public void UpdateOTP(DateTimeOffset time)
        {
            var formatString = $"D{Generator.NumDigits}";
            OTP = Generator.GenerateOTP(time).ToString(formatString);
        }
    }
}
