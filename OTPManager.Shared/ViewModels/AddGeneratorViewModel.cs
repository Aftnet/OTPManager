using System;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using OTPManager.Shared.Models;
using OTPManager.Shared.Services;
using OtpNet;

namespace OTPManager.Shared.ViewModels
{
    public class AddGeneratorViewModel : MvxViewModel<AddGeneratorViewModel.Parameter>
    {
        public class Parameter
        {
            public OTPGenerator Generator { get; }
            public bool IsInitialView { get; }

            public Parameter(OTPGenerator generator, bool isInitialView)
            {
                Generator = generator ?? throw new ArgumentNullException(nameof(generator));
                IsInitialView = isInitialView;
            }
        }

        public const int MinSecretLength = 12;
        internal const bool AllowExportingDefault = false;

        private IMvxNavigationService Navigator { get; }
        private IStorageService DataStore { get; }

        private bool IsInitialView { get; set; } = false;

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

        public IMvxCommand AddGenerator { get; }
        public IMvxCommand Cancel { get; }

        public override void Prepare(AddGeneratorViewModel.Parameter parameter)
        {
            ResetDefaults();

            IsInitialView = parameter.IsInitialView;

            var generator = parameter.Generator;
            Label = generator.Label;
            SecretBase32 = generator.SecretBase32;
            Issuer = generator.Issuer;
        }

        public AddGeneratorViewModel(IMvxNavigationService navigator, IStorageService dataStore)
        {
            Navigator = navigator;
            DataStore = dataStore;

            AddGenerator = new MvxCommand(CreateEntryHandler, () => DataIsValid);
            Cancel = new MvxCommand(() => CloseAsync());

            ResetDefaults();
        }

        private async void CreateEntryHandler()
        {
            var otpGenerator = new OTPGenerator()
            {
                Label = Label,
                Secret = Base32Encoding.ToBytes(SecretBase32),
                Issuer = Issuer,
                AllowExporting = AllowExporting
            };

            await DataStore.InsertOrReplaceAsync(otpGenerator);
            ResetDefaults();

            await CloseAsync();
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

        private Task<bool> CloseAsync()
        {
            if (IsInitialView)
            {
                return Navigator.Navigate<CodesDisplayViewModel>();
            }
            else
            {
                return Navigator.Close(this);
            }
        }
    }
}
