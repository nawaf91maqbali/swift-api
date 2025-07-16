namespace SwiftAPI.Shared
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
        public GetActionAttribute(string name = null) : base(name, ActionType.Get) { }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PostActionAttribute : ActionAttribute
    {
        public PostActionAttribute(string name = null) : base(name, ActionType.Post) { }
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

    [AttributeUsage(AttributeTargets.Interface)]
    public class SecureEndpointAttribute : Attribute
    {
        public string? Policy { get; }
        public string? Role { get; }
        public SecureEndpointAttribute(string? policy = null, string? role = null)
        {
            Policy = policy;
            Role = role;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class SecureActionAttribute : Attribute
    {
        public string? Policy { get; }
        public string? Role { get; }
        public SecureActionAttribute(string? policy = null, string? role = null)
        {
            Policy = policy;
            Role = role;
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
}
