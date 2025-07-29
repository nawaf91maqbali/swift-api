using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using SwiftAPI.Core;
using SwiftAPI.Shared;
using System.Reflection;

namespace SwiftAPI.Helpers
{
    internal static class CachingHelper
    {
        internal static void EnableCaching(this RouteHandlerBuilder api, MethodInfo getAction)
        {
            var getActionAttr = getAction.GetCustomAttribute<ActionAttribute>();
            if (getActionAttr is GetActionAttribute getActionAttrWithCache && getActionAttrWithCache.EnableCache)
            {
                api.CacheOutput(o => o.SetVaryByQuery("*")
                .SetVaryByHeader("*")
                .Expire(TimeSpan.FromMinutes(getActionAttrWithCache.CacheDuration)));
            }
        }
    }
}
