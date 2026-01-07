using Microsoft.AspNetCore.Authentication;

namespace SwiftAPI.Core.AuthHandlerOptions
{
    public class BasicAuthOption : AuthenticationSchemeOptions
    {
        public List<BasicAuthCridentional>? AuthCridentionals { get; set; }
    }

    public class BasicAuthCridentional
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public List<string> Roles { get; set; } = new();
        public BasicAuthCridentional(string username, string password) {
            Username = username;
            Password = password;
        }
    }
}
