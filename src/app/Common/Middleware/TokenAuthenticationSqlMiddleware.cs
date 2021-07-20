using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Common.DALSql;
using Common.DALSql.Data;
using Common.Utils;
using Microsoft.AspNetCore.Http;

namespace Common.Middleware
{
    public class TokenAuthenticationSqlMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenAuthenticationSqlMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUnitOfWork unitOfWork)
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
                var token = await unitOfWork.Tokens.FindOneByFilterAsNoTracking(new TokenFilter
                {
                    Value = accessToken
                });
                if (token != null && !token.IsExpired())
                {
                    var principal = new Principal(new GenericIdentity(token.UserId.ToString()), new string[] { });

                    Thread.CurrentPrincipal = principal;
                    context.User = principal;
                }
            }

            await _next(context);
        }
    }
}
