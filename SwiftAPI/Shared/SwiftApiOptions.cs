using SwiftAPI.Core;

namespace SwiftAPI.Shared
{
    /// <summary>
    /// Options class for configuring SwiftAPI behavior, such as authentication scheme and API key name.
    /// </summary>
    public class SwiftApiOptions
    {
        public AuthScheme AuthScheme { get; set; }
        public string ApiKeyName { get; set; } = "X-API-KEY";
        public OAuth2Options? OAuth2Options { get; set; } 
        public OpenIdConnectOptions? OpenIdConnectOptions { get; set; }

    }

    public class OAuth2Options
    {
        public required string OAuth2AuthUrl { get; set; }
        public required string OAuth2TokenUrl { get; set; }
        public required OAuth2Flow OAuth2Flow { get; set; }
        public Dictionary<string, string>? OAuth2Scopes { get; set; }
    }

    public class OpenIdConnectOptions { 
        public required string OpenIdConnectConfigUrl { get; set; }
    }
}
