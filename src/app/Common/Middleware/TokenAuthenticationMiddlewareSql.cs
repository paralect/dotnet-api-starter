using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Common.DalSql.Entities;
using Common.DalSql.Filters;
using Common.Enums;
using Common.ServicesSql.Domain.Interfaces;
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

    public async Task Invoke(HttpContext context, ITokenService tokenService)
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
            var token = await tokenService.FindOneAsync(new TokenFilter
            {
                Value = accessToken,
                AsNoTracking = true,
                IncludeProperties = new List<Expression<Func<Token, object>>>
                {
                    x => x.User
                }
            });

            if (token != null && !token.IsExpired())
            {
                var principal = new Principal(
                    new GenericIdentity(token.UserId.ToString()),
                    new string[]
                    {
                        Enum.GetName(typeof(UserRole), token.User.Role)
                    }
                );

                Thread.CurrentPrincipal = principal;
                context.User = principal;
            }
        }

        await _next(context);
    }
}