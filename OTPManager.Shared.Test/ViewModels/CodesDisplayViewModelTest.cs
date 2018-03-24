using OTPManager.Shared.Models;
using OTPManager.Shared.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OTPManager.Shared.Test.ViewModels
{
    public class CodesDisplayViewModelTest : TestBase<CodesDisplayViewModel>
    {
        private static readonly IReadOnlyList<OTPGenerator> TestGenerators = Enumerable.Range(1, 3).Select(d => CreateOTPGenerator(d)).ToArray();

        protected override CodesDisplayViewModel GetTarget()
        {
            return new CodesDisplayViewModel(NavigatorMock.Object, PlatformServiceMock.Object, DataStoreMock.Object, BarcodeScannerMock.Object, UriServiceMock.Object);
        }

        public CodesDisplayViewModelTest() : base()
        {
            DataStoreMock.Setup(d => d.GetAllAsync()).Returns(Task.FromResult(TestGenerators.ToList()));
        }

        [Fact]
        public async Task LoadingWorks()
        {
            await Target.ViewAppearingAsync();
            Assert.True(Target.LastUpdateTimeCode > CodesDisplayViewModel.DefaultUpdateTimeCode);
            Assert.True(Target.GeneratorsAvailable);

            Assert.Equal(TestGenerators.Count, Target.Items.Count);
            Assert.Equal(TestGenerators, Target.Items.Select(d=>d.Generator).ToArray());
        }

        [Fact]
        public void ManuallyCreatingEntryWorks()
        {
            Target.CreateEntryManual.Execute(null);

            NavigatorMock.Verify(d => d.Navigate<AddGeneratorViewModel>(null));
        }

        [Fact]
        public void CreateGeneratorFromQRWorks()
        {
            var scanOutput = new ZXing.Result("SomeText", null, null, ZXing.BarcodeFormat.QR_CODE);
            BarcodeScannerMock.Setup(d => d.Scan()).Returns(Task.FromResult(scanOutput));

            Target.CreateEntryQR.Execute(null);
            UriServiceMock.Verify(d => d.CreateGeneratorFromUri(scanOutput.Text));
        }

        [Fact]
        public async Task SelectionWorks()
        {
            await Target.ViewAppearingAsync();
            var selectedItem = Target.Items.First();

            Target.ItemClicked.Execute(selectedItem);
            NavigatorMock.Verify(d => d.Navigate<DisplayGeneratorViewModel, OTPGenerator>(selectedItem.Generator, null));
        }
    }
}
