using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Core.Abstract
{
    public abstract class BaseController : Controller
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
