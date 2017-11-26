using CQRSLite_Retrosheet.Domain.Requests;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;

namespace CQRSLite_Retrosheet.Web.Filters
{
    public class ValidationActionFilter : IActionFilter
    {
        private ILogger logger;

        public ValidationActionFilter(ILogger<ValidationActionFilter> logger)
        {
            this.logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // do something before the action executes
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);

                string body;
                var req = context.HttpContext.Request;
                req.EnableRewind();
                using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8, true, (int)req.ContentLength, true))
                {
                    body = reader.ReadToEnd();
                }
                req.Body.Position = 0;

                string msg = "";

                foreach (var state in context.ModelState.Values)
                {
                    foreach (var e in state.Errors)
                    {
                        msg = msg + " ~ " + e.ErrorMessage;
                    }
                }

                logger.LogError("Request Body = " + body + " Validation Errors" + msg);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // do something after the action executes
        }
    }
}