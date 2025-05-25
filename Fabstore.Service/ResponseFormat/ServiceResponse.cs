using Fabstore.Domain.ResponseFormat;

namespace Fabstore.Service.ResponseFormat;

//public class ServiceResponse : IServiceResponse
//    {
//    public bool Success { get; set; } = true;
//    public string Message { get; set; } = string.Empty;
//    public ActionType ActionType { get; set; } = ActionType.None;

//    public ServiceResponse(bool success, string message, ActionType actionType)
//        {
//        Success = success;
//        Message = message;
//        ActionType = actionType;

//        }
//    }

//public class ServiceResponse<T> : IServiceResponse<T>
//    {

//    public bool Success { get; set; } = true;
//    public string Message { get; set; } = string.Empty;
//    public ActionType ActionType { get; set; } = ActionType.None;
//    public T? Data { get; set; }

//    public ServiceResponse(bool success, string message, ActionType actionType, T data)
//        {
//        Success = success;
//        Message = message;
//        ActionType = actionType;
//        Data = data;
//        }
//    }

public class ServiceResponse : IServiceResponse
    {
    public bool Success { get; set; }
    public string Message { get; set; }
    public ActionType ActionType { get; set; }

    public ServiceResponse(bool success, string message, ActionType actionType)
        {
        Success = success;
        Message = message;
        ActionType = actionType;
        }
    }

public class ServiceResponse<T> : ServiceResponse, IServiceResponse<T>
    {
    public T? Data { get; set; }

    public ServiceResponse(bool success, string message, ActionType actionType, T? data = default)
        : base(success, message, actionType)
        {
        Data = data;
        }
    }



