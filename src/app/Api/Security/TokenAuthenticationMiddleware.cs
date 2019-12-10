using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Api.Core;
using Api.Core.Interfaces.DAL;
using Api.Core.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Api.Security
{
    public class TokenAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITokenRepository _tokenRepository;

        public TokenAuthenticationMiddleware(RequestDelegate next, ITokenRepository tokenRepository)
        {
            _next = next;
            _tokenRepository = tokenRepository;
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
                var userId = _tokenRepository.FindOne(t => t.Value == accessToken)?.UserId;
                if (userId.HasValue())
                {
                    // TODO query User and set user details into identity if necessary

                    var principal = new Principal(new GenericIdentity(userId), new string[] { });

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
