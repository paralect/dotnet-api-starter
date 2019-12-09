using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Api.Core.Settings
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }

        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            byte[] buff = Encoding.UTF8.GetBytes(Secret);
            //if (buff.Length < 128)
            //{
            //    Array.Resize(ref buff, 128);
            //}
            return new SymmetricSecurityKey(buff);
        }
    }
}
