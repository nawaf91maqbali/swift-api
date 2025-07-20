using System.Reflection;

namespace SwiftAPI.Shared
{
    /// <summary>
    /// Metadata class for storing information about an endpoint method.
    /// </summary>
    class MethodMetadata
    {
        public MethodInfo Method { get; }

        public MethodMetadata(MethodInfo method)
        {
            Method = method;
        }
    }
}
