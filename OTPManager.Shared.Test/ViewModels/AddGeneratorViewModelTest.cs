using Moq;
using OTPManager.Shared.Models;
using OTPManager.Shared.ViewModels;
using Xunit;

namespace OTPManager.Shared.Test.ViewModels
{
    public class AddGeneratorViewModelTest : TestBase<AddGeneratorViewModel>
    {
        protected override AddGeneratorViewModel GetTarget()
        {
            return new AddGeneratorViewModel(NavigatorMock.Object, DataStoreMock.Object);
        }

        [Fact]
        public void CreationWorks()
        {
            var generator = CreateOTPGenerator(1);
            Target.Label = generator.Label;
            Target.Issuer = generator.Issuer;
            Target.SecretBase32 = generator.SecretBase32;
            Target.AllowExporting = !AddGeneratorViewModel.AllowExportingDefault;

            Target.AddGenerator.Execute(null);

            DataStoreMock.Verify(d => d.InsertOrReplaceAsync(It.Is<OTPGenerator>(
                e => e.Label == generator.Label && e.Issuer == generator.Issuer
                && e.SecretBase32 == generator.SecretBase32 && e.AllowExporting == !AddGeneratorViewModel.AllowExportingDefault)));
            NavigatorMock.Verify(d => d.Close(Target));

            Assert.Empty(Target.Label);
            Assert.Empty(Target.SecretBase32);
            Assert.Empty(Target.Issuer);
            Assert.Equal(AddGeneratorViewModel.AllowExportingDefault, Target.AllowExporting);
        }

        [Fact]
        public void DataValidationWorks()
        {
            Assert.False(Target.DataIsValid);

            Target.Label = "a";
            Assert.False(Target.DataIsValid);
            Target.Issuer = "b";
            Assert.False(Target.DataIsValid);
            Target.SecretBase32 = "a";
            Assert.False(Target.DataIsValid);
            Target.SecretBase32 = "ab";
            Assert.False(Target.DataIsValid);
            Target.SecretBase32 = "CCCCCCCCCCCC";
            Assert.True(Target.DataIsValid);
        }

        [Fact]
        public void PrefillWorks()
        {
            Target.AllowExporting = !AddGeneratorViewModel.AllowExportingDefault;
            var generator = CreateOTPGenerator(1);
            Target.Prepare(generator);

            Assert.Equal(generator.Label, Target.Label);
            Assert.Equal(generator.Issuer, Target.Issuer);
            Assert.Equal(generator.SecretBase32, Target.SecretBase32);
            Assert.Equal(AddGeneratorViewModel.AllowExportingDefault, Target.AllowExporting);
        }

        [Fact]
        public void CancelingWorks()
        {
            Target.Cancel.Execute(null);
            DataStoreMock.Verify(d => d.InsertOrReplaceAsync(It.IsAny<OTPGenerator>()), Times.Never());
            NavigatorMock.Verify(d => d.Close(Target));
        }
    }
}
