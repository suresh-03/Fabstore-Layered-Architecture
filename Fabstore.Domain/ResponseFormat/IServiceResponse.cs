namespace Fabstore.Domain.ResponseFormat;

public interface IServiceResponse
    {
    bool Success { get; set; }
    string Message { get; set; }
    ActionType ActionType { get; set; }
    }

public interface IServiceResponse<T> : IServiceResponse
    {
    T? Data { get; set; }
    }


public enum ActionType
    {
    None,               // No action performed or default state (No specific HTTP status code (200))
    Created,            // 201 Created - Resource successfully created
    Retrieved,          // 200 OK - Successful data retrieval
    Updated,            // 200 OK or 204 No Content - Successful update (204 if no content returned)
    Deleted,            // 200 OK or 204 No Content - Successful deletion (204 if no content returned)
    Failed,             // 400 Bad Request or 500 Internal Server Error - Generic failure
    NotFound,           // 404 Not Found - Resource not found
    Unauthorized,       // 401 Unauthorized - Authentication required or failed
    Forbidden,          // 403 Forbidden - Authenticated but not allowed
    Conflict,           // 409 Conflict - Data conflict or duplicate entry
    ValidationError,    // 400 Bad Request - Validation failure
    ServerError         // 500 Internal Server Error - Unexpected server-side error
    }










