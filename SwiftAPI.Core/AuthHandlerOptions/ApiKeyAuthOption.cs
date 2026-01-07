using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SwiftAPI.Core.AuthHandlerOptions
{
    public class ApiKeyAuthOption : AuthenticationSchemeOptions
    {
        public List<ApiKeyCridentional>? AuthCridentionals { get; set; }
    }

    public class ApiKeyCridentional
    {
        public string KeyName { get; set; }
        public string KeyValue { get; set; }
        //public List<string>? WhiteListedEndPoints { get; set; }
        //public List<string>? BlackListedEndPoints { get; set; }

        public ApiKeyCridentional(string keyName, string keyValue) {
            KeyName = keyName;
            KeyValue = keyValue;
        }
    }
}
