using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwiftAPI.Core.AuthHandlerOptions
{
    public class BearerAuthOption
    {
        public Action<JwtBearerOptions>? JwtBearerOptions { get; set; }
    }
}
