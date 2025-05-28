using Fabstore.Domain.ResponseFormat;
using Fabstore.WebApplication.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Fabstore.WebApplication.Filters;

// Provides utility methods for formatting API responses in a consistent way
public static class ResponseFilter
    {
    // Handles a generic service response and returns a JsonResult with appropriate status code
    public static JsonResult HandleResponse<T>(IServiceResponse<T> response)
        {
        ResponseModel<T> responseModel = new ResponseModel<T>()
            {
            Message = response.Message,
            Data = response.Data,
            Success = response.Success,
            };

        return HandleResponseInternal(responseModel, response.ActionType);
        }

    // Handles a non-generic service response and returns a JsonResult with appropriate status code
    public static JsonResult HandleResponse(IServiceResponse response)
        {
        ResponseModel responseModel = new ResponseModel()
            {
            Message = response.Message,
            Success = response.Success,
            };

        return HandleResponseInternal(responseModel, response.ActionType);
        }

    // Internal method to map ActionType to HTTP status code and return a JsonResult
    private static JsonResult HandleResponseInternal(ResponseModel response, ActionType actionType)
        {
        if (response == null)
            {
            return new JsonResult(response) { StatusCode = HttpStatusCode.BAD_REQUEST };
            }

        // Map ActionType to HTTP status code
        switch (actionType)
            {
            case ActionType.Created:
                return new JsonResult(response) { StatusCode = HttpStatusCode.CREATED };
            case ActionType.Updated:
            case ActionType.None:
            case ActionType.Deleted:
            case ActionType.Retrieved:
                return new JsonResult(response) { StatusCode = HttpStatusCode.OK };
            case ActionType.NotFound:
                return new JsonResult(response) { StatusCode = HttpStatusCode.NOT_FOUND };
            case ActionType.Unauthorized:
                return new JsonResult(response) { StatusCode = HttpStatusCode.UNAUTHORIZED };
            case ActionType.Forbidden:
                return new JsonResult(response) { StatusCode = HttpStatusCode.FORBIDDEN };
            case ActionType.Conflict:
                return new JsonResult(response) { StatusCode = HttpStatusCode.CONFLICT };
            case ActionType.ValidationError:
                return new JsonResult(response) { StatusCode = HttpStatusCode.BAD_REQUEST };
            case ActionType.Failed:
            case ActionType.ServerError:
                return new JsonResult(response) { StatusCode = HttpStatusCode.INTERNAL_SERVER_ERROR };
            default:
                return new JsonResult(response) { StatusCode = HttpStatusCode.OK };
            }
        }

    // Handles a custom response with explicit status code and optional redirect URL
    public static JsonResult HandleResponse(bool success, string message, int statusCode, string? redirectUrl = null)
        {
        return new JsonResult(new ResponseModel()
            {
            Message = message,
            Success = success,
            RedirectUrl = redirectUrl
            })
            { StatusCode = statusCode };
        }
    }

// Basic response model for API responses
public class ResponseModel
    {
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? RedirectUrl { get; set; }
    }

// Generic response model that includes data payload
public class ResponseModel<T> : ResponseModel
    {
    public T? Data { get; set; }
    }