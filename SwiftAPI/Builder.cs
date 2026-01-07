using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SwiftAPI.AuthHandler;
using SwiftAPI.Core;
using SwiftAPI.Core.AuthHandlerOptions;
using SwiftAPI.Helpers;
using SwiftAPI.Shared;
using System.Text;

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
        public static void AddSwiftAPI(this IServiceCollection services, Action<SwiftApiOptions> configureOptions)
        {
            var config = new SwiftApiOptions();
            configureOptions?.Invoke(config);

            services.ServiceRegistration();
            services.AddEndpointsApiExplorer();
            services.AddOutputCache();



            services.AddSwaggerGen(options =>
            {
                options.DocumentFilter<SchemaModelRegistrationFilter>();

                options.AddAuthSchema(config);
                options.OperationFilter<AuthOperationFilter>(config);
                options.OperationFilter<SchemaOprationFilter>();

            });

            services.AddSwiftApiAuth(config);
        }
        /// <summary>
        /// Maps all SwiftAPI endpoints to the WebApplication pipeline based on defined interfaces and actions.
        /// </summary>
        /// <param name="app"></param>
        public static void MapSwiftAPI(this WebApplication app, bool enableAuth = false)
        {
            app.UseOutputCache();
            app.BuildApi();

            if (enableAuth)
            {
                app.UseAuthentication();
                app.UseAuthorization();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
        }
    }
}
