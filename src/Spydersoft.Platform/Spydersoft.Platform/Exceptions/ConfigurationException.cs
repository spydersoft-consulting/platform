using System;

namespace Spydersoft.Platform.Exceptions;

/// <summary>
/// Exception thrown when there is a configuration error in the application.
/// This exception indicates issues with application settings, missing configuration, or invalid configuration values.
/// </summary>
public class ConfigurationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the configuration error.</param>
    public ConfigurationException(string message) : base(message)
    {
    }
}
