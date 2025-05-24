using Fabstore.Domain.ResponseFormat;
using Fabstore.WebApplication.Constants;
using Microsoft.AspNetCore.Mvc;


namespace Fabstore.WebApplication.Filters;

public static class ResponseFilter
    {
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

    public static JsonResult HandleResponse(IServiceResponse response)
        {
        ResponseModel responseModel = new ResponseModel()
            {
            Message = response.Message,
            Success = response.Success,
            };

        return HandleResponseInternal(responseModel, response.ActionType);
        }

    private static JsonResult HandleResponseInternal(ResponseModel response, ActionType actionType)
        {
        if (response == null)
            {
            return new JsonResult(response) { StatusCode = HttpStatusCode.BAD_REQUEST };
            }

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



public class ResponseModel
    {
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? RedirectUrl { get; set; }
    }

public class ResponseModel<T> : ResponseModel
    {
    public T? Data { get; set; }
    }


