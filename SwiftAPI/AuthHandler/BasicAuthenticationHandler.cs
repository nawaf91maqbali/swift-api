using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SwiftAPI.Core.AuthHandlerOptions;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace SwiftAPI.AuthHandler
{
    internal class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthOption>
    {
        public const string SchemeName = "BasicAuthentication";
        public BasicAuthenticationHandler(IOptionsMonitor<BasicAuthOption> options, ILoggerFactory logger, UrlEncoder encoder) 
            : base(options, logger, encoder)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var endPoint = Request.HttpContext.GetEndpoint();
            if (endPoint == null)
                return await Task.FromResult(AuthenticateResult.NoResult());

            var requiresAuth = endPoint!.Metadata.GetMetadata<IAuthorizeData>() != null;

            if(!requiresAuth)
                return await Task.FromResult(AuthenticateResult.NoResult());

            if(!Request.Headers.TryGetValue("Authorization", out var authHeaderValue))
                return AuthenticateResult.Fail("Missing Authorization Header");

            var authHeader = AuthenticationHeaderValue.Parse(authHeaderValue!);
            var cridBytes = Convert.FromBase64String(authHeader.Parameter!);
            var crids = Encoding.UTF8.GetString(cridBytes).Split(':');

            var username = crids[0];
            var password = crids[1];
            
            if(username == null || password == null)
                return AuthenticateResult.Fail("Invalid Username or Password");

            Options.AuthCridentionals ??= new List<BasicAuthCridentional>();

            var user = Options.AuthCridentionals.FirstOrDefault(x => x.Username == username);
            if(user == null)
                return AuthenticateResult.Fail("Invalid Username");

            if(user.Password != password)
                return AuthenticateResult.Fail("Invalid Password");

            //var apiName = Request.Path.Value;
            //if (user.BlackListedEndPoints is List<string> blackList && blackList.Any(x => x.Contains(apiName!)))
            //    return AuthenticateResult.Fail("EndPoint Access Denied");

            //if (user.BlackListedEndPoints is List<string> whiteList && whiteList.Any(x => x.Contains(apiName!)))
            //    return AuthenticateResult.Fail("EndPoint Access Denied");

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Name, user.Username)
            };
            foreach (var role in user.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return AuthenticateResult.Success(ticket);
        }
    }
}
