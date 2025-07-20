using System.Text.RegularExpressions;

namespace SwiftAPI.Helpers
{
    /// <summary>
    /// Helper class for converting names to Swift API naming conventions.
    /// </summary>
    internal static class NamingHelper
    {
        /// <summary>
        /// Converts a string to Swift API naming convention.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static string ToSwiftApiName(this string name)
        {
            return Regex.Replace(name, "(?<=[a-z])([A-Z])", "_$1").ToLower();
        }
    }
}
