using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SwiftAPI.Shared;
using System.Reflection;

namespace SwiftAPI.Helpers
{
    /// <summary>
    /// Helper class for mapping API endpoints in a WebApplication.
    /// </summary>
    static class ApiHelper
    {
        /// <summary>
        /// Maps a GET API endpoint to the specified route with the provided action method.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="route"></param>
        /// <param name="endPoint"></param>
        /// <param name="action"></param>
        /// <param name="apiName"></param>
        public static void MapGetApi(this WebApplication app, string route, Type endPoint, MethodInfo action, string apiName)
        {
            var enableAuth = action.EnableAuthorization(endPoint);
            var api = app.MapGet(route, async (HttpRequest req, HttpResponse res) =>
            {
                var authEx = action.ValidateAuthorization(endPoint, req.HttpContext.User);
                if (authEx != null)
                {
                    var error = authEx.HandleError(res);
                    await res.WriteAsJsonAsync(error);
                    return;
                }
                var service = app.Services.GetServiceFromSafeScope(endPoint, out var scope);
                try
                {
                    var args = await req.ResolveArgsAsync(action);
                    var result = action.Invoke(service, args);
                    await res.WriteAsync(result);
                }
                catch (Exception ex)
                {
                    var error = ex.HandleError(res);
                    await res.WriteAsJsonAsync(error);
                }
                finally
                {
                    scope?.Dispose();
                }
            }).WithOpenApi(o => o.ResolveOperations(apiName, action))
            .WithMetadata(new EndpointMethodMetadata(action))
            .Produces(StatusCodes.Status200OK, action.GetReturnType());

            if (enableAuth)
                api.RequireAuthorization();
        }
        /// <summary>
        /// Maps a POST API endpoint to the specified route with the provided action method.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="route"></param>
        /// <param name="endPoint"></param>
        /// <param name="action"></param>
        /// <param name="apiName"></param>
        public static void MapPostApi(this WebApplication app, string route, Type endPoint, MethodInfo action, string apiName)
        {
            var enableAuth = action.EnableAuthorization(endPoint);
            var api = app.MapPost(route, async (HttpRequest req, HttpResponse res) =>
            {
                var authEx = action.ValidateAuthorization(endPoint, req.HttpContext.User);
                if (authEx != null)
                {
                    var error = authEx.HandleError(res);
                    await res.WriteAsJsonAsync(error);
                    return;
                }
                var service = app.Services.GetServiceFromSafeScope(endPoint, out var scope);
                try
                {
                    var args = await req.ResolveArgsAsync(action);
                    var result = action.Invoke(service, args);
                    await res.WriteAsync(result);
                }
                catch (Exception ex)
                {
                    var error = ex.HandleError(res);
                    await res.WriteAsJsonAsync(error);
                }
                finally
                {
                    scope?.Dispose();
                }
            }).WithOpenApi(o => o.ResolveOperations(apiName, action))
            .WithMetadata(new EndpointMethodMetadata(action))
            .Produces(StatusCodes.Status200OK, action.GetReturnType());

            if (enableAuth)
                api.RequireAuthorization();
        }
        /// <summary>
        /// Maps a PUT API endpoint to the specified route with the provided action method.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="route"></param>
        /// <param name="endPoint"></param>
        /// <param name="action"></param>
        /// <param name="apiName"></param>
        public static void MapPutApi(this WebApplication app, string route, Type endPoint, MethodInfo action, string apiName)
        {
            var enableAuth = action.EnableAuthorization(endPoint);
            var api = app.MapPut(route, async (HttpRequest req, HttpResponse res) =>
            {
                var authEx = action.ValidateAuthorization(endPoint, req.HttpContext.User);
                if (authEx != null)
                {
                    var error = authEx.HandleError(res);
                    await res.WriteAsJsonAsync(error);
                    return;
                }
                var service = app.Services.GetServiceFromSafeScope(endPoint, out var scope);
                try
                {
                    var args = await req.ResolveArgsAsync(action);
                    var result = action.Invoke(service, args);
                    await res.WriteAsync(result);
                }
                catch (Exception ex)
                {
                    var error = ex.HandleError(res);
                    await res.WriteAsJsonAsync(error);
                }
                finally
                {
                    scope?.Dispose();
                }
            }).WithOpenApi(o => o.ResolveOperations(apiName, action))
            .WithMetadata(new EndpointMethodMetadata(action))
            .Produces(StatusCodes.Status200OK, action.GetReturnType());

            if (enableAuth)
                api.RequireAuthorization();
        }
        /// <summary>
        /// Maps a DELETE API endpoint to the specified route with the provided action method.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="route"></param>
        /// <param name="endPoint"></param>
        /// <param name="action"></param>
        /// <param name="apiName"></param>
        public static void MapDeleteApi(this WebApplication app, string route, Type endPoint, MethodInfo action, string apiName)
        {
            var enableAuth = action.EnableAuthorization(endPoint);
            var api = app.MapDelete(route, async (HttpRequest req, HttpResponse res) =>
            {
                var authEx = action.ValidateAuthorization(endPoint, req.HttpContext.User);
                if (authEx != null)
                {
                    var error = authEx.HandleError(res);
                    await res.WriteAsJsonAsync(error);
                    return;
                }
                var service = app.Services.GetServiceFromSafeScope(endPoint, out var scope);
                try
                {
                    var args = await req.ResolveArgsAsync(action);
                    var result = action.Invoke(service, args);
                    await res.WriteAsync(result);
                }
                catch (Exception ex)
                {
                    var error = ex.HandleError(res);
                    await res.WriteAsJsonAsync(error);
                }
                finally
                {
                    scope?.Dispose();
                }
            }).WithOpenApi(o => o.ResolveOperations(apiName, action))
            .WithMetadata(new EndpointMethodMetadata(action))
            .Produces(StatusCodes.Status200OK, action.GetReturnType());

            if (enableAuth)
                api.RequireAuthorization();
        }
    }
}
