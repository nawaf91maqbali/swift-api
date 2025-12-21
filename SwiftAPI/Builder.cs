using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SwiftAPI.Helpers;
using SwiftAPI.Shared;

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
            services.AddOutputCache();



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
            app.UseOutputCache();
            app.BuildApi();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
        }
    }
}
