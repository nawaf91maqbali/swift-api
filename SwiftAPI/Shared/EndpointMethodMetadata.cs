using System.Reflection;

namespace SwiftAPI.Shared
{
    /// <summary>
    /// Metadata class for storing information about an endpoint method.
    /// </summary>
    class EndpointMethodMetadata
    {
        public MethodInfo Method { get; }

        public EndpointMethodMetadata(MethodInfo method)
        {
            Method = method;
        }
    }
}
