using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using OTPManager.Shared.Components;
using OTPManager.Shared.Models;
using OTPManager.Shared.Services;

namespace OTPManager.Shared.ViewModels
{
    public class AddGeneratorViewModel : MvxViewModel<OTPGenerator>
    {
        public const int MinSecretLength = 12;
        internal const bool AllowExportingDefault = false;

        private readonly IMvxNavigationService Navigator;
        private readonly IStorageService DataStore;

        public MvxCommand AddGenerator { get; private set; }

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

        public override void Prepare(OTPGenerator parameter)
        {
            ResetDefaults();

            Label = parameter.Label;
            SecretBase32 = parameter.SecretBase32;
            Issuer = parameter.Issuer;
        }

        public AddGeneratorViewModel(IMvxNavigationService navigator, IStorageService dataStore)
        {
            Navigator = navigator;
            DataStore = dataStore;

            AddGenerator = new MvxCommand(CreateEntryHandler, () => DataIsValid);
            ResetDefaults();
        }

        private async void CreateEntryHandler()
        {
            var otpGenerator = new OTPGenerator()
            {
                Label = Label,
                Secret = OTPBase32Converter.FromBase32String(SecretBase32),
                Issuer = Issuer,
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

            if (!OTPBase32Converter.IsValidBase32String(SecretBase32))
            {
                return;
            }

            DataIsValid = true;
        }
    }
}
