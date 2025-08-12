using System;

namespace Spydersoft.Platform.Attributes;

/// <summary>
/// Class DependencyInjectionAttribute.
/// Implements the <see cref="Attribute" />
/// </summary>
/// <param name="ServiceInterface">The service interface.</param>
/// <param name="ServiceLifetime">The service lifetime.</param>
/// <param name="rank">The rank.</param>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class)]
public class DependencyInjectionAttribute : Attribute
{
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