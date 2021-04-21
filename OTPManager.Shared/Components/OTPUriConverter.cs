using OTPManager.Shared.Models;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Net;

namespace OTPManager.Shared.Components
{
    internal static class OTPUriConverter
    {
        public const string UriScheme = "otpauth";
        public const string UriHost = "totp";
        public const string UriQueryLabelDelimiter = ":";
        public const string UriQuerySecret = "secret";
        public const string UriQueryAlgorithm = "algorithm";
        public const string UriQueryDigits = "digits";
        public const string UriQueryIssuer = "issuer";
        public static readonly string UriFormat = string.Format("{0}://{1}/{{0}}?{2}={{1}}&{3}={{2}}&{4}={{3}}&{5}={{4}}",
            UriScheme, UriHost, UriQuerySecret, UriQueryAlgorithm, UriQueryDigits, UriQueryIssuer);

        public static OTPGenerator OTPGeneratorFromUri(Uri input)
        {
            if (input.Scheme != UriScheme)
            {
                return null;
            }

            if (input.Host != UriHost)
            {
                return null;
            }
            //Label required parameter
            const int expectedSegmentsLength = 2;
            if (input.Segments.Length != expectedSegmentsLength)
            {
                return null;
            }
            var labelSegment = input.Segments[expectedSegmentsLength - 1];
            if (string.IsNullOrEmpty(labelSegment))
            {
                return null;
            }

            var output = new OTPGenerator();

            labelSegment = WebUtility.UrlDecode(labelSegment);
            //If an issuer has been included in the label segment, extract it
            var delimiterPosition = labelSegment.IndexOf(UriQueryLabelDelimiter);
            if (delimiterPosition == -1) //no issuer
            {
                output.Label = labelSegment;
            }
            else
            {
                output.Issuer = labelSegment.Substring(0, delimiterPosition);
                output.Label = labelSegment.Substring(delimiterPosition + 1);
            }

            //Secret required parameter
            string value;
            var queryValues = ParseQueryString(input.Query);

            if (!queryValues.ContainsKey(UriQuerySecret))
            {
                return null;
            }

            value = queryValues[UriQuerySecret];
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            try
            {
                output.SecretBase32 = value;
            }
            catch
            {
                return null;
            }

            //Num digits optional parameter
            if (queryValues.ContainsKey(UriQueryDigits))
            {
                try
                {
                    output.NumDigits = int.Parse(queryValues[UriQueryDigits]);
                }
                catch
                {
                    return null;
                }
            }

            //Algorithm optional parameter
            if (queryValues.ContainsKey(UriQueryAlgorithm))
            {
                value = queryValues[UriQueryAlgorithm].ToUpperInvariant();
                if (!OTPGenerator.AlgorithmsMapping.ContainsKey(value))
                {
                    return null;
                }

                output.AlgorithmName = value;
            }

            //Issuer optional parameter
            if (queryValues.ContainsKey(UriQueryIssuer))
            {
                output.Issuer = queryValues[UriQueryIssuer];
            }

            return output;
        }

        public static Uri UriFromOTPGenerator(OTPGenerator input)
        {
            var uriLabel = string.Format("{0}{1}{2}", Uri.EscapeDataString(input.Issuer), UriQueryLabelDelimiter, Uri.EscapeDataString(input.Label));
            var output = string.Format(UriFormat, uriLabel, input.SecretBase32.Replace("=", string.Empty), input.AlgorithmName, input.NumDigits, Uri.EscapeDataString(input.Issuer));
            return new Uri(output);
        }

        private static Dictionary<string, string> ParseQueryString(string query)
        {
            var output = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(query) || query.Length < 1)
            {
                return output;
            }

            query = query.Substring(1);
            var keyValuesArr = query.Split('&');
            foreach (var i in keyValuesArr)
            {
                var keyValue = i.Split('=');
                if (keyValue.Length == 2)
                {
                    var key = keyValue[0];
                    var value = keyValue[1];
                    if (!(string.IsNullOrEmpty(key) || value == null))
                    {
                        output.Add(key, value);
                    }
                }
            }

            return output;
        }
    }
}
