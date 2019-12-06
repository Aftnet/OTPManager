using OTPManager.Shared.Components;
using OTPManager.Shared.Models;
using System;
using Xunit;

namespace OTPManager.Shared.Test.Components
{
    public class OTPUriConverterTest
    {
        protected static readonly OTPGenerator SampleGenerator = new OTPGenerator
        {
            Label = "Alice Loller@test.com",
            SecretBase32 = "ABABABABABABABAB",
            AlgorithmName = OTPGenerator.HMacSha256Name,
            Issuer = "Test"
        };

        [Fact]
        public void GenerateUriWorks()
        {
            var reference = "otpauth://totp/Test:Alice Loller%40test.com?secret=ABABABABABABABAB&algorithm=SHA256&digits=6&issuer=Test";
            var uri = OTPUriConverter.UriFromOTPGenerator(SampleGenerator);
            Assert.Equal(reference, uri.ToString());
        }

        [Fact]
        public void InitializeFromUriWorks()
        {
            var generatedUri = OTPUriConverter.UriFromOTPGenerator(SampleGenerator);
            var model = OTPUriConverter.OTPGeneratorFromUri(generatedUri);

            Assert.NotNull(model);
            Assert.NotNull(model.Label);
            Assert.Equal(SampleGenerator.Label, model.Label);
            Assert.NotNull(SampleGenerator.Secret);
            Assert.Equal(SampleGenerator.Secret, model.Secret);
            Assert.Equal(SampleGenerator.NumDigits, model.NumDigits);
            Assert.NotEmpty(model.AlgorithmName);
            Assert.Equal(SampleGenerator.AlgorithmName, model.AlgorithmName);
            Assert.NotNull(model.Issuer);
            Assert.Equal(SampleGenerator.Issuer, model.Issuer);
            Assert.Equal(SampleGenerator.AllowExporting, model.AllowExporting);
            Assert.NotNull(model.Uid);
            Assert.NotEqual(SampleGenerator.Uid, model.Uid);
        }

        [Theory]
        [InlineData("otpauth://totp/Test:Alice Loller%40test.com?secret=ABABABABABABABAB&algorithm=SHA256&digits=6&issuer=Test", "Alice Loller@test.com", "Test")]
        [InlineData("otpauth://totp/Test:Alice Loller%40test.com?secret=ABABABABABABABAB&algorithm=SHA256&digits=6", "Alice Loller@test.com", "Test")]
        [InlineData("otpauth://totp/Alice Loller%40test.com?secret=ABABABABABABABAB&algorithm=SHA256&digits=6&issuer=Test", "Alice Loller@test.com", "Test")]
        [InlineData("otpauth://totp/Omg:Alice Loller%40test.com?secret=ABABABABABABABAB&algorithm=SHA256&digits=6&issuer=Test", "Alice Loller@test.com", "Test")]
        [InlineData("otpauth://totp/Alice Loller%40test.com?secret=ABABABABABABABAB&algorithm=SHA256&digits=6", "Alice Loller@test.com", "")]
        [InlineData("otpauth://totp/Alice Loller%40test.com?secret=ABABABABABABABAB&algorithm=SHA256&digits=6&issuer=", "Alice Loller@test.com", "")]
        [InlineData("otpauth://totp/:Alice Loller%40test.com?secret=ABABABABABABABAB&algorithm=SHA256&digits=6", "Alice Loller@test.com", "")]
        public void LabelAndIssuerAreCorrectlyParsed(string uriString, string expectedLabel, string expectedIssuer)
        {
            var uri = new Uri(uriString);
            var model = OTPUriConverter.OTPGeneratorFromUri(uri);

            Assert.Equal(expectedLabel, model.Label);
            Assert.Equal(expectedIssuer, model.Issuer);
        }

        [Theory]
        [InlineData("tpauth://totp/SomeLabel?secret=ABABABABABABABAB&algorithm=SHA256&digits=6")] //Wrong scheme
        [InlineData("otpauth://hotp/SomeLabel?secret=ABABABABABABABAB&algorithm=SHA256&digits=6")] //Wrong path
        [InlineData("otpauth://totp/?secret=ABABABABABABABAB&algorithm=SHA256&digits=6")] //No label
        [InlineData("otpauth://totp/SomeLabel?secret=&algorithm=SHA256&digits=6")] //No secret
        [InlineData("otpauth://totp/SomeLabel?algorithm=SHA256&digits=6")] //No secret
        [InlineData("otpauth://totp/SomeLabel?secret=ABABABAB@ABABABAB&algorithm=SHA256&digits=6")] //No base32 secret
        [InlineData("otpauth://totp/SomeLabel?secret=ABABABABABABABAZSB&algorithm=SHA256&digits=d")] //Invalid base32 secret
        [InlineData("otpauth://totp/SomeLabel?secret=ABABABABABABABAB&algorithm=&digits=6")] //No algorithm
        [InlineData("otpauth://totp/SomeLabel?secret=ABABABABABABABAB&algorithm=SHA12&digits=6")] //Wrong algorithm
        [InlineData("otpauth://totp/SomeLabel?secret=ABABABABABABABAB&algorithm=SHA256&digits=")] //No digits
        [InlineData("otpauth://totp/SomeLabel?secret=ABABABABABABABAB&algorithm=SHA256&digits=d")] //Digits not a number
        public void InvalidUriCreatesNullGenerator(string uriString)
        {
            var uri = new Uri(uriString);
            var model = OTPUriConverter.OTPGeneratorFromUri(uri);
            Assert.Null(model);
        }
    }
}
