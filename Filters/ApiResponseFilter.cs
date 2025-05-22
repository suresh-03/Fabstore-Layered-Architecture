using FabstoreWebApplication.ViewModels.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FabstoreWebApplication.Filters
    {
    public class ApiResponseFilter : IActionFilter
        {
        public void OnActionExecuted(ActionExecutedContext context)
            {
            if (context.Result is ObjectResult objectResult && objectResult.Value is ApiResponse apiResponse)
                {
                context.HttpContext.Response.StatusCode = apiResponse.StatusCode;

                context.Result = new JsonResult(apiResponse);
                }
            }

        public void OnActionExecuting(ActionExecutingContext context)
            {
            // Nothing to Execute Now
            }
        }
    }
