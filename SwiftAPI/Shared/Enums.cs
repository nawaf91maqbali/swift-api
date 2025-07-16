namespace SwiftAPI.Shared
{
    public enum ActionType
    {
        Get,
        Post,
        Put,
        Delete,
        Patch,
        Options,
        Head
    }

    public enum RegisterType
    {
        Scoped,
        Singleton,
        Transient
    }

    public enum ParamType
    {
        FromBody,
        FromQuery,
        FromRoute,
        FromHeader
    }

    public enum AuthScheme
    {
        None,
        Basic,
        Bearer,
        ApiKey
    }
}
