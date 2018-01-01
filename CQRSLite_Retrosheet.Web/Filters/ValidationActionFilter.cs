using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CQRSLite_Retrosheet.Web.Filters
{
    public class ValidationActionFilter : IActionFilter
    {
        private ILogger logger;

        public ValidationActionFilter(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger("ValidationActionFilter");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // do something before the action executes
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);

                string body = ((ControllerActionDescriptor)(context.ActionDescriptor)).ActionName;
                foreach (var arg in context.ActionArguments)
                {
                    body = body + JsonConvert.SerializeObject(arg.Value);
                }

                string msg = "";

                foreach (var state in context.ModelState.Values)
                {
                    foreach (var e in state.Errors)
                    {
                        msg = msg + " ~ " + e.ErrorMessage;
                    }
                }

                logger.LogError("Request = " + body + " Validation Errors" + msg);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // do something after the action executes
        }
    }
}