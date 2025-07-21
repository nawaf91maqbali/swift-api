using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SwiftAPI.Helpers
{
    /// <summary>
    /// Helper class for retrieving all methods from an interface, including inherited methods from base interfaces.
    /// </summary>
    static class MethodHendler
    {
        /// <summary>
        /// Recursively retrieves all methods from the specified interface type, including those from inherited interfaces.
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="methods"></param>
        /// <param name="visited"></param>
        /// <returns></returns>
        internal static List<MethodInfo> GetAllMethods(this Type interfaceType, List<MethodInfo>? methods = null, HashSet<Type>? visited = null)
        {
            methods ??= new List<MethodInfo>();
            visited ??= new HashSet<Type>();

            // Prevent re-processing the same interface
            if (!visited.Add(interfaceType))
                return methods;

            // Add declared methods
            methods.AddRange(interfaceType.GetMethods());

            // Recurse into inherited interfaces
            foreach (var subInterface in interfaceType.GetInterfaces())
            {
                subInterface.GetAllMethods(methods, visited);
            }

            return methods.Distinct().ToList();
        }
    }
}
