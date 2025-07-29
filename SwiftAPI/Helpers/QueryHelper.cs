using Microsoft.OpenApi.Models;
using System.Reflection;

namespace SwiftAPI.Helpers
{
    /// <summary>
    /// Helper class for resolving query parameters in OpenAPI operations.
    /// </summary>
    internal static class QueryHelper
    {
        /// <summary>
        /// Resolves query parameters for an OpenAPI operation based on the parameter information.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="o"></param>
        internal static void ResolveQueryParams(this ParameterInfo p, OpenApiOperation o)
        {
            if (p.ParameterType.GetOpenApiType() == "object")
            {
                var properties = p.ParameterType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var prop in properties)
                {
                    o.Parameters.Add(new OpenApiParameter
                    {
                        Name = prop.Name,
                        In = ParameterLocation.Query,
                        Required = !IsNullable(prop.PropertyType),
                        Schema = prop.PropertyType.GenerateOpenApiSchema()
                    });
                }
            }
            else
            {
                o.Parameters.Add(new OpenApiParameter
                {
                    Name = p.Name!,
                    In = ParameterLocation.Query,
                    Required = !p.IsOptional && !IsNullable(p.ParameterType),
                    Schema = p.ParameterType.GenerateOpenApiSchema()
                });
            }
        }

        /// <summary>
        /// Determines if a type is nullable.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsNullable(Type type)
        {
            return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
        }
    }
}
