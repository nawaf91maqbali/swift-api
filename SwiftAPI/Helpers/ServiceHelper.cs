using Microsoft.Extensions.DependencyInjection;
using SwiftAPI.Shared;
using System.Reflection;

namespace SwiftAPI.Helpers
{
    /// <summary>
    /// Helper class for service registration and retrieval in the dependency injection container.
    /// </summary>
    static class ServiceHelper
    {
        /// <summary>
        /// Registers all services that implement interfaces with the EndPointAttribute in the dependency injection container.
        /// </summary>
        /// <param name="services"></param>
        public static void ServiceRegistration(this IServiceCollection services)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var interfaceTypes = assemblies
                .SelectMany(a => a.GetTypes()).Where(t => t.IsInterface 
                && t.GetCustomAttribute<EndPointAttribute>() != null).ToList();

            foreach (var interfaceType in interfaceTypes)
            {
                var implementations = assemblies
                    .SelectMany(a => a.GetTypes())
                    .Where(t =>t.IsClass && !t.IsAbstract 
                    && interfaceType.IsAssignableFrom(t) && t != interfaceType).ToList();

                foreach (var implementation in implementations)
                {
                    if (interfaceType.IsAssignableFrom(implementation))
                    {
                        services.AddScoped(interfaceType, implementation);
                    }
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
        public static object GetServiceFromSafeScope(this IServiceProvider rootProvider, Type serviceType, out IServiceScope? scope)
        {
            try
            {
                scope = rootProvider.CreateScope();
                var service = scope.ServiceProvider.GetService(serviceType);

                return service ?? throw new Exception($"Service of type {serviceType.Name} is not registered");
            }
            catch
            {
                throw new Exception($"Service of type {serviceType.Name} is not registred");
            }
        }
    }
}
