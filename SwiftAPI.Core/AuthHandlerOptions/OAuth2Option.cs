using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwiftAPI.Core.AuthHandlerOptions
{
    public class OAuth2Option
    {
        public Action<JwtBearerOptions>? JwtBearerOptions { get; set; }
        public string AuthorizationUrl { get; set; }
        public string TokenUrl { get; set; }
        public OAuth2Flow OAuth2Flow { get; set; } 
        public Dictionary<string, string>? Scopes { get; set; }

        public OAuth2Option()
        {
            AuthorizationUrl = string.Empty;
            TokenUrl = string.Empty;
            Scopes = new Dictionary<string, string>();
            OAuth2Flow = OAuth2Flow.AuthorizationCode;

        }

        public string UriBuilder(string baseUri, string path)
        {
            if (!baseUri.EndsWith('/'))
                baseUri = $"{baseUri}/";

            if (path.StartsWith('/'))
                path = path.Substring(1);

            return $"{baseUri}{path}";
        }
    }
}
