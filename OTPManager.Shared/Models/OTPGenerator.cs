﻿using OTPManager.Shared.Components;
using OtpNet;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OTPManager.Shared.Models
{
    public class OTPGenerator
    {
        internal const string HMacSha1Name = "SHA1";
        internal const string HMacSha256Name = "SHA256";
        internal const string HMacSha512Name = "SHA512";

        internal static readonly IReadOnlyDictionary<string, OtpHashMode> AlgorithmsMapping = new Dictionary<string, OtpHashMode>
        {
            { HMacSha1Name, OtpHashMode.Sha1 },
            { HMacSha256Name, OtpHashMode.Sha256 },
            { HMacSha512Name, OtpHashMode.Sha512 }
        };

        private const bool AllowExportingDefault = false;
        internal const int TimeStepSeconds = 30;

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
            get { return Secret != null ? Base32Encoding.ToString(Secret) : null; }
        }

        private int numDigits = MinNumDigits;
        public int NumDigits
        {
            get => numDigits;
            set => numDigits = Math.Max(Math.Min(value, MaxNumDigits), MinNumDigits);
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
        
        public string GenerateOTP(DateTime input)
        {
            var otp = new Totp(Secret, TimeStepSeconds, AlgorithmsMapping[AlgorithmName], NumDigits);
            return otp.ComputeTotp(input);
        }

        public Uri ToUri()
        {
            return OTPUriConverter.UriFromOTPGenerator(this);
        }
    }
}
