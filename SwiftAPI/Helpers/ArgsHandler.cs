﻿using Microsoft.AspNetCore.Http;
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
#pragma warning disable CS8603 // Possible null reference return.
                    return ConvertTo(p.ParameterType, routeVal);
#pragma warning restore CS8603 // Possible null reference return.
                }
                else
                {
                    var queryVal = req.Query[p.Name!].FirstOrDefault();
                    if (queryVal != null)
#pragma warning disable CS8603 // Possible null reference return.
                        return ConvertTo(p.ParameterType, queryVal);
#pragma warning restore CS8603 // Possible null reference return.

                    return p.ParameterType.IsValueType
                        ? Activator.CreateInstance(p.ParameterType)!
                        : null!;
                }
            }
        }
        /// <summary>
        /// Resolves method parameters from the HTTP request body, including files.
        /// </summary>
        /// <param name="param"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        internal static async Task<object> ResolveFromBodyAsync(this ParameterInfo param, HttpRequest req)
        {
            req.EnableBuffering();

            try
            {
                if (req.HasFormContentType)
                {
                    await req.ReadFormAsync(); // ensures Form is populated

                    // Handle single file
                    if (param.ParameterType == typeof(IFormFile))
#pragma warning disable CS8603 // Possible null reference return.
                        return req.Form.Files.FirstOrDefault();
#pragma warning restore CS8603 // Possible null reference return.

                    // Handle array of files
                    if (param.ParameterType == typeof(IFormFile[]))
                        return req.Form.Files.ToArray();

                    // Handle complex model from form fields (e.g., string name + IFormFile file)
                    var instance = Activator.CreateInstance(param.ParameterType);
                    var properties = param.ParameterType.GetProperties();

                    foreach (var prop in properties)
                    {
                        if (typeof(IFormFile).IsAssignableFrom(prop.PropertyType))
                        {
                            var file = req.Form.Files.GetFile(prop.Name);
                            if (file != null)
                                prop.SetValue(instance, file);
                        }
                        else if (typeof(IFormFile[]).IsAssignableFrom(prop.PropertyType))
                        {
                            var files = req.Form.Files.GetFiles(prop.Name).ToArray();
                            if (files.Any())
                                prop.SetValue(instance, files);
                        }
                        else if (req.Form.TryGetValue(prop.Name, out var value))
                        {
                            var converted = ConvertTo(prop.PropertyType, value.ToString());
                            prop.SetValue(instance, converted);
                        }
                    }

#pragma warning disable CS8603 // Possible null reference return.
                    return instance;
#pragma warning restore CS8603 // Possible null reference return.
                }
                else
                {
                    // Fallback to JSON deserialization
                    var result = await JsonSerializer.DeserializeAsync(
                        req.Body,
                        param.ParameterType,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    req.Body.Position = 0;
#pragma warning disable CS8603 // Possible null reference return.
                    return result;
#pragma warning restore CS8603 // Possible null reference return.
                }
            }
            catch (Exception)
            {
                req.Body.Position = 0;
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
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
