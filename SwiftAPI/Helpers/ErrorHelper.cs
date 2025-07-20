using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SwiftAPI.Helpers
{
    /// <summary>
    /// Helper class for handling errors in API endpoints.
    /// </summary>
    internal static class ErrorHelper
    {
        /// <summary>
        /// Handles exceptions and returns appropriate HTTP responses with ProblemDetails.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        internal static ProblemDetails HandleError(this Exception exception, HttpResponse res)
        {
            switch (exception)
            {
                case ArgumentNullException:
                case ArgumentException:
                case FormatException:
                case OverflowException:
                    res.StatusCode = 400;
                    return new ProblemDetails
                    {
                        Status = 400,
                        Title = "Invalid input",
                        Detail = exception.Message,
                        Type = "https://httpstatuses.org/400"
                    };

                case UnauthorizedAccessException:
                    res.StatusCode = 401;
                    return new ProblemDetails
                    {
                        Status = 401,
                        Title = "Unauthorized",
                        Detail = exception.Message,
                        Type = "https://httpstatuses.org/401"
                    };

                case KeyNotFoundException:
                case FileNotFoundException:
                case DirectoryNotFoundException:
                case InvalidDataException:
                    res.StatusCode = 404;
                    return new ProblemDetails
                    {
                        Status = 404,
                        Title = "Resource not found",
                        Detail = exception.Message,
                        Type = "https://httpstatuses.org/404"
                    };

                case NotSupportedException:
                case NotImplementedException:
                    res.StatusCode = 501;
                    return new ProblemDetails
                    {
                        Status = 501,
                        Title = "Feature not implemented",
                        Detail = exception.Message,
                        Type = "https://httpstatuses.org/501"
                    };

                case InvalidOperationException:
                    res.StatusCode = 409;
                    return new ProblemDetails
                    {
                        Status = 409,
                        Title = "Operation conflict",
                        Detail = exception.Message,
                        Type = "https://httpstatuses.org/409"
                    };

                case TimeoutException:
                    res.StatusCode = 408;
                    return new ProblemDetails
                    {
                        Status = 408,
                        Title = "Request timed out",
                        Detail = exception.Message,
                        Type = "https://httpstatuses.org/408"
                    };

                case OperationCanceledException:
                    res.StatusCode = 499; // Client Closed Request
                    return new ProblemDetails
                    {
                        Status = 499,
                        Title = "Operation cancelled",
                        Detail = exception.Message,
                        Type = "https://httpstatuses.org/499"
                    };

                case ValidationException:
                    res.StatusCode = 422;
                    return new ProblemDetails
                    {
                        Status = 422,
                        Title = "Validation failed",
                        Detail = exception.Message,
                        Type = "https://httpstatuses.org/422"
                    };

                default:
                    res.StatusCode = 500;
                    return new ProblemDetails
                    {
                        Status = 500,
                        Title = "An unexpected error occurred",
                        Detail = exception.Message,
                        Type = "https://httpstatuses.org/500"
                    };
            }
        }
    }
}
