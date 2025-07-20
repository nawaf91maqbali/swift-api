using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;

namespace SwiftAPI.Helpers
{
    /// <summary>
    /// Helper class for writing results to HTTP responses in a consistent JSON format.
    /// </summary>
    internal static class ResultHelper
    {
        /// <summary>
        /// Writes the result to the HTTP response as JSON, handling both synchronous and asynchronous results.
        /// </summary>
        /// <param name="res"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        internal static async Task WriteAsync(this HttpResponse res, object? result)
        {
            res.ContentType = "application/json";

            if (result is Task resultAsync)
            {
                await resultAsync;

                var exceptionProp = resultAsync.GetType().GetProperty("Exception");
                if (exceptionProp != null)
                {
                    var exception = exceptionProp.GetValue(resultAsync) as Exception;
                    if (exception != null)
                        throw exception.GetBaseException();
                }

                var resultProp = resultAsync.GetType().GetProperty("Result");
                if (resultProp != null)
                {
                    var resultValue = resultProp.GetValue(resultAsync);

                    if (resultValue == null)
                    {
                        res.StatusCode = StatusCodes.Status204NoContent;
                        return;
                    }

                    // Force JSON serialization even for string
                    await res.WriteAsync(JsonSerializer.Serialize(resultValue), Encoding.UTF8);
                }
                else
                {
                    res.StatusCode = StatusCodes.Status204NoContent;
                }
            }
            else
            {
                if (result == null)
                {
                    res.StatusCode = StatusCodes.Status204NoContent;
                }
                else
                {
                    // Force JSON serialization even for string
                    await res.WriteAsync(JsonSerializer.Serialize(result), Encoding.UTF8);
                }
            }
        }
    }
}
