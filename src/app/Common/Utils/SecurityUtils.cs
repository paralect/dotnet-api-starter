using System;
using System.Security.Cryptography;

namespace Common.Utils
{
    public static class SecurityUtils
    {
        public static string GenerateSecureToken(int tokenLength = 48)
        {
            byte[] buf = new byte[tokenLength];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(buf);
            }

            return BytesToString(buf);
        }

        public static string GetHash(this string str)
        {
            return BCrypt.Net.BCrypt.HashPassword(str, 10);
        }

        public static bool IsHashEqual(this string str, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(str, hash);
        }

        public static string BytesToString(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", string.Empty);
        }
    }
}
