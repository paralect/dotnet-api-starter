using Api.Security;

namespace Api.Controllers
{
    [Authorize]
    public abstract class BaseAuthorizedController : BaseController
    {
        protected string CurrentUserId => User.Identity.Name;
    }
}
