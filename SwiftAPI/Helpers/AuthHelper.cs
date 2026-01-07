using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using SwiftAPI.AuthHandler;
using SwiftAPI.Core;
using SwiftAPI.Core.AuthHandlerOptions;
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
        /// Register Auth to the project
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        internal static void AddSwiftApiAuth(this IServiceCollection services, SwiftApiOptions config)
        {
            if (config.AuthScheme == AuthScheme.Basic && config.BasicAuthOption != null)
                services.AddAuthentication(BasicAuthenticationHandler.SchemeName)
                    .AddScheme<BasicAuthOption, BasicAuthenticationHandler>
                    (BasicAuthenticationHandler.SchemeName, config.BasicAuthOption);

            if (config.AuthScheme == AuthScheme.ApiKey && config.ApiKeyAuthOption != null)
                services.AddAuthentication(ApiKeyAuthenticationHandler.SchemeName)
                    .AddScheme<ApiKeyAuthOption, ApiKeyAuthenticationHandler>
                    (ApiKeyAuthenticationHandler.SchemeName, config.ApiKeyAuthOption);

            if (config.AuthScheme == AuthScheme.Bearer && config.BearerAuthOptions != null)
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(config.BearerAuthOptions.JwtBearerOptions!);


            if (config.AuthScheme == AuthScheme.OAuth2 && config.OAuth2Options != null)
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(config.OAuth2Options.JwtBearerOptions!);

            if (config.AuthScheme == AuthScheme.OpenIdConnect && config.OpenIdConnectOptions != null)
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(config.OpenIdConnectOptions.JwtBearerOptions!);

            if (config.AuthScheme != AuthScheme.None)
                services.AddAuthorization();
        }
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
                        Scheme = "Basic",
                        In = ParameterLocation.Header,
                        Description = "Basic Auth. Format: **Basic &lt;base64(username:password)&gt;**"
                    });
                    break;

                case AuthScheme.ApiKey:
                    if (opt.ApiKeyAuthOption == null)
                        return;

                    var keysOptions = new ApiKeyAuthOption();
                    opt.ApiKeyAuthOption.Invoke(keysOptions);

                    if (keysOptions.AuthCridentionals == null)
                        return;

                    foreach(var key in keysOptions.AuthCridentionals)
                    {
                        options.AddSecurityDefinition(key.KeyName, new OpenApiSecurityScheme
                        {
                            Name = key.KeyName,
                            Scheme = ApiKeyAuthenticationHandler.SchemeName,
                            Type = SecuritySchemeType.ApiKey,
                            In = ParameterLocation.Header,
                            Description = $"API Key passed in `{key.KeyName}` header"
                        });
                    }
                    break;
                case AuthScheme.OAuth2:
                    ArgumentNullException.ThrowIfNull(opt.OAuth2Options);
                    options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
                    {
                        Name = "OAuth2",
                        Scheme = "OAuth2",
                        Type = SecuritySchemeType.OAuth2,
                        Description = "OAuth2 Authorization Code flow",
                        Flows = GetOpenApiOAuthFlow(opt.OAuth2Options)
                    });
                    break;
                case AuthScheme.OpenIdConnect:
                    ArgumentNullException.ThrowIfNull(opt.OpenIdConnectOptions);
                    ArgumentException.ThrowIfNullOrWhiteSpace(opt.OpenIdConnectOptions.OpenIdConnectConfigUrl);
                    options.AddSecurityDefinition("OpenIdConnect", new OpenApiSecurityScheme
                    {
                        Name = "OpenIdConnect",
                        Scheme = "OpenIdConnect",
                        Type = SecuritySchemeType.OpenIdConnect,
                        Description = "OpenID Connect authentication",
                        OpenIdConnectUrl = new Uri(opt.OpenIdConnectOptions.OpenIdConnectConfigUrl)
                    });
                    break;
                case AuthScheme.None:
                    return; // no auth
            }
        }
        /// <summary>
        /// Get Type of flow for OAuth2
        /// </summary>
        /// <param name="oAuth2Options"></param>
        /// <returns></returns>
        private static OpenApiOAuthFlows GetOpenApiOAuthFlow(OAuth2Option oAuth2Options)
        {
            switch (oAuth2Options.OAuth2Flow)
            {
                case OAuth2Flow.Password:
                    return new OpenApiOAuthFlows
                    {
                        Password = new()
                        {
                            AuthorizationUrl = new Uri(oAuth2Options.AuthorizationUrl),
                            TokenUrl = new Uri(oAuth2Options.TokenUrl),
                            Scopes = oAuth2Options.Scopes
                        }
                    };
                case OAuth2Flow.ClientCredentials:
                    return new OpenApiOAuthFlows
                    {
                        ClientCredentials = new()
                        {
                            AuthorizationUrl = new Uri(oAuth2Options.AuthorizationUrl),
                            TokenUrl = new Uri(oAuth2Options.TokenUrl),
                            Scopes = oAuth2Options.Scopes
                        }
                    };
                default:
                    return new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new()
                        {
                            AuthorizationUrl = new Uri(oAuth2Options.AuthorizationUrl),
                            TokenUrl = new Uri(oAuth2Options.TokenUrl),
                            Scopes = oAuth2Options.Scopes
                        }
                    };
            }
        }
    }
    /// <summary>
    /// Auth method filter to apply auth on each method required authorization 
    /// </summary>
    internal class AuthOperationFilter : IOperationFilter
    {
        //private readonly string? _schemeName;
        private readonly List<string> _schemesName = new List<string>();
        private readonly List<string> _scopes = new List<string>();
        private readonly HashSet<MethodInfo> _securedMethods;
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthOperationFilter"/> class.
        /// </summary>
        /// <param name="options"></param>
        public AuthOperationFilter(SwiftApiOptions options)
        {
            var _schemeName = options.AuthScheme switch
            {
                AuthScheme.Bearer => "Bearer",
                AuthScheme.Basic => "Basic",
                AuthScheme.ApiKey => "ApiKey",
                AuthScheme.OAuth2 => "OAuth2",
                AuthScheme.OpenIdConnect => "OpenIdConnect",
                _ => null
            };

            //Set Schemes Names;
            if(options.AuthScheme == AuthScheme.ApiKey)
            {
                if(options.ApiKeyAuthOption != null)
                {
                    var apiKeyOptions = new ApiKeyAuthOption();
                    options.ApiKeyAuthOption.Invoke(apiKeyOptions);
                    if(apiKeyOptions.AuthCridentionals != null)
                    {
                        foreach (var apyKeyScheme in apiKeyOptions.AuthCridentionals)
                            _schemesName.Add(apyKeyScheme.KeyName);
                    }
                }
            }
            else
            {
                _schemesName.Add(_schemeName?? "None");
            }

            //Set Scopes of OAuth2
            if (options.AuthScheme == AuthScheme.OAuth2)
            {
                if (options.OAuth2Options != null)
                {
                    if (options.OAuth2Options.Scopes != null)
                        _scopes = options.OAuth2Options.Scopes.Keys.ToList();
                }
            }
            // Load only secured interface methods (those marked with SecureEndpointAttribute)
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var interfaceMethods = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsInterface && t.GetCustomAttribute<EndPointAttribute>() != null)
                .SelectMany(intf => intf.GetMethods().Where(m => m.EnableAuthorization(intf)))
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
            //if (_schemeName == null)
            if (!_schemesName.Any())
                return;
            // Get the original interface MethodInfo (if available)
            var interfaceMethod = context.ApiDescription.ActionDescriptor
                .EndpointMetadata?.OfType<MethodMetadata>()
                .FirstOrDefault()?.Method;

            operation.Security = new List<OpenApiSecurityRequirement>();
            if (interfaceMethod == null || !_securedMethods.Contains(interfaceMethod))
                return;


            var document = context.Document;
            foreach(var _schemeName in _schemesName)
            {
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference(_schemeName, document, null),
                        _scopes
                    }
                });
            }
            //operation.Security = new List<OpenApiSecurityRequirement>
            //{
            //    new OpenApiSecurityRequirement
            //    {

            //        {
            //            new OpenApiSecuritySchemeReference(_schemeName, document, null),
            //            _scopes
            //        }
            //    }
            //};
        }
    }
}
