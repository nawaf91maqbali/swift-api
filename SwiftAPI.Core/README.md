# SwiftApi.Core

**SwiftApi.Core** is a lightweight and extensible core component library designed to power the [SwiftApi](https://github.com/nawaf91maqbali/swift-api.git) ecosystem. It provides shared types, and utility classes to support dynamic API generation in .NET applications.

## ✨ Features

- 🔧 Core types and base classes for SwiftApi
- 🧩 Attribute-based endpoint configuration
- 🧼 Clean and minimal dependencies
- ✅ Nullable and implicit usings enabled
- 🚀 Built with .NET 8

## 📦 Installation

Install from NuGet:

```bash
dotnet add package SwiftApi.Core
```

Or via the Package Manager Console:

```powershell
Install-Package SwiftApi.Core
```

## 🛠 Usage

```csharp
public interface IUserService
{
    [GetAction("users")]
    IEnumerable<UserDto> GetAllUsers();

    [PostAction("users")]
    UserDto CreateUser(UserDto user);
}
```

The above interface can be used by the SwiftApi runtime to generate endpoints dynamically.

## 📄 License

This project is licensed under the [MIT License](LICENSE).

## 🙋‍♂️ Author

**Nawaf AL-Maqbali**  
📧 [LinkedIn](https://www.linkedin.com/in/nawaf-al-maqbali-6bb4a6227)
