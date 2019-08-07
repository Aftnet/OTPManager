using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using OTPManager.Shared.Models;
using OTPManager.Shared.Services;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;

namespace OTPManager.Shared.ViewModels
{
    public class DisplayGeneratorViewModel : MvxViewModel<OTPGenerator>
    {
        private readonly IMvxNavigationService Navigator;
        private readonly IStorageService DataStore;

        private static readonly BarcodeWriterGeneric QRWriter = new BarcodeWriterGeneric()
        {
            Format = BarcodeFormat.QR_CODE
        };

        private OTPGenerator generator;
        public OTPGenerator Generator
        {
            get => generator;
            private set
            {
                if (SetProperty(ref generator, value))
                {
                    Label = generator.Label;
                    SecretBase32 = generator.SecretBase32;
                    Issuer = generator.Issuer;
                    AllowExporting = generator.AllowExporting;
                    DeleteGenerator.RaiseCanExecuteChanged();
                    var task = GenerateQRAsync();
                }
            }
        }

        private string label;
        public string Label
        {
            get => label;
            private set { SetProperty(ref label, value); }
        }

        private string secretBase32;
        public string SecretBase32
        {
            get => secretBase32;
            private set { SetProperty(ref secretBase32, value); }
        }

        private string issuer;
        public string Issuer
        {
            get => issuer;
            private set { SetProperty(ref issuer, value); }
        }

        private bool allowExporting;
        public bool AllowExporting
        {
            get => allowExporting;
            private set { SetProperty(ref allowExporting, value); }
        }

        private BitMatrix qrData;
        public BitMatrix QRData
        {
            get  => qrData;
            private set { SetProperty(ref qrData, value); }
        }

        public IMvxCommand DeleteGenerator { get; }

        public override void Prepare(OTPGenerator parameter)
        {
            Generator = parameter;
        }

        public DisplayGeneratorViewModel(IMvxNavigationService navigator, IStorageService dataStore)
        {
            Navigator = navigator;
            DataStore = dataStore;

            DeleteGenerator = new MvxCommand(DeleteGeneratorHandler, () => Generator != null);
        }

        private async void DeleteGeneratorHandler()
        {
            await DataStore.DeleteAsync(Generator);
            await Navigator.Close(this);
        }

        private async Task GenerateQRAsync()
        {
            QRData = null;
            if (Generator == null)
            {
                return;
            }
            
            var uri = Generator.ToUri();
            QRData = await Task.Run(() =>
            {               
                var output = QRWriter.Encode(uri.ToString());
                return output;
            });
        }
    }
}
