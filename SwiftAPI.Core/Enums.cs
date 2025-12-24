using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftAPI.Core
{
    public enum ActionType
    {
        Get,
        Post,
        Put,
        Delete,
        Patch,
        Options,
        Head,
        None
    }

    public enum RegisterType
    {
        Scoped,
        Singleton,
        Transient
    }

    public enum ParamType
    {
        FromBody,
        FromQuery,
        FromRoute,
        FromHeader
    }

    public enum AuthScheme
    {
        None,
        Basic,
        Bearer,
        ApiKey,
        OAuth2,
        OpenIdConnect
    }

    public enum OAuth2Flow
    {
        Password,
        AuthorizationCode,
        ClientCredentials
    }
}
