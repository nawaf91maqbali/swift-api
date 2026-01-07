using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SwiftAPI.Core.AuthHandlerOptions;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace SwiftAPI.AuthHandler
{
    internal class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthOption>
    {
        public const string SchemeName = "ApiKey";
        public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthOption> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var endPoint = Request.HttpContext.GetEndpoint();
            if (endPoint == null)
                return await Task.FromResult(AuthenticateResult.NoResult());

            var requiresAuth = endPoint!.Metadata.GetMetadata<IAuthorizeData>() != null;

            if (!requiresAuth)
                return await Task.FromResult(AuthenticateResult.NoResult());

            if(Options.AuthCridentionals == null)
                return await Task.FromResult(AuthenticateResult.Fail("API Keys Cridentionals Not Configured"));

            foreach(var key in Options.AuthCridentionals)
            {
                if(Request.Headers.TryGetValue(key.KeyName, out var providedKeyValue))
                {
                    if(key.KeyValue != providedKeyValue)
                        return await Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));

                    var claims = new List<Claim>(){new Claim(ClaimTypes.Name, key.KeyName)};

                    var identity = new ClaimsIdentity(claims, SchemeName);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, SchemeName);

                    return await Task.FromResult(AuthenticateResult.Success(ticket));
                }  
            }

            return await Task.FromResult(AuthenticateResult.Fail("API Key missing"));
        }
    }
}
