﻿using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using OTPManager.Shared.Models;
using OTPManager.Shared.Services;
using OtpNet;

namespace OTPManager.Shared.ViewModels
{
    public class AddGeneratorViewModel : MvxViewModel<OTPGenerator>
    {
        public const int MinSecretLength = 12;
        internal const bool AllowExportingDefault = false;

        private IMvxNavigationService Navigator { get; }
        private IStorageService DataStore { get; }

        private bool dataIsValid;
        public bool DataIsValid
        {
            get => dataIsValid;
            private set { if (SetProperty(ref dataIsValid, value)) { AddGenerator.RaiseCanExecuteChanged(); } }
        }

        private string label;
        public string Label
        {
            get => label;
            set { if (SetProperty(ref label, value)) { CheckCreationIsAllowed(); } }
        }

        private string secretBase32;
        public string SecretBase32
        {
            get => secretBase32;
            set { if (SetProperty(ref secretBase32, value)) { CheckCreationIsAllowed(); } }
        }

        private string issuer;
        public string Issuer
        {
            get => issuer;
            set { if (SetProperty(ref issuer, value)) { CheckCreationIsAllowed(); } }
        }

        private bool allowExporting;
        public bool AllowExporting
        {
            get => allowExporting;
            set { SetProperty(ref allowExporting, value); }
        }

        private int numDigits = OTPGenerator.MinNumDigits;
        public int NumDigits
        {
            get => numDigits;
            set { if (SetProperty(ref numDigits, value)) { CheckCreationIsAllowed(); } }
        }

        public IMvxCommand AddGenerator { get; }
        public IMvxCommand Cancel { get; }

        public override void Prepare(OTPGenerator parameter)
        {
            ResetDefaults();

            Label = parameter.Label;
            SecretBase32 = parameter.SecretBase32;
            Issuer = parameter.Issuer;
            NumDigits = parameter.NumDigits;
        }

        public AddGeneratorViewModel(IMvxNavigationService navigator, IStorageService dataStore)
        {
            Navigator = navigator;
            DataStore = dataStore;

            AddGenerator = new MvxCommand(CreateEntryHandler, () => DataIsValid);
            Cancel = new MvxCommand(() => Navigator.Close(this));

            ResetDefaults();
        }

        private async void CreateEntryHandler()
        {
            var otpGenerator = new OTPGenerator()
            {
                Label = Label,
                SecretBase32 = SecretBase32,
                Issuer = Issuer,
                NumDigits = NumDigits,
                AllowExporting = AllowExporting
            };

            await DataStore.InsertOrReplaceAsync(otpGenerator);
            ResetDefaults();

            await Navigator.Close(this);
        }

        private void ResetDefaults()
        {
            Label = SecretBase32 = Issuer = string.Empty;
            AllowExporting = AllowExportingDefault;
        }

        private void CheckCreationIsAllowed()
        {
            DataIsValid = false;

            if (string.IsNullOrEmpty(Label) || string.IsNullOrWhiteSpace(Label))
            {
                return;
            }

            if (string.IsNullOrEmpty(SecretBase32) || string.IsNullOrWhiteSpace(SecretBase32))
            {
                return;
            }

            if (SecretBase32.Length < MinSecretLength)
            {
                return;
            }

            if (NumDigits < OTPGenerator.MinNumDigits || NumDigits > OTPGenerator.MaxNumDigits)
            {
                return;
            }

            try
            {
                Base32Encoding.ToBytes(SecretBase32);
            }
            catch
            {
                return;
            }

            DataIsValid = true;
        }
    }
}
