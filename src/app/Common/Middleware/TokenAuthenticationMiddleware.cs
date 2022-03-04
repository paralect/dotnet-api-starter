using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Common.DAL.Interfaces;
using Common.DAL.Repositories;
using Common.Enums;
using Common.Utils;
using Microsoft.AspNetCore.Http;

namespace Common.Middleware
{
    public class TokenAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ITokenRepository tokenRepository)
        {
            var accessToken = context.Request.Cookies[Constants.CookieNames.AccessToken];
            if (accessToken.HasNoValue())
            {
                var authorization = context.Request.Headers["Authorization"].ToString();
                if (authorization.HasValue())
                {
                    accessToken = authorization.Replace("Bearer", "").Trim();
                }
            }

            if (accessToken.HasValue())
            {
                var userToken = await tokenRepository.FindOneAsync(new TokenFilter
                {
                    Value = accessToken
                });

                if (userToken != null && !userToken.IsExpired())
                {
                    var principal = new Principal(new GenericIdentity(userToken.UserId), new string[] { Enum.GetName(typeof(UserRole), userToken.UserRole) });

                    Thread.CurrentPrincipal = principal;
                    context.User = principal;
                }
            }

            await _next(context);
        }
    }
}
