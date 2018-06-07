using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_api_starter.Infrastructure.Abstract
{
    public abstract class BaseController : Controller
    {
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
