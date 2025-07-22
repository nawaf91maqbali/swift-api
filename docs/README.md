# SwiftApi

**SwiftApi** is a powerful .NET class library that transforms your interfaces — or even your model classes — into fully functional API endpoints with **zero controller boilerplate**. Designed for speed, simplicity, and flexibility, SwiftApi enables you to build scalable APIs with minimal code and maximum control.

---

## 🚀 Key Features

- ✅ **Zero Controllers**  
  Automatically exposes your service interfaces or models as API endpoints — no need to write a single controller.

- ⚙️ **Unlimited Endpoints**  
  Add as many interfaces or model classes as you like — SwiftApi handles the routing dynamically.

- 🔐 **Built-in Security**  
  Easily secure endpoints with support for various authentication schemes (Bearer, Basic, API Key, etc.).

- 📄 **Swagger Support**  
  Built-in Swagger/OpenAPI integration for instant, interactive API documentation.

- 🧩 **Endpoint Management**  
  Enable, disable, or configure individual endpoints via attributes or settings — without touching controllers.

- 💾 **Response Caching**  
  Built-in support for caching GET responses for improved performance and scalability.

- 🧱 **Model-Based API Generation**  
  Automatically generate basic CRUD endpoints directly from model classes.

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

### 🔧 Basic Interface Usage

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

3. Register SwiftApi in `Program.cs`:
```csharp
builder.Services.AddSwiftAPI();
app.MapSwiftAPI();
```

---

## 🧱 Model-Based Endpoint Generation (Interface Binding Required)

SwiftApi supports model-based endpoint generation by specifying the interface and its implementation directly on the model.

### 🔁 Example with Generic Interface

```csharp
[ModelEndPoint(typeof(IGenericService<User>), typeof(GenericService<User>))]
[SecureEndpoint]
public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    [Required]
    public string Email { get; set; }
}
```

### 🔁 Example with Specific Interface

```csharp
[ModelEndPoint(typeof(IPaymentService), typeof(PaymentService))]
public class Payment
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? Status { get; set; }
    public string? PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    public string? Currency { get; set; }
}
```

Each model will generate full CRUD endpoints via the specified service interface and implementation.

---

## 🔐 Securing Endpoints

Use attributes or configuration to secure endpoints per-method or globally.

### 🔐 Securing Interface EndPoints

```csharp
[EndPoint("users")]
[SecureEndpoint(role: "Admin,Manager")]
public interface IUserService
{
    [GetAction("get-user-by-id")]
    Task<User?> GetUserByIdAsync([QueryParam] Guid id);

    [GetAction("get-users")]
    [OpenAction] // Allows unauthenticated access
    Task<List<User>> GetUsersAsync();

    [PostAction("create-users")]
    Task CreateUserAsync([BodyParam] User user);

    [PutAction("update-users")]
    Task UpdateUserAsync([RouteParam] Guid id, [BodyParam] User user);

    [DeleteAction("delete-users")]
    [SecureAction(policy: "delete")] //optional: specify the policy of the action
    Task DeleteUserAsync([RouteParam] Guid id);
}
```

### 🔐 Securing Model EndPoints

```csharp
[ModelEndPoint(typeof(IGenericService<User>), 
    typeof(GenericService<User>))]
[SecureEndpoint(role: "Admin,Manager")]
public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    [Required]
    public string Email { get; set; }
}
```

Update your SwiftApi registration:

```csharp
builder.Services.AddSwiftAPI(o =>
{
    o.AuthScheme = AuthScheme.Basic; // Options: Bearer, Basic, ApiKey
});
```

---

## ⚡ Response Caching

Enable response caching for GET methods using the enableCache and cacheDuration variables in the [GetAction()] attribute.

```csharp
[GetAction(enableCache: true, cacheDuration: 5)] // default false, default duration 1 min
Task<List<User>> GetUsersAsync();
```

You can configure global cache behavior in `Program.cs` if needed.

---

## 🧪 Swagger Integration

Once your app is running, visit:

```
/swagger
```

To see your auto-generated, interactive documentation.

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

## 🙋‍♂️ Author

**Nawaf AL-Maqbali**  
📧 [LinkedIn](https://www.linkedin.com/in/nawaf-al-maqbali-6bb4a6227)
