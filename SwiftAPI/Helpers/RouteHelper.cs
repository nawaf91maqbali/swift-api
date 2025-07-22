using SwiftAPI.Core;
using SwiftAPI.Shared;
using System.Reflection;

namespace SwiftAPI.Helpers
{
    /// <summary>
    /// Helper class for building route strings based on method parameters.
    /// </summary>
    internal static class RouteHelper
    {
        /// <summary>
        /// Builds a route string by appending route parameters from method parameters.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal static string BuildRoute(this string route, List<ParameterInfo> parameters)
        {
            foreach (var p in parameters)
            {
                var fromRouteAttr = p.GetCustomAttribute<ParamAttribute>();
                if (fromRouteAttr != null)
                {
                    if (fromRouteAttr.Type == ParamType.FromRoute)
                        route += $"/{{{p.Name}}}";
                }
            }
            return route;
        }
    }
}
