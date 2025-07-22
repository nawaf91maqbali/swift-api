using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using SwiftAPI.Core;
using SwiftAPI.Shared;
using System.Reflection;

namespace SwiftAPI.Helpers
{
    /// <summary>
    /// Helper class for generating OpenAPI schemas from .NET types.
    /// </summary>
    internal static class SchemaHelper
    {
        /// <summary>
        /// Generates an OpenAPI schema for a given .NET type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static OpenApiSchema GenerateOpenApiSchema(this Type type)
        {
            if (Nullable.GetUnderlyingType(type) is Type underlyingNullable)
                type = underlyingNullable;

            if (type == typeof(IFormFile) || type == typeof(IFormFile[]) || type == typeof(IFormFileCollection))
            {
                return new OpenApiSchema
                {
                    Type = "object",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        [type.Name ?? "file"] = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        }
                    },
                    Required = new HashSet<string> { type.Name ?? "file" }
                };
            }

            var (openApiType, format) = type.GetOpenApiTypeAndFormat();
            var schema = new OpenApiSchema
            {
                Type = openApiType,
                Format = format
            };

            if (openApiType == "array")
            {
                Type? elementType = type.IsArray
                    ? type.GetElementType()
                    : type.GetGenericArguments().FirstOrDefault();

                schema.Items = elementType != null
                    ? elementType.GenerateOpenApiSchema()
                    : new OpenApiSchema { Type = "object" };
            }
            else if (openApiType == "object" && type.IsClass && type != typeof(string))
            {
                schema.Properties = new Dictionary<string, OpenApiSchema>();

                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in properties)
                {
                    schema.Properties[prop.Name] = prop.PropertyType.GenerateOpenApiSchema();
                }
            }

            return schema;
        }
        /// <summary>
        /// Generates an OpenAPI return schema for a given operation based on the method's return type.
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="method"></param>
        internal static void GenerateOpenApiReturnSchema(this OpenApiOperation operation, MethodInfo method)
        {
            operation.Responses.Clear();

            var returnType = method.ReturnType;

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
                returnType = returnType.GetGenericArguments()[0];

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(ActionResult<>))
                returnType = returnType.GetGenericArguments()[0];

            if (returnType == typeof(void) || returnType == typeof(Task))
            {
                operation.Responses["204"] = new OpenApiResponse { Description = "No Content" };
            }
            else
            {
                operation.Responses["200"] = new OpenApiResponse
                {
                    Description = "Success",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = returnType.GenerateOpenApiSchema()
                        }
                    }
                };
            }
        }
        /// <summary>
        /// Gets the OpenAPI type for a given .NET type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static string GetOpenApiType(this Type type)
        {
            if (type == typeof(string) || type == typeof(char))
                return "string";

            if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte) || type == typeof(uint) || type == typeof(ulong))
                return "integer";

            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
                return "number";

            if (type == typeof(bool))
                return "boolean";

            if (type == typeof(DateTime) || type == typeof(DateTimeOffset))
                return "string"; // optionally add format: date-time

            if (type == typeof(Guid))
                return "string"; // optionally add format: uuid

            if (type.IsArray ||
                (type.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition())))
                return "array";

            // Nullable<T> unwrap
            if (Nullable.GetUnderlyingType(type) is Type underlyingType)
                return GetOpenApiType(underlyingType);

            return "object"; // fallback
        }
        /// <summary>
        /// Gets the OpenAPI type and format for a given .NET type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static (string type, string? format) GetOpenApiTypeAndFormat(this Type type)
        {
            if (Nullable.GetUnderlyingType(type) is Type underlying)
                return GetOpenApiTypeAndFormat(underlying);

            if (type == typeof(string)) return ("string", null);
            if (type == typeof(Guid)) return ("string", "uuid");
            if (type == typeof(DateTime) || type == typeof(DateTimeOffset)) return ("string", "date-time");

            if (type == typeof(bool)) return ("boolean", null);
            if (type == typeof(int) || type == typeof(short) || type == typeof(byte)) return ("integer", "int32");
            if (type == typeof(long) || type == typeof(ulong)) return ("integer", "int64");

            if (type == typeof(float)) return ("number", "float");
            if (type == typeof(double) || type == typeof(decimal)) return ("number", "double");

            if (type == typeof(IFormFile) || type == typeof(IFormFile[]) || type == typeof(IFormFileCollection))
                return ("string", "binary");

            if (type.IsArray && type.GetElementType() == typeof(IFormFile))
                return ("array", null);

            if (type.IsArray) return ("array", null);

            if (type.IsGenericType)
            {
                var genericDef = type.GetGenericTypeDefinition();
                if (genericDef == typeof(List<>) || genericDef == typeof(IEnumerable<>)
                    || genericDef == typeof(ICollection<>) || genericDef == typeof(IReadOnlyList<>)
                    || genericDef == typeof(IReadOnlyCollection<>))
                {
                    return ("array", null);
                }
            }

            return ("object", null);
        }
        /// <summary>
        /// Gets the return type of a method, handling Task and ActionResult types.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        internal static Type GetReturnType(this MethodInfo method, Type? concreteGenericType = null)
        {
            var returnType = method.ReturnType;

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
                returnType = returnType.GetGenericArguments()[0];

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(ActionResult<>))
                returnType = returnType.GetGenericArguments()[0];

            if (returnType.IsGenericParameter && concreteGenericType != null)
                returnType = concreteGenericType;

            return returnType;
        }

    }
    /// <summary>
    /// Document filter for registering schema models in Swagger documentation.
    /// </summary>
    internal class SchemaModelRegistrationFilter : IDocumentFilter
    {
        private readonly List<Type> _modelTypes;
        public SchemaModelRegistrationFilter(){
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _modelTypes = assemblies.SelectMany(a => a.GetTypes()).Where(t =>
            t.IsClass &&!t.IsAbstract 
            && !t.ContainsGenericParameters 
            && (t.GetCustomAttribute<SchemaModelAttribute>() != null 
            || t.GetCustomAttribute<ModelEndPointAttribute>() != null))
                .ToList();}
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var model in _modelTypes)
            {
                if (!context.SchemaRepository.Schemas.ContainsKey(model.Name))
                {
                    var schema = context.SchemaGenerator.GenerateSchema(model, context.SchemaRepository);
                    context.SchemaRepository.Schemas[model.Name] = schema;
                }
            }
        }
    }
}
