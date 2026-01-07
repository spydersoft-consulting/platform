using System;

namespace Spydersoft.Platform.Attributes;

/// <summary>
/// Attribute for marking classes to be automatically registered in the dependency injection container.
/// Classes decorated with this attribute will be discovered and registered at application startup.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class)]
public class DependencyInjectionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DependencyInjectionAttribute"/> class.
    /// </summary>
    /// <param name="serviceInterface">The service interface type to register.</param>
    /// <param name="serviceLifetime">The service lifetime (Transient, Scoped, or Singleton). Default is Transient.</param>
    /// <param name="rank">The registration rank for ordering multiple implementations. Default is 0.</param>
    public DependencyInjectionAttribute(Type serviceInterface, LifetimeOfService serviceLifetime = LifetimeOfService.Transient, int rank = 0)
    {
        ServiceInterface = serviceInterface;
        Lifetime = serviceLifetime;
        Rank = rank;
    }

	/// <summary>
    /// Gets or sets the service interface.
    /// </summary>
    /// <value>The service interface.</value>
    public Type ServiceInterface { get; set; }
	/// <summary>
	/// Gets or sets the lifetime.
	/// </summary>
	/// <value>The lifetime.</value>
	public LifetimeOfService Lifetime { get; set; }
	/// <summary>
	/// Gets or sets the rank.
	/// </summary>
	/// <value>The rank.</value>
	public int Rank { get; set; }
}

/// <summary>
/// Enum LifetimeOfService
/// </summary>
public enum LifetimeOfService
{
	/// <summary>
	/// Transient objects are created every time they are requested
	/// </summary>
	Transient = 1,
	/// <summary>
	/// Scoped objects are created once for each request and reused within the context of that request.
	/// </summary>
	Scoped = 2,
	/// <summary>
	/// Singleton objects are created once for the lifetime of the application
	/// </summary>
	Singleton = 3
}