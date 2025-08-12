# Dependency Injection Extensions
This library provides a simple way to register services for dependency injection by decorating classes with an attribute and calling a single extension method.

## DependencyInjectionAttribute
Decorate your service implementation classes with `DependencyInjectionAttribute` to specify the interface, lifetime, and optional rank for registration.

### Attribute Usage
````csharp
[DependencyInjection(typeof(IMyService), LifetimeOfService.Scoped, rank: 10)]
public class MyService : IMyService
{
	// Implementation
}
````
- `serviceInterface`: The interface type to register for this implementation.
- `serviceLifetime`: The lifetime for the service (Transient, Scoped, Singleton). Default is Transient.
- `rank`: Optional integer to control registration order (lower ranks are registered first). Default is 0.

### LifetimeOfService Enum
````csharp
public enum LifetimeOfService
{
	Transient = 1,
	Scoped = 2,
	Singleton = 3
}
````

## Registering Decorated Services

Call the extension method in your startup code to automatically register all decorated services from all loaded assemblies:

````csharp
builder.Services.AddSpydersoftDecoratedServices();
````

This will scan all loaded assemblies for classes decorated with `DependencyInjectionAttribute` and register them with the specified interface and lifetime. If multiple implementations are found for the same interface, the order of registration is determined by the `rank` property (lowest first).

## Example
````csharp
// Service interface
public interface IMyService { }

// Implementation 1
[DependencyInjection(typeof(IMyService), LifetimeOfService.Singleton, rank: 1)]
public class MySingletonService : IMyService { }

// Implementation 2
[DependencyInjection(typeof(IMyService), LifetimeOfService.Transient, rank: 2)]
public class MyTransientService : IMyService { }

// In Program.cs or Startup.cs
builder.Services.AddSpydersoftDecoratedServices();
````

## Notes

- Only classes decorated with `DependencyInjectionAttribute` will be registered.
- The attribute must specify the interface type to register.
- The extension method uses reflection to find and register all eligible services at startup.
- Registration order is determined by the `rank` property (lowest first).
