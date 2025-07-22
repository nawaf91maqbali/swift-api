using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using SwiftAPI.Core;
using SwiftAPI.Shared;
using System.Reflection;
using System.Security.Claims;

namespace SwiftAPI.Helpers
{
    /// <summary>
    /// Helper class for validating user authorization for API actions.
    /// </summary>
    internal static class AuthHelper
    {
        /// <summary>
        /// Validates if the user is authorized to access the specified action in the endpoint.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="endPoint"></param>
        /// <param name="user"></param>
        /// <exception cref="UnauthorizedAccessException"></exception>
        internal static Exception? ValidateAuthorization(this MethodInfo action, Type endPoint, ClaimsPrincipal user)
        {
            if (action.EnableAuthorization(endPoint))
            {
                var secureEndpoint = endPoint.GetCustomAttribute<SecureEndpointAttribute>();
                var secureAction = action.GetCustomAttribute<SecureActionAttribute>();

                if (action.EnableAuthorization(endPoint) && (user?.Identity == null || !user.Identity.IsAuthenticated))
                    return new UnauthorizedAccessException("User is not authenticated.");

                if (secureEndpoint != null)
                {
                    if (secureEndpoint.Role != null)
                    {
                        var passRole = false;
                        foreach (var role in secureEndpoint.Role.Split(",") ?? Enumerable.Empty<string>())
                        {
                            if (user.IsInRole(role))
                            {
                                passRole = true;
                                break;
                            }

                        }
                        if (!passRole)
                            return new UnauthorizedAccessException($"User does not have the required role: {secureEndpoint.Role}.");
                    }
                }

                if (secureAction != null)
                {
                    if (secureAction.Policy != null)
                    {
                        var passPolicy = false;
                        foreach (var policy in secureAction.Policy.Split(",") ?? Enumerable.Empty<string>())
                        {
                            if (user.HasClaim(ClaimTypes.AuthorizationDecision, policy))
                            {
                                passPolicy = true;
                                break;
                            }

                        }
                        if (!passPolicy)
                            return new UnauthorizedAccessException($"User does not meet the required policy: {secureAction.Policy}.");
                    }
                }
            }

            return null; // No exception means authorization is valid
        }
        /// <summary>
        /// Determines if the action requires authorization based on endpoint and action attributes.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        internal static bool EnableAuthorization(this MethodInfo action, Type endPoint)
        {
            var openAction = action.GetCustomAttribute<OpenActionAttribute>() ?? endPoint.GetCustomAttribute<OpenActionAttribute>();
            if (openAction != null && openAction.OpenToPublic)
                return false;

            var secureEndpoint = endPoint.GetCustomAttribute<SecureEndpointAttribute>();
            var secureAction = action.GetCustomAttribute<SecureActionAttribute>();
            if (secureEndpoint == null && secureAction == null)
                return false;

            return true;
        }
        /// <summary>
        /// Adds the appropriate authentication schema to Swagger based on the provided options.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="opt"></param>
        internal static void AddAuthSchema(this SwaggerGenOptions options, SwiftApiOptions? opt)
        {
            if (opt == null)
                return;

            switch (opt.AuthScheme)
            {
                case AuthScheme.Bearer:
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Enter JWT like: **Bearer &lt;token&gt;**"
                    });
                    break;

                case AuthScheme.Basic:
                    options.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "basic",
                        In = ParameterLocation.Header,
                        Description = "Basic Auth. Format: **Basic &lt;base64(username:password)&gt;**"
                    });
                    break;

                case AuthScheme.ApiKey:
                    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                    {
                        Name = opt.ApiKeyName,
                        Type = SecuritySchemeType.ApiKey,
                        In = ParameterLocation.Header,
                        Description = $"API Key passed in `{opt.ApiKeyName}` header"
                    });
                    break;

                case AuthScheme.None:
                    return; // no auth
            }
        }
    }
    /// <summary>
    /// Auth method filter to apply auth on each method required authorization 
    /// </summary>
    internal class AuthOperationFilter : IOperationFilter
    {
        private readonly string? _schemeName;
        private readonly HashSet<MethodInfo> _securedMethods;
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthOperationFilter"/> class.
        /// </summary>
        /// <param name="options"></param>
        public AuthOperationFilter(SwiftApiOptions options)
        {
            _schemeName = options.AuthScheme switch
            {
                AuthScheme.Bearer => "Bearer",
                AuthScheme.Basic => "Basic",
                AuthScheme.ApiKey => "ApiKey",
                _ => null
            };

            // Load only secured interface methods (those marked with SecureEndpointAttribute)
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var interfaceMethods = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsInterface && t.GetCustomAttribute<EndPointAttribute>() != null)
                .SelectMany(intf =>intf.GetMethods().Where(m => m.EnableAuthorization(intf)))
                .ToHashSet(); // so we can compare MethodInfo easily

            _securedMethods = interfaceMethods;

            var models = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && t.GetCustomAttribute<ModelEndPointAttribute>() != null).ToList();

            models.ForEach(m =>
            {
                var modelMethods = m.GetCustomAttribute<ModelEndPointAttribute>()?.Interface?.GetAllMethods()
                .Where(method => method.EnableAuthorization(m)).ToHashSet();
                if (modelMethods != null)
                    _securedMethods.UnionWith(modelMethods);
            });
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (_schemeName == null)
                return;

            // Get the original interface MethodInfo (if available)
            var interfaceMethod = context.ApiDescription.ActionDescriptor
                .EndpointMetadata?.OfType<MethodMetadata>()
                .FirstOrDefault()?.Method;

            if (interfaceMethod == null || !_securedMethods.Contains(interfaceMethod))
                return;

            operation.Security ??= new List<OpenApiSecurityRequirement>();
            operation.Security.Add(new OpenApiSecurityRequirement 
            {
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference{
                            Type = ReferenceType.SecurityScheme,Id = _schemeName
                        }
                    }, Array.Empty<string>()
                }
            });
        }
    }
}
