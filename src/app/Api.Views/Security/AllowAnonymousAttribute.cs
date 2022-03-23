using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Views.Security;

public class AllowAnonymousAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
    }
}
