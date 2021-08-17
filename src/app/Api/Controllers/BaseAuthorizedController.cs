using System;
using Api.Security;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    public abstract class BaseAuthorizedController : BaseController
    {
        protected string GetCurrentUserId()
        {
            return User.Identity.Name;
        }

        protected BadRequestResult BadRequest(string field, string errorMessage)
        {
            ModelState.AddModelError(field, errorMessage);
            return BadRequest();
        }
    }
}
