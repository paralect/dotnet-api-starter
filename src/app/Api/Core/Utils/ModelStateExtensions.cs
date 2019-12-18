﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace Api.Core.Utils
{
    public static class ModelStateExtensions
    {
        public static object GetErrors(this ModelStateDictionary modelState)
        {
            var errors = modelState.ToDictionary(x => x.Key, y => y.Value.Errors.FirstOrDefault()?.ErrorMessage);

            return new { errors };
        }
    }
}
