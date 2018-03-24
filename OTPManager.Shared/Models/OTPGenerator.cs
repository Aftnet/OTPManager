using OTPManager.Shared.Components;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace OTPManager.Shared.Models
{
    public class OTPGenerator
    {
        internal const string HMacSha1Name = "SHA1";
        internal const string HMacSha256Name = "SHA256";
        internal const string HMacSha512Name = "SHA512";

        internal static readonly IReadOnlyDictionary<string, Func<byte[], HMAC>> AlgorithmsMapping = new Dictionary<string, Func<byte[], HMAC>>
        {
            { HMacSha1Name, d => new HMACSHA1(d) },
            { HMacSha256Name, d => new HMACSHA256(d) },
            { HMacSha512Name, d => new HMACSHA512(d) }
        };

        private const bool AllowExportingDefault = false;
        private static readonly DateTimeOffset Epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
        internal static readonly TimeSpan TimeStep = new TimeSpan(0, 0, 30);

        public const int MinNumDigits = 6;
        public const int MaxNumDigits = 8;

        public static readonly int[] DigitsPowers = Enumerable.Range(0, MaxNumDigits + 1).Select(d => (int)Math.Pow(10, d)).ToArray();

        [PrimaryKey]
        public string Uid { get; set; }

        public string Label { get; set; }

        public string Issuer { get; set; }

        public bool AllowExporting { get; set; }

        public string AlgorithmName { get; set; }

        [Ignore]
        public byte[] Secret { get; set; }

        public string DbEncryptedSecret { get; set; }

        public string DbEncryptedSecretIV { get; set; }

        [Ignore]
        public string SecretBase32
        {
            get { return Secret != null ? OTPBase32Converter.ToBase32String(Secret) : null; }
        }

        private int numDigits = MinNumDigits;
        public int NumDigits
        {
            get { return numDigits; }
            set { numDigits = Math.Max(Math.Min(value, MaxNumDigits), MinNumDigits); }
        }

        public static OTPGenerator FromUri(Uri input)
        {
            return OTPUriConverter.OTPGeneratorFromUri(input);
        }

        public OTPGenerator()
        {
            Uid = Guid.NewGuid().ToString();
            Label = Issuer = string.Empty;
            AlgorithmName = HMacSha1Name;
            AllowExporting = AllowExportingDefault;
        }
        
        public int GenerateOTP(DateTimeOffset input)
        {
            var timeCode = GenerateTimeCode(input);
            return GenerateOTP(timeCode);
        }

        public int GenerateOTP(UInt64 input)
        {
            var data = BitConverter.GetBytes(input);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(data);
            }

            //create a hash for the known secret and append the challenge
            using (var hasher = AlgorithmsMapping[AlgorithmName](Secret))
            {
                var hmac = hasher.ComputeHash(data);

                int offset = hmac[hmac.Length - 1] & 0xf;

                int binary = ((hmac[offset + 0] & 0x7F) << 24) |
                            ((hmac[offset + 1] & 0xFF) << 16) |
                            ((hmac[offset + 2] & 0xFF) << 8) |
                            ((hmac[offset + 3] & 0xFF));

                int password = binary % DigitsPowers[NumDigits];

                //return the OTP with zero-padding for the number of desired digits
                return password;
            }
        }

        public Uri ToUri()
        {
            return OTPUriConverter.UriFromOTPGenerator(this);
        }

        public static UInt64 GenerateTimeCode(DateTimeOffset input)
        {
            var difference = (UInt64)Math.Abs(input.Subtract(Epoch).TotalSeconds);
            var output = difference / (UInt64)TimeStep.Seconds;
            return output;
        }
    }
}
