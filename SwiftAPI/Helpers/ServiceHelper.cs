using Microsoft.Extensions.DependencyInjection;
using SwiftAPI.Core;
using SwiftAPI.Shared;
using System.Reflection;

namespace SwiftAPI.Helpers
{
    /// <summary>
    /// Helper class for service registration and retrieval in the dependency injection container.
    /// </summary>
    internal static class ServiceHelper
    {
        /// <summary>
        /// Registers all services that implement interfaces with the EndPointAttribute in the dependency injection container.
        /// </summary>
        /// <param name="services"></param>
        internal static void ServiceRegistration(this IServiceCollection services)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var endPoints = assemblies
                .SelectMany(a => a.GetTypes()).Where(t => (t.IsInterface
                || t.IsClass) && (t.GetCustomAttribute<EndPointAttribute>() != null
                || t.GetCustomAttribute<ModelEndPointAttribute>() != null)).ToList();

            foreach(var endPoint in endPoints)
            {
                if (!endPoint.IsInterface)
                {
                    var modelAttr = endPoint.GetCustomAttribute<ModelEndPointAttribute>();

                    var @interface = modelAttr?.Interface;
                    var @implementation = modelAttr?.Implementation;

                    if (@interface == null || implementation == null)
                        continue;

                    services.AddScoped(@interface, @implementation);
                }
                else
                {

                    var @interface = endPoint;
                    var @implementation = assemblies
                        .SelectMany(a => a.GetTypes())
                        .FirstOrDefault(t => t.IsClass && !t.IsAbstract
                        && @interface.IsAssignableFrom(t) && t != @interface);

                    if (@interface == null || implementation == null)
                        continue;

                    services.AddScoped(@interface, @implementation);
                }
            }
        }

        /// <summary>
        /// Retrieves a service from the root service provider within a new scope.
        /// </summary>
        /// <param name="rootProvider"></param>
        /// <param name="serviceType"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal static object GetServiceFromSafeScope(this IServiceProvider rootProvider, Type serviceType, out IServiceScope? scope)
        {
            try
            {
                scope = rootProvider.CreateScope();
                object? service = null;
                if (!serviceType.IsInterface)
                {
                    var serviceTypeAttr = serviceType.GetCustomAttribute<ModelEndPointAttribute>();
                    if (serviceTypeAttr == null)
                        throw new Exception($"ModelEndPointAttribute is not defined on {serviceType.Name} class");
                    var @interface = serviceTypeAttr.Interface;
                    if (@interface == null)
                        throw new Exception($"ModelEndPointAttribute on {serviceType.Name} does not specify an interface");

                    service = scope.ServiceProvider.GetService(@interface);
                }
                else
                {
                    service = scope.ServiceProvider.GetService(serviceType);
                }
                return service ?? throw new Exception($"Service of type {serviceType.Name} is not registered");
            }
            catch
            {
                throw new Exception($"Service of type {serviceType.Name} is not registred");
            }
        }
    }
}
