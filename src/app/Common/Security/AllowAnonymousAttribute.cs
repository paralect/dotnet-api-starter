using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Common.Security;

public class AllowAnonymousAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
    }
}
