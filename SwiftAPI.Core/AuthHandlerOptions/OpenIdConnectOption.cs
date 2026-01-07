using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwiftAPI.Core.AuthHandlerOptions
{
    public class OpenIdConnectOption
    {
        public Action<JwtBearerOptions>? JwtBearerOptions { get; set; }
        public string? OpenIdConnectConfigUrl { get; set; }
    }
}
