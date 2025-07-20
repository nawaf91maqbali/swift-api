using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

namespace SwiftAPI.Helpers
{
    /// <summary>
    /// Handles the resolution of method parameters from HTTP requests.
    /// </summary>
    internal static class ArgsHandler
    {
        /// <summary>
        /// Resolves method parameters from the HTTP request's query string or route values.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        internal static object ResolveFromQueryOrRoute(this ParameterInfo p, HttpRequest req)
        {
            if (SchemaHelper.GetOpenApiType(p.ParameterType) == "object")
            {
                var objectParam = Activator.CreateInstance(p.ParameterType);
                var properties = p.ParameterType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in properties)
                {
                    if (req.RouteValues.TryGetValue(prop.Name, out var routeVal))
                    {
                        prop.SetValue(objectParam, ConvertTo(prop.PropertyType, routeVal));
                    }
                    else
                    {
                        var queryVal = req.Query[prop.Name].FirstOrDefault();
                        if (queryVal != null)
                            prop.SetValue(objectParam, ConvertTo(prop.PropertyType, queryVal));
                        else if (prop.PropertyType.IsValueType)
                            prop.SetValue(objectParam, Activator.CreateInstance(prop.PropertyType));
                        else
                            prop.SetValue(objectParam, null);
                    }
                }
                return objectParam!;
            }
            else
            {
                if (req.RouteValues.TryGetValue(p.Name!, out var routeVal))
                {
                    return ConvertTo(p.ParameterType, routeVal);
                }
                else
                {
                    var queryVal = req.Query[p.Name!].FirstOrDefault();
                    if (queryVal != null)
                        return ConvertTo(p.ParameterType, queryVal);

                    return p.ParameterType.IsValueType
                        ? Activator.CreateInstance(p.ParameterType)!
                        : null!;
                }
            }
        }
        /// <summary>
        /// Resolves method parameters from the HTTP request body.
        /// </summary>
        /// <param name="param"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        internal static async Task<object> ResolveFromBodyAsync(this ParameterInfo param, HttpRequest req)
        {
            req.EnableBuffering(); // Allows multiple reads of the stream
            try
            {
                var result = await JsonSerializer.DeserializeAsync(
                    req.Body,
                    param.ParameterType,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                req.Body.Position = 0; // Reset for potential future reads
                return result;
            }
            catch (JsonException)
            {
                return null; // or throw specific exception
            }
        }
        /// <summary>
        /// Converts a value to the specified type, handling nullable types and custom conversions.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object? ConvertTo(Type type, object? value)
        {
            if (value == null)
                return type.IsValueType ? Activator.CreateInstance(type) : null;

            if (type == typeof(Guid))
                return Guid.Parse(value.ToString()!);

            if (Nullable.GetUnderlyingType(type) is Type underlyingType)
            {
                if (string.IsNullOrWhiteSpace(value.ToString()))
                    return null;

                return ConvertTo(underlyingType, value);
            }

            var converter = TypeDescriptor.GetConverter(type);
            if (converter != null && converter.CanConvertFrom(value.GetType()))
                return converter.ConvertFrom(value);

            return Convert.ChangeType(value, type);
        }
    }
}
