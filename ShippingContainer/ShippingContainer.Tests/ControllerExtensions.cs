using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ShippingContainer.Tests
{
    /// <summary>
    /// Extension methods for controller testing
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// Trigger model validation (normally handled by MVC)
        /// </summary>
        public static void ValidateViewModel(this Controller controller, object viewModelToValidate)
        {
            controller?.ModelState?.Clear();

            var validationContext = new ValidationContext(viewModelToValidate, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(viewModelToValidate, validationContext, validationResults, true);

            foreach (var validationResult in validationResults)
            {
                controller.ModelState.AddModelError(validationResult.MemberNames.FirstOrDefault() ?? string.Empty, validationResult.ErrorMessage);
            }
        }
    }
}
