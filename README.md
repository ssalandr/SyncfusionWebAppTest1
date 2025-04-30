# SyncfusionWebAppTest1

ASP.NET Core Web Application with Syncfusion Components

## Project Structure

```
SyncfusionWebAppTest1/
├── Controllers/         # MVC Controllers
├── Data/               # Database context and migrations
├── Models/             # Data models and ViewModels
├── Pages/              # Razor Pages
├── Services/           # Business logic and services
├── wwwroot/            # Static files (CSS, JS, images)
└── Properties/         # Project properties and launch settings
```

## Development Guidelines

### ASP.NET Core Best Practices

1. **Controllers**
   - Use attribute routing
   - Implement proper error handling
   - Use async/await for I/O operations
   - Follow RESTful conventions

2. **Models**
   - Use data annotations for validation
   - Implement proper model binding
   - Use ViewModels for complex views
   - Follow naming conventions

3. **Services**
   - Implement dependency injection
   - Use interfaces for better testability
   - Follow single responsibility principle
   - Implement proper error handling

4. **Data Access**
   - Use Entity Framework Core
   - Implement repository pattern
   - Use async operations
   - Implement proper connection management

### Security

- Use HTTPS in production
- Implement proper authentication
- Use anti-forgery tokens
- Validate all inputs
- Follow OWASP guidelines

### Performance

- Use caching appropriately
- Optimize database queries
- Implement proper logging
- Use async operations for I/O-bound work

## Getting Started

1. **Prerequisites**
   - .NET 7.0 SDK or later
   - Visual Studio 2022 or VS Code
   - SQL Server (for database)

2. **Installation**
   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```

3. **Configuration**
   - Update connection strings in `appsettings.json`
   - Configure Syncfusion license
   - Set up environment variables

## Documentation

### Official Resources
- [ASP.NET Core Documentation](https://learn.microsoft.com/en-us/aspnet/core/)
- [Syncfusion Documentation](https://help.syncfusion.com/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)

### Project-Specific Documentation
- [API Documentation](./Documentation/API.md)
- [Database Schema](./Documentation/Database.md)
- [Deployment Guide](./Documentation/Deployment.md)

## Contributing

1. Fork the repository
2. Create your feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details. 