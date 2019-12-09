using System;
using System.Security.Cryptography;
using System.Text;

namespace Api.Core.Utils
{
    public static class SecurityUtils
    {
        public static string GenerateSecureToken()
        {
            byte[] buf = new byte[48];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(buf);
            }

            return BytesToString(buf);
        }

        public static string GetHash(this string str)
        {
            byte[] hash;
            byte[] data = Encoding.UTF8.GetBytes($"{str[0]}{str.Substring(1, str.Length - 1)}");

            using (SHA256 shaM = new SHA256Managed())
            {
                hash = shaM.ComputeHash(data);
            }
            return BytesToString(hash);
        }

        public static bool IsHashEqual(this string str, string hash)
        {
            return string.Compare(str.GetHash(), hash, StringComparison.Ordinal) == 0;
        }

        public static string BytesToString(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", string.Empty);
        }
    }
}
