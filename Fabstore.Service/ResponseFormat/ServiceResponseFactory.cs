using Fabstore.Domain.ResponseFormat;

namespace Fabstore.Service.ResponseFormat;

public class ServiceResponseFactory : IServiceResponseFactory
    {
    public IServiceResponse CreateResponse(bool success, string message, ActionType actionType)
        {
        return new ServiceResponse(success, message, actionType);
        }

    public IServiceResponse<T> CreateResponse<T>(bool success, string message, ActionType actionType, T? data = default)
        {
        return new ServiceResponse<T>(success, message, actionType, data);
        }
    }

