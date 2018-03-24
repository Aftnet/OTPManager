using OTPManager.Shared.ViewModels;
using System.Threading.Tasks;
using Xunit;

namespace OTPManager.Shared.Test.ViewModels
{
    public class DisplayGeneratorViewModelTest : TestBase<DisplayGeneratorViewModel>
    {
        private readonly TaskCompletionSource<bool> QRGenerationTCS = new TaskCompletionSource<bool>();

        protected override DisplayGeneratorViewModel GetTarget()
        {
            var output = new DisplayGeneratorViewModel(NavigatorMock.Object, DataStoreMock.Object);
            output.ShouldAlwaysRaiseInpcOnUserInterfaceThread(false);
            output.PropertyChanged += (d, e) =>
            {
                if (e.PropertyName == nameof(Target.QRData))
                {
                    QRGenerationTCS.SetResult(true);
                }
            };

            return output;
        }

        [Fact]
        public async Task DataIsDisplayed()
        {
            Assert.Null(Target.Generator);
            Assert.Null(Target.Issuer);
            Assert.Null(Target.Label);
            Assert.Null(Target.SecretBase32);
            Assert.Null(Target.QRData);
            Assert.False(Target.AllowExporting);

            var generator = CreateOTPGenerator(2);
            Target.Prepare(generator);

            Assert.Equal(generator.Label, Target.Label);
            Assert.Equal(generator.SecretBase32, Target.SecretBase32);
            Assert.Equal(generator.Issuer, Target.Issuer);
            Assert.Equal(generator.AllowExporting, Target.AllowExporting);

            await QRGenerationTCS.Task;

            Assert.NotNull(Target.QRData);
        }

        [Fact]
        public async void DeletingWorks()
        {
            var generator = CreateOTPGenerator(2);
            Target.Prepare(generator);

            await QRGenerationTCS.Task;

            Assert.True(Target.DeleteGenerator.CanExecute(null));
            Target.DeleteGenerator.Execute(null);

            DataStoreMock.Verify(d => d.DeleteAsync(generator));
            NavigatorMock.Verify(d => d.Close(Target));
        }
    }
}
