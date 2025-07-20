using Microsoft.AspNetCore.Http;
using SwiftAPI.Shared;
using System.Reflection;

namespace SwiftAPI.Helpers
{
    /// <summary>
    /// Helper class for resolving method arguments from HTTP requests.
    /// </summary>
    internal static class ArgsHelper
    {
        /// <summary>
        /// Resolves method parameters from the HTTP request's query string, route values, or body.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        internal static async Task<object[]> ResolveArgsAsync(this HttpRequest req, MethodInfo method)
        {
            var parameters = method.GetParameters();
            var args = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                var paramType = param.GetCustomAttribute<ParamAttribute>()?.Type ?? ParamType.FromQuery;

                args[i] = paramType switch
                {
                    ParamType.FromBody => await param.ResolveFromBodyAsync(req),
                    ParamType.FromRoute => param.ResolveFromQueryOrRoute(req),
                    _ => param.ResolveFromQueryOrRoute(req)
                };
            }

            return args;
        }
    }
}
