using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Common.Services.Interfaces;
using Common.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Common.Middleware
{
    public class TokenAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITokenService _tokenService;

        public TokenAuthenticationMiddleware(RequestDelegate next, ITokenService tokenService)
        {
            _next = next;
            _tokenService = tokenService;
        }

        public async Task Invoke(HttpContext context)
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
                var token = await _tokenService.FindAsync(accessToken);
                if (token != null && !token.IsExpired())
                {
                    var principal = new Principal(new GenericIdentity(token.UserId), new string[] { });

                    Thread.CurrentPrincipal = principal;
                    context.User = principal;
                }
            }

            await _next(context);
        }
    }

    public static class TokenAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenAuthenticationMiddleware>();
        }
    }
}
