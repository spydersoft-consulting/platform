using System;

namespace Spydersoft.Platform.Attributes;

/// <summary>
/// Attribute for marking options classes to be automatically configured from application settings.
/// Classes decorated with this attribute will be bound to the specified configuration section at startup.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class InjectOptionsAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InjectOptionsAttribute"/> class.
    /// </summary>
    /// <param name="sectionName">The configuration section name to bind to this options class.</param>
    /// <param name="tags">Optional comma-separated tags for filtering which options to include.</param>
    public InjectOptionsAttribute(string sectionName, string tags = "")
    {
        SectionName = sectionName;
        RawTags = tags;
        Tags = tags.Split(',', StringSplitOptions.RemoveEmptyEntries);
    }
    
    /// <summary>
    /// Gets the configuration section name.
    /// </summary>
    /// <value>
    /// The section name in the configuration file.
    /// </value>
    public string SectionName { get; }

    /// <summary>
    /// Gets the raw comma-separated tags string.
    /// </summary>
    /// <value>
    /// The raw tags string.
    /// </value>
    public string RawTags { get; }

    /// <summary>
    /// Gets the parsed array of tags.
    /// </summary>
    /// <value>
    /// An array of tag strings for filtering.
    /// </value>
    public string[] Tags { get; }
}
