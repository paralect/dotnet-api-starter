using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.Services.Interfaces.Infrastructure;
using Common;
using Common.DALSql;
using Common.DALSql.Entities;
using Common.DALSql.Repositories;
using Common.Enums;
using Common.Services.Interfaces;
using Common.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Api.Core.Services.Infrastructure
{
    public class AuthSqlService : IAuthSqlService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenSqlService _tokenSqlService;
        private readonly AppSettings _appSettings;
        private readonly HttpContext _httpContext;

        public AuthSqlService(
            IUnitOfWork unitOfWork,
            ITokenSqlService tokenSqlService,
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _tokenSqlService = tokenSqlService;
            _appSettings = appSettings.Value;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task SetTokensAsync(long userId)
        {
            var tokens = (await _unitOfWork.Tokens.FindByQueryAsNoTracking(
                new DbQuery<Token>().AddFilter(t => t.UserId == userId)
            )).ToList();
            
            SetTokenCookies(tokens);
        }

        public void SetTokens(IEnumerable<Token> tokens)
        {
            SetTokenCookies(tokens.ToList());
        }

        public async Task UnsetTokensAsync(long userId)
        {
            await _tokenSqlService.DeleteUserTokensAsync(userId);

            _httpContext.Response.Cookies.Delete(Constants.CookieNames.AccessToken);
            _httpContext.Response.Cookies.Delete(Constants.CookieNames.RefreshToken);
        }

        private void SetTokenCookies(ICollection<Token> tokens)
        {
            var accessToken = tokens.Single(t => t.Type == TokenTypeEnum.Access);
            var refreshToken = tokens.Single(t => t.Type == TokenTypeEnum.Refresh);

            var domain = new Uri(_appSettings.WebUrl).Host;

            _httpContext.Response.Cookies.Append(Constants.CookieNames.AccessToken, accessToken.Value, new CookieOptions
            {
                HttpOnly = false,
                Expires = accessToken.ExpireAt,
                Domain = domain
            });

            _httpContext.Response.Cookies.Append(Constants.CookieNames.RefreshToken, refreshToken.Value, new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.ExpireAt,
                Domain = domain
            });
        }
    }
}