using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;

namespace Api.Core.Abstract
{
    [ApiController]
    [Route("[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected ObjectId CurrentUserId
        {
            get
            {
                ObjectId userId = ObjectId.Empty;
                ObjectId.TryParse(User.Claims.FirstOrDefault(x => x.Type == "id")?.Value, out userId);
                return userId;
            }
        }

        protected virtual object GetErrorsFromModelState(ModelStateDictionary modelState)
        {
            Dictionary<string, string> errors = modelState.ToDictionary(x => x.Key, y => y.Value.Errors.FirstOrDefault()?.ErrorMessage);

            return new { errors };
        }

        protected virtual object GetErrorsModel(params object[] errors)
        {
            return new { errors };
        }
    }
}
