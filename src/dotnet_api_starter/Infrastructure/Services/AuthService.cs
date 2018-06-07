using dotnet_api_starter.Infrastructure.Abstract;
using dotnet_api_starter.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dotnet_api_starter.Infrastructure.Services
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
                new Claim("_id", id.ToString()),
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token");

            JwtSecurityToken jwt = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                signingCredentials: new SigningCredentials(_jwtSettings.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            //jwt.Payload["issuer"] = _jwtSettings.Issuer;
            //jwt.Payload["audience"] = _jwtSettings.Audience;

            string encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
    }
}
