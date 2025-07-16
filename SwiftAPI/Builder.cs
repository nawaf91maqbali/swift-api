using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SwiftAPI.Helpers;
using SwiftAPI.Shared;
using System.Reflection;

namespace SwiftAPI
{
    /// <summary>
    /// Builder class for setting up SwiftAPI endpoints and services.
    /// </summary>
    public static class Builder
    {
        /// <summary>
        /// Registers all necessary services for SwiftAPI in the dependency injection container.
        /// </summary>
        /// <param name="services"></param>
        public static void AddSwiftAPI(this IServiceCollection services, Action<SwiftApiOptions>? configureOptions = null)
        {
            services.ServiceRegistration();
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.DocumentFilter<SchemaModelRegistrationFilter>();

                var config = new SwiftApiOptions();
                configureOptions?.Invoke(config);

                options.AddAuthSchema(config);
                options.OperationFilter<AuthOperationFilter>(config);

            });
        }
        /// <summary>
        /// Maps all SwiftAPI endpoints to the WebApplication pipeline based on defined interfaces and actions.
        /// </summary>
        /// <param name="app"></param>
        public static void MapSwiftAPI(this WebApplication app)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var endPoints = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => t.IsInterface && t.GetCustomAttributes<EndPointAttribute>().Any());

            foreach (var endPoint in endPoints)
            {
                var apiName = endPoint.GetCustomAttribute<EndPointAttribute>()?.Name ?? endPoint.Name.ToSwiftApiName();
                var actions = endPoint.GetMethods();

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
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
        }
    }
}
