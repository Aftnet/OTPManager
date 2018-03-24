using SimpleBase;

namespace OTPManager.Shared.Components
{
    internal static class OTPBase32Converter
    {
        public static byte[] FromBase32String(string input)
        {
            return Base32.Rfc4648.Decode(input);
        }

        public static string ToBase32String(byte[] input)
        {
            return Base32.Rfc4648.Encode(input, false);
        }

        public static bool IsValidBase32String(string input)
        {
            try
            {
                Base32.Rfc4648.Decode(input);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
