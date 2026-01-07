using Microsoft.AspNetCore.Authentication.JwtBearer;
using SwiftAPI.Core;
using SwiftAPI.Core.AuthHandlerOptions;

namespace SwiftAPI.Shared
{
    /// <summary>
    /// Options class for configuring SwiftAPI behavior, such as authentication scheme and API key name.
    /// </summary>
    public class SwiftApiOptions
    {
        public AuthScheme AuthScheme { get; internal set; }
        public Action<BasicAuthOption>? BasicAuthOption { get; internal set; }
        public Action<ApiKeyAuthOption>? ApiKeyAuthOption { get; internal set; }
        public BearerAuthOption? BearerAuthOptions { get; internal set; }
        public OAuth2Option? OAuth2Options { get; internal set; } 
        public OpenIdConnectOption? OpenIdConnectOptions { get; set; }
    }

    public static class SwiftApiOptionsExtensions
    {
        public static void UseBearer(this SwiftApiOptions options, Action<BearerAuthOption> bearerAuthOptions)
        {
            options.BearerAuthOptions = new BearerAuthOption();
            bearerAuthOptions.Invoke(options.BearerAuthOptions);
            options.AuthScheme = AuthScheme.Bearer;
        }
        public static void UseBasic(this SwiftApiOptions options, Action<BasicAuthOption> basicAuthOption)
        {
            options.BasicAuthOption = basicAuthOption;
            options.AuthScheme = AuthScheme.Basic;
        }
        public static void UseApiKey(this SwiftApiOptions options, Action<ApiKeyAuthOption> apiKeyAuthOption)
        {
            options.ApiKeyAuthOption = apiKeyAuthOption;
            options.AuthScheme = AuthScheme.ApiKey;
        }
        public static void UseOAuth2(this SwiftApiOptions options, Action<OAuth2Option> oAuth2Option)
        {
            options.OAuth2Options ??= new OAuth2Option();
            oAuth2Option.Invoke(options.OAuth2Options);
            options.AuthScheme = AuthScheme.OAuth2;
        }

        public static void UseOpenIdConnect(this SwiftApiOptions options, Action<OpenIdConnectOption> openIdConnectOption)
        {
            options.OpenIdConnectOptions ??= new OpenIdConnectOption();
            openIdConnectOption.Invoke(options.OpenIdConnectOptions);
            options.AuthScheme = AuthScheme.OpenIdConnect;
        }
    }
}
