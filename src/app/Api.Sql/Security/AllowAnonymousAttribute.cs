using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Sql.Security
{
    public class AllowAnonymousAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
        }
    }
}
