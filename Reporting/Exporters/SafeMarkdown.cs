namespace MatchMaker.Reporting.Exporters;

/// <summary>
/// Wrapper class for markdown text that needs escaping of special characters.
/// </summary>
/// <remarks>
/// Initializes a new instance of the SafeMarkdown class.
/// </remarks>
/// <param name="value">The text to be escaped when rendered</param>
public class SafeMarkdown(string value)
{
    /// <summary>
    /// Gets the raw value without escaping.
    /// </summary>
    public string Value { get; } = value ?? string.Empty;

    /// <summary>
    /// Returns the raw value.
    /// </summary>
    public override string ToString()
    {
        return this.Value;
    }
}
