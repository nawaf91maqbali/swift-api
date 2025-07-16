using Microsoft.OpenApi.Models;
using SwiftAPI.Shared;
using System.Reflection;

namespace SwiftAPI.Helpers
{
    /// <summary>
    /// Helper class for resolving OpenAPI operations based on method parameters and attributes.
    /// </summary>
    static class OperationHelper
    {
        /// <summary>
        /// Resolves OpenAPI operation details from a method, including parameters and tags.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="tagName"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static OpenApiOperation ResolveOperations(this OpenApiOperation o, string tagName, MethodInfo method)
        {
            o.OperationId = $"{tagName}_{method.Name}".ToLower();
            o.Tags = new List<OpenApiTag> { new OpenApiTag { Name = tagName.ToUpper() } };
            var parameters = method.GetParameters();
            foreach (var p in parameters)
            {
                var location = p.GetCustomAttribute<ParamAttribute>()?.Type ?? ParamType.FromQuery;
                switch (location)
                {
                    case ParamType.FromRoute:
                        o.Parameters.Add(new OpenApiParameter
                        {
                            Name = p.Name!,
                            In = ParameterLocation.Path,
                            Required = !p.IsOptional,
                            Schema = p.ParameterType.GenerateOpenApiSchema()
                        });
                        break;
                    case ParamType.FromHeader:
                        o.Parameters.Add(new OpenApiParameter
                        {
                            Name = p.Name!,
                            In = ParameterLocation.Header,
                            Required = !p.IsOptional,
                            Schema = p.ParameterType.GenerateOpenApiSchema()
                        });
                        break;
                    case ParamType.FromBody:
                        o.RequestBody = new OpenApiRequestBody
                        {
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = p.ParameterType.GenerateOpenApiSchema()
                                }
                            }
                        };
                        break;
                    default:
                        p.ResolveQueryParams(o);
                        break;

                }
            }

            o.GenerateOpenApiReturnSchema(method);

            return o;
        }
    }
}
