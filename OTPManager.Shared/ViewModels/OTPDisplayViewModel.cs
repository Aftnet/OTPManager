using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using OTPManager.Shared.Models;
using OTPManager.Shared.Services;
using System;

namespace OTPManager.Shared.ViewModels
{
    public class OTPDisplayViewModel : MvxViewModel
    {
        private readonly IMvxNavigationService Navigator;
        private readonly IPlatformService PlatformService;

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

        public OTPDisplayViewModel(IMvxNavigationService navigator, IPlatformService platformService, OTPGenerator gen)
        {
            Navigator = navigator;
            PlatformService = platformService;

            generator = gen;
            UpdateOTP(DateTimeOffset.UtcNow);

            CopyToClipboard = new MvxCommand(() =>
            {
                PlatformService.SetClipboardContent(OTP);
            });
        }

        public void UpdateOTP(DateTimeOffset time)
        {
            var formatString = $"D{Generator.NumDigits}";
            OTP = Generator.GenerateOTP(time).ToString(formatString);
        }
    }
}
