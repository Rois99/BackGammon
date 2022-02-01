using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace Backgammon.Services.Identity.Extensions
{
    public static class ModelStateExtensions
    {
        public static IEnumerable<string> GetAllErrors(this ModelStateDictionary modelState) => modelState.Values.Where(v => v.ValidationState == ModelValidationState.Invalid)
                    .SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();


    }
}
