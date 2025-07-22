using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SwiftAPI.Helpers
{
    /// <summary>
    /// Helper class for writing results to HTTP responses in a consistent JSON format.
    /// </summary>
    internal static class ResultHelper
    {
        /// <summary>
        /// Writes the result of an action to the HTTP response.
        /// </summary>
        /// <param name="res"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        internal static async Task WriteAsync(this HttpResponse res, object? result)
        {
            if (result is Task resultTask)
            {
                await resultTask;

                // Check for exceptions
                var exceptionProp = resultTask.GetType().GetProperty("Exception");
                if (exceptionProp?.GetValue(resultTask) is Exception exception)
                    throw exception.GetBaseException();

                // Extract the value from Task<T>
                var resultProp = resultTask.GetType().GetProperty("Result");
                var value = resultProp?.GetValue(resultTask);
                await WriteResultAsync(res, value);
            }
            else
            {
                await WriteResultAsync(res, result);
            }
        }

        private static async Task WriteResultAsync(HttpResponse res, object? value)
        {
            if (value == null || value.GetType().FullName == "System.Threading.Tasks.VoidTaskResult")
            {
                res.StatusCode = StatusCodes.Status204NoContent;
                return;
            }

            // Built-in support for Minimal API Results (like Results.File, Results.Json, etc.)
            if (value is IResult iResult)
            {
                await iResult.ExecuteAsync(res.HttpContext);
                return;
            }

            // Handle file path (string)
            if (value is string filePath && File.Exists(filePath))
            {
                res.ContentType = "application/octet-stream";
                res.Headers.ContentDisposition = $"attachment; filename=\"{Path.GetFileName(filePath)}\"";
                await res.SendFileAsync(filePath);
                return;
            }

            // Handle byte[]
            if (value is byte[] bytes)
            {
                res.ContentType = "application/octet-stream";
                res.Headers.ContentDisposition = "attachment; filename=\"file\"";
                await res.Body.WriteAsync(bytes, 0, bytes.Length);
                return;
            }

            // Handle Stream
            if (value is Stream stream)
            {
                res.ContentType = "application/octet-stream";
                res.Headers.ContentDisposition = "attachment; filename=\"file\"";
                await stream.CopyToAsync(res.Body);
                return;
            }

            // Handle plain string as text
            if (value is string str)
            {
                res.ContentType = "text/plain";
                await res.WriteAsync(str, Encoding.UTF8);
                return;
            }

            // Default to JSON
            res.ContentType = "application/json";
            var json = JsonSerializer.Serialize(value, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
            await res.WriteAsync(json, Encoding.UTF8);
        }
    }
}
