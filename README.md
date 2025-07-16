# SwiftApi

**SwiftApi** is a powerful .NET class library that transforms your interfaces into fully functional API endpoints — with **zero controller boilerplate**. Designed for speed, simplicity, and flexibility, SwiftApi enables you to build scalable APIs with minimal code and maximum control.

---

## 🚀 Key Features

- ✅ **Zero Controllers**  
  Automatically exposes your service interfaces as API endpoints — no need to write a single controller.

- ⚙️ **Unlimited Endpoints**  
  Add as many interfaces and methods as you like — SwiftApi handles the routing dynamically.

- 🔐 **Built-in Security**  
  Easily secure endpoints with support for various authentication schemes (Bearer, Basic, API Key, etc.).

- 📄 **Swagger Support**  
  Built-in Swagger/OpenAPI integration for instant, interactive API documentation.

- 🧩 **Endpoint Management**  
  Enable, disable, or configure individual endpoints via attributes or settings — without touching controllers.

- 🎯 **.NET 8+ Compatible**  
  Built on the latest .NET standards with full support for .NET 8 and future versions.

---

## ✅ Supported Actions

SwiftApi currently supports the following HTTP actions via method attributes:

- `GetAction` → HTTP GET  
- `PostAction` → HTTP POST  
- `PutAction` → HTTP PUT  
- `DeleteAction` → HTTP DELETE

---

## 🛠️ Getting Started

```bash
dotnet add package SwiftApi
```

### Basic Usage

1. Define your interface:
```csharp
[EndPoint("users")]
public interface IUserService
{
    [GetAction]
    Task<User?> GetUserByIdAsync([QueryParam] Guid id);
    [GetAction("get-users")]
    Task<List<User>> GetUsersAsync();
    [PostAction("create-users")]
    Task CreateUserAsync([BodyParam] User user);
    [PutAction("update-users")]
    Task UpdateUserAsync([RouteParam] Guid id, [BodyParam] User user);
    [DeleteAction("delete-users")]
    Task DeleteUserAsync([RouteParam] Guid id);
}
```

2. Define your schema:
```csharp
[SchemaModel]
public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    [Required]
    public string Email { get; set; }
}
```

3. Register and enable SwiftApi in your `Program.cs`:
```csharp
builder.Services.AddSwiftAPI(); // Register services
app.MapSwiftAPI();              // Map endpoints
```

4. Start your app and explore the auto-generated Swagger UI at `/swagger`.

---

## 🔐 Securing Endpoints

Use attributes or configuration to require authentication per endpoint or globally. SwiftApi supports:

- Bearer Tokens
- Basic Auth
- API Key Headers

### Implementing Authorization & Authentication

1. Define your Authorization Schema:
- Bearer Tokens
- Basic Auth
- API Key Headers

2. Add Authorization and Authentication to you interface
```csharp
[EndPoint("users")]
[SecureEndpoint(role: "Admin,Manager", policy: "read,write,edit,delete")]
public interface IUserService
{
    [GetAction("get-user-by-id")]
    Task<User?> GetUserByIdAsync([QueryParam] Guid id);
    [GetAction("get-users")]
    [OpenAction] //Allows unauthenticated access
    Task<List<User>> GetUsersAsync();
    [PostAction("create-users")]
    Task CreateUserAsync([BodyParam] User user);
    [PutAction("update-users")]
    Task UpdateUserAsync([RouteParam] Guid id, [BodyParam] User user);
    [DeleteAction("delete-users")]
    Task DeleteUserAsync([RouteParam] Guid id);
}
```

3. Update your swiftApi registration:
```csharp
builder.Services.AddSwiftAPI(o =>
{
    // Set the auth schema you are using, e.g., Basic, Bearer, etc.
    o.AuthScheme = AuthScheme.Basic; 
});
```

4. Start your app and explore the auto-generated Swagger UI with Auth at `/swagger`.

---

## 🙋‍♂️ Author

**Nawaf AL-Maqbali**  
📧 [LinkedIn](https://www.linkedin.com/in/nawaf-al-maqbali-6bb4a6227)
