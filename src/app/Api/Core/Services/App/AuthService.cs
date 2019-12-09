using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Core.Interfaces.Services.App;
using Api.Core.Interfaces.Services.Infrastructure;
using Api.Core.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api.Core.Services.App
{
    public class AuthService : IAuthService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ITokenService _tokenService;

        public AuthService(IOptions<JwtSettings> jwtSettings, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _jwtSettings = jwtSettings.Value;
        }

        public string CreateAuthToken(string id)
        {
            var claims = new List<Claim>
            {
                new Claim("id", id),
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

        public async Task SetTokens(string userId)
        {
            var tokens = await _tokenService.CreateAuthTokens(userId);
        }
    }
}
