namespace SwiftAPI.Shared
{
    /// <summary>
    /// Options class for configuring SwiftAPI behavior, such as authentication scheme and API key name.
    /// </summary>
    public class SwiftApiOptions
    {
        public AuthScheme AuthScheme { get; set; }
        public string ApiKeyName { get; set; } = "X-API-KEY";
    }
}
