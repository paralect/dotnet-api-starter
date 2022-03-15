using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using Common.Utils;

namespace Api.Utils
{
    public static class ModelStateExtensions
    {
        public static object GetErrors(this ModelStateDictionary modelState)
        {
            var errors = modelState.ToDictionary(x => x.Key.ToCamelCase(), y => y.Value.Errors.Select(x => x.ErrorMessage));

            return new { errors };
        }
    }
}
