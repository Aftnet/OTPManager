using OTPManager.Shared.Models;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OTPManager.Shared.Test.Models
{
    public class OTPGeneratorTest : TestBase<OTPGenerator>
    {
        protected override OTPGenerator GetTarget()
        {
            return CreateOTPGenerator(10);
        }

        [Fact]
        public void NewEntryModelHasGuid()
        {
            Guid parsedGuid;
            var result = Guid.TryParse(Target.Uid, out parsedGuid);
            Assert.True(result);
        }

        [Fact]
        public void NumDigitsIsConstrainedToRange()
        {
            Target.NumDigits = OTPGenerator.MinNumDigits - 4;
            Assert.Equal(OTPGenerator.MinNumDigits, Target.NumDigits);
            Target.NumDigits = OTPGenerator.MaxNumDigits + 4;
            Assert.Equal(OTPGenerator.MaxNumDigits, Target.NumDigits);
        }

        public static IEnumerable<object[]> TimeBasedTokenizationWorksData()
        {
            yield return new object[] { new DateTimeOffset(2015, 11, 27, 16, 28, 40, TimeSpan.Zero), 508637 };
            yield return new object[] { new DateTimeOffset(2015, 11, 27, 16, 31, 11, TimeSpan.Zero), 065374 };
        }

        [Theory]
        [MemberData(nameof(TimeBasedTokenizationWorksData))]
        public void TimeBasedTokenizationWorks(DateTimeOffset time, int expectedOTP)
        {
            Target.Secret = Base32Encoding.ToBytes("AAAAAAAAAAAA");
            var otp = Target.GenerateOTP(time);
            Assert.Equal(expectedOTP, otp);
        }

        [Theory]
        [InlineData(0, 755224)]
        [InlineData(1, 287082)]
        [InlineData(2, 359152)]
        public void Sha1Works(ulong timeStamp, int expectedOTP)
        {
            Target.AlgorithmName = OTPGenerator.HMacSha1Name;
            Target.Secret = Encoding.UTF8.GetBytes("12345678901234567890");

            var otp = Target.GenerateOTP(timeStamp);
            Assert.Equal(expectedOTP, otp);
        }

        [Fact]
        public void Sha512Works()
        {
            Target.AlgorithmName = OTPGenerator.HMacSha512Name;
            Target.Secret = Encoding.UTF8.GetBytes("SomeKey");

            var interval = 66778UL;
            var otp = Target.GenerateOTP(interval);

            Assert.Equal(539281, otp);
        }
    }
}
