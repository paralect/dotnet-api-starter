using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_api_starter.Infrastructure.Utils
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

        public static string GetHash(this string str, string salt = "")
        {
            byte[] hash;
            byte[] data = Encoding.UTF8.GetBytes($"{str[0]}{salt}{str.Substring(1, str.Length - 1)}");

            using (SHA256 shaM = new SHA256Managed())
            {
                hash = shaM.ComputeHash(data);
            }
            return BytesToString(hash);
        }

        public static string GenerateSalt()
        {
            byte[] buf = new byte[16];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(buf);
            }

            return BytesToString(buf);
        }

        public static bool IsHashEqual(this string str, string hash, string salt = "")
        {
            return str.GetHash(salt).CompareTo(hash) == 0;
        }

        public static string BytesToString(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", string.Empty);
        }
    }
}
