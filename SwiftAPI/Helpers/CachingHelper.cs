using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using SwiftAPI.Shared;
using System.Reflection;

namespace SwiftAPI.Helpers
{
    internal static class CachingHelper
    {
        internal static void EnableCaching(this MethodInfo getAction, HttpResponse res)
        {
            var getActionAttr = getAction.GetCustomAttribute<ActionAttribute>();
            if (getActionAttr is GetActionAttribute getActionAttrWithCache)
            {
                if(getActionAttrWithCache.EnableCache)
                {
                    res.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromMinutes(getActionAttrWithCache.CacheDuration)
                    };
                    res.Headers[HeaderNames.Vary] = "Accept-Encoding";
                }
            }
        }
    }
}
