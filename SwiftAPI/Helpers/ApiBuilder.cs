using Microsoft.AspNetCore.Builder;
using SwiftAPI.Shared;
using System.Reflection;

namespace SwiftAPI.Helpers
{
    /// <summary>
    /// Helper class for building API endpoints in a WebApplication.
    /// </summary>
    internal static class ApiBuilder
    {
        /// <summary>
        /// Builds API endpoints for all types marked with EndPointAttribute or ModelEndPointAttribute in the current AppDomain.
        /// </summary>
        /// <param name="app"></param>
        internal static void BuildApi(this WebApplication app)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var endPoints = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => (t.IsInterface || t.IsClass) 
                && (t.GetCustomAttributes<EndPointAttribute>().Any()
                || t.GetCustomAttributes<ModelEndPointAttribute>().Any()));

            foreach (var endPoint in endPoints)
            {
                var apiName = endPoint.GetCustomAttribute<EndPointAttribute>()?.Name ?? endPoint.Name.ToSwiftApiName();
                var @interface = endPoint;
                if(!endPoint.IsInterface)
                    @interface = endPoint.GetCustomAttribute<ModelEndPointAttribute>()?.Interface ?? endPoint;
                var actions = @interface.GetAllMethods();

                foreach (var action in actions)
                {
                    var actionName = action.GetCustomAttribute<ActionAttribute>()?.Name ?? action.Name.ToSwiftApiName();
                    var actionType = action.GetCustomAttribute<ActionAttribute>()?.Action ?? ActionType.Get;

                    var route = $"api/{apiName}/{actionName}";
                    route = route.BuildRoute(action.GetParameters().ToList());

                    switch (actionType)
                    {
                        case ActionType.Post:
                            app.MapPostApi(route, endPoint, action, apiName);
                            break;
                        case ActionType.Put:
                            app.MapPutApi(route, endPoint, action, apiName);
                            break;
                        case ActionType.Delete:
                            app.MapDeleteApi(route, endPoint, action, apiName);
                            break;
                        default:
                            app.MapGetApi(route, endPoint, action, apiName);
                            break;
                    }
                }
            }
        }
    }
}
