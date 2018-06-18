using Api.Core.Abstract;
using Api.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly JwtSettings _jwtSettings;
        public AuthService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string CreateAuthToken(ObjectId id)
        {
            var claims = new List<Claim>
            {
                new Claim("id", id.ToString()),
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token");

            JwtSecurityToken jwt = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                signingCredentials: new SigningCredentials(_jwtSettings.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            string encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
    }
}
