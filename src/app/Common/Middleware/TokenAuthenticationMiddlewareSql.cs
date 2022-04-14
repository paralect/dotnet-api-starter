using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Common.DalSql.Filters;
using Common.DalSql.Interfaces;
using Common.Enums;
using Common.Utils;
using Microsoft.AspNetCore.Http;

namespace Common.Middleware;

public class TokenAuthenticationMiddlewareSql
{
    private readonly RequestDelegate _next;

    public TokenAuthenticationMiddlewareSql(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ITokenRepository tokenRepository)
    {
        if (!context.Request.Path.Equals(Constants.HealthcheckPath))
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
                var token = await tokenRepository.FindOneAsync(new TokenFilter
                {
                    Value = accessToken,
                    AsNoTracking = true
                },
                x => new UserTokenModel
                {
                    UserId = x.UserId,
                    UserRole = x.User.Role,
                    ExpireAt = x.ExpireAt
                });

                if (token != null && !token.IsExpired())
                {
                    var principal = new Principal(
                        new GenericIdentity(token.UserId.ToString()),
                        new string[]
                        {
                        Enum.GetName(typeof(UserRole), token.UserRole)
                        }
                    );

                    Thread.CurrentPrincipal = principal;
                    context.User = principal;
                }
            }
        }

        await _next(context);
    }
}

public class UserTokenModel : IExpirable
{
    public long UserId { get; set; }
    public UserRole UserRole { get; set; }
    public DateTime ExpireAt { get; set; }
}