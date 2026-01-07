namespace Spydersoft.Platform.Hosting.Options;
/// <summary>
/// Class IdentityOptions.
/// </summary>
public class IdentityOptions
{
    /// <summary>
    /// The section name
    /// </summary>
    public const string SectionName = "Identity";

    /// <summary>
    /// Gets or sets a value indicating whether identity/authentication is enabled.
    /// When enabled, JWT Bearer authentication will be configured.
    /// </summary>
    /// <value>
    /// <c>true</c> if identity is enabled; otherwise, <c>false</c>. Default is <c>false</c>.
    /// </value>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Gets or sets the authority.
    /// </summary>
    /// <value>The authority.</value>
    public string? Authority { get; set; } = null;

    /// <summary>
    /// Gets or sets the name of the application.
    /// </summary>
    /// <value>The name of the application.</value>
    public string ApplicationName { get; set; } = "spydersoft-application";
}