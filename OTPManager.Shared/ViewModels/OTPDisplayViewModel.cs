using MvvmCross.Commands;
using MvvmCross.ViewModels;
using OTPManager.Shared.Models;
using Plugin.Share.Abstractions;
using System;

namespace OTPManager.Shared.ViewModels
{
    public class OTPDisplayViewModel : MvxViewModel
    {
        private IShare ShareService { get; }
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

        public OTPDisplayViewModel(IShare shareService, OTPGenerator gen)
        {
            ShareService = shareService;

            Generator = gen;
            UpdateOTP(DateTime.UtcNow);

            CopyToClipboard = new MvxCommand(() =>
            {
                ShareService.SetClipboardText(OTP);
            });
        }

        public void UpdateOTP(DateTime time)
        {
            OTP = Generator.GenerateOTP(time);
        }
    }
}
