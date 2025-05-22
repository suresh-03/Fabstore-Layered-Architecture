using FabstoreWebApplication.ViewModels.Auth;
using Microsoft.AspNetCore.Mvc;

namespace FabstoreWebApplication.Helpers
    {
    public sealed class ObjectResultHelper
        {
        public static ObjectResult CreateObjectResult(string status, string message, int statusCode, string redirectUrl = "")
            {
            return new ObjectResult(new ApiResponse
                {
                Status = status,
                Message = message,
                StatusCode = statusCode,
                RedirectUrl = redirectUrl
                });
            }
        }
    }
