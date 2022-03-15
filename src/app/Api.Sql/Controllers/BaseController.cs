using Common.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected long? CurrentUserId
        {
            get
            {
                var currentUserId = User?.Identity?.Name;
                return currentUserId.HasValue()
                    ? long.Parse(currentUserId)
                    : (long?)null;
            }
        }

        protected BadRequestResult BadRequest(string field, string errorMessage)
        {
            ModelState.AddModelError(field, errorMessage);
            return BadRequest();
        }
    }
}