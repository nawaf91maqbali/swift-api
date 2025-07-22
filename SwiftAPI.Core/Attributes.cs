using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftAPI.Core
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class EndPointAttribute : Attribute
    {
        public string? Name { get; }
        public EndPointAttribute(string? name = null) => Name = name;
    }

    #region Parameter Attributes
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParamAttribute : Attribute
    {
        public ParamType Type { get; }
        public ParamAttribute(ParamType type = ParamType.FromQuery) => Type = type;
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class QueryParamAttribute : ParamAttribute
    {
        public QueryParamAttribute() : base(ParamType.FromQuery) { }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class BodyParamAttribute : ParamAttribute
    {
        public BodyParamAttribute() : base(ParamType.FromBody) { }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class RouteParamAttribute : ParamAttribute
    {
        public RouteParamAttribute() : base(ParamType.FromRoute) { }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class HeaderParamAttribute : ParamAttribute
    {
        public HeaderParamAttribute() : base(ParamType.FromHeader) { }
    }
    #endregion

    #region Action Attributes
    [AttributeUsage(AttributeTargets.Method)]
    public class ActionAttribute : Attribute
    {
        public string Name { get; }
        public ActionType Action { get; }
        public ActionAttribute(string name = null, ActionType action = ActionType.Get)
        {
            Name = name;
            Action = action;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GetActionAttribute : ActionAttribute
    {
        public bool EnableCache { get; }
        public int CacheDuration { get; }
        public GetActionAttribute(string name = null, bool enableCache = false, int cacheDuration = 1)
            : base(name, ActionType.Get)
        {
            EnableCache = enableCache;
            CacheDuration = cacheDuration;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PostActionAttribute : ActionAttribute
    {
        public string ContentType { get; }
        public PostActionAttribute(string name = null, string contentType = "application-json") : base(name, ActionType.Post) { 
            ContentType = contentType;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PutActionAttribute : ActionAttribute
    {
        public PutActionAttribute(string name = null) : base(name, ActionType.Put) { }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class DeleteActionAttribute : ActionAttribute
    {
        public DeleteActionAttribute(string name = null) : base(name, ActionType.Delete) { }
    }
    #endregion

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class SecureEndpointAttribute : Attribute
    {
        public string? Role { get; }
        public SecureEndpointAttribute(string? role = null)
        {
            Role = role;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class SecureActionAttribute : Attribute
    {
        public string? Policy { get; }
        public SecureActionAttribute(string? policy = null)
        {
            Policy = policy;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class OpenActionAttribute : Attribute
    {
        public bool OpenToPublic { get; }
        public OpenActionAttribute(bool openToPublic = true)
        {
            OpenToPublic = openToPublic;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class SchemaModelAttribute : Attribute
    {
        public SchemaModelAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ModelEndPointAttribute : Attribute
    {
        public string? Name { get; }
        public Type Interface { get; }
        public Type Implementation { get; }

        public ModelEndPointAttribute(Type @interface, Type @implementation, string? name = null)
        {
            Interface = @interface;
            Implementation = @implementation;
            Name = name;
        }
    }
}
