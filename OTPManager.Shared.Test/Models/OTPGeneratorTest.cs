using OTPManager.Shared.Models;
using OtpNet;
using System;
using System.Collections.Generic;
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
            yield return new object[] { new DateTime(2015, 11, 27, 16, 28, 40), "508637" };
            yield return new object[] { new DateTime(2015, 11, 27, 16, 31, 11), "065374" };
        }

        [Theory]
        [MemberData(nameof(TimeBasedTokenizationWorksData))]
        public void TimeBasedTokenizationWorks(DateTime time, string expectedOTP)
        {
            Target.Secret = Base32Encoding.ToBytes("AAAAAAAAAAAA");
            var otp = Target.GenerateOTP(time);
            Assert.Equal(expectedOTP, otp);
        }
    }
}
