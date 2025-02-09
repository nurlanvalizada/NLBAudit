# NLBAudit - Comprehensive and Extensible Auditing Library

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Overview

NLBAudit is a comprehensive and extensible auditing library for .NET applications. It provides functionalities to log and track method executions, including details such as execution time, duration, input parameters, return values, and exceptions. The library is designed to be easily integrated with Entity Framework Core and ASP.NET Core applications.

## Features

- Auditing of method executions
- Logging of user information, IP address, and browser details
- Integration with Entity Framework Core
- Customizable audit log storage
- Exception tracking

## Getting Started

### Prerequisites

- .NET 8.0

### Installation

To install NLBAudit, add the following NuGet package to your project:

```sh
dotnet add package NLBAudit.Core
```
It is framework independent package and will write audit records to logs and will use test session and test client. It is useful for testing purposes

If you want to integrate NLBAudit to ASP.NET Core API project you need to install to additional packages
```sh
dotnet add package NLBAudit.AspNetCore
dotnet add package NLBAudit.Store.EfCore
```

### Entity Framework Core Configuration

1. **Define your `DbContext`**:

    ```csharp
    public class MyDbContext(DbContextOptions<MyDbContext> options) : DbContext(options), IAuditedContext<int>
    {
        public DbSet<AuditLogEntity<int>> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureEfCoreAuditing<int>();
            modelBuilder.Entity<AuditLogEntity<int>>().ToTable("AuditLogs");
        }
    }
    ```

2. **Configure your `DbContext` in `Program.cs`**:

    ```csharp
    builder.Services.AddDbContext<MyDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnectionString"));
    });

    builder.Services.ConfigureAuditingEfCoreStore<int, MyDbContext>();
    ```

### ASP.NET Core Integration

1. **Register the auditing services in `Program.cs`**:

    ```csharp
    builder.Services.ConfigureAspNetCoreAuditing<int>(config =>
    {
        config.IsEnabled = true;
        config.IsEnabledForAnonymousUsers = true;
        config.SaveReturnValues = true;
    });
    ```

2. **Use the auditing filter for Web APIs**:

    ```csharp
    builder.Services.AddControllers(options =>
    {
        options.AddAuditingFilter<int>();
    });
    ```
3. **Use the endpoint filter for Minimal APIs**:
   ```csharp
    app.MapGet("/weatherforecast", () =>
           {
               var forecast = Enumerable.Range(1, 5).Select(index =>
                                            new WeatherForecast
                                            (
                                                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                                                Random.Shared.Next(-20, 55),
                                                summaries[Random.Shared.Next(summaries.Length)]
                                            ))
                                        .ToArray();
               return forecast;
           })
           .AddEndpointFilter<MinimalApiEndpointAuditFilter<int>>()
   ```

### Usage

This is all you need :) 

### Customization
You can customize the audit logging by extending the AuditInfo class or implementing your own storage mechanism by extending IAuditingStore.  

### Contributing
Contributions are welcome! Please open an issue or submit a pull request on GitHub.  

### License
This project is licensed under the [MIT LICENSE](LICENSE). You are free to use it in both commercial and open-source software.
