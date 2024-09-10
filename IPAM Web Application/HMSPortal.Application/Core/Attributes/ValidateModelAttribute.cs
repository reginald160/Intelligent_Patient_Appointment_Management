using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSPortal.Application.Core.Attributes
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var controller = (Controller)context.Controller;
                var viewModel = context.ActionArguments.Values.FirstOrDefault();

                // Collect error messages
                List<string> errorMessages = new List<string>();
                foreach (var error in context.ModelState)
                {
                    var errors = error.Value.Errors.Select(x => x.ErrorMessage).ToList();
                    errorMessages.AddRange(errors);
                }

                // Combine all error messages into a single message
                string allErrorMessages = string.Join("; ", errorMessages);

                // Add the combined error message to ModelState
                context.ModelState.AddModelError("error-V", allErrorMessages);

                // Return the view with the model state errors and viewModel
                context.Result = controller.View(viewModel);
            }
        }
    }
}


