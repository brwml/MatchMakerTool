namespace MatchMaker.Tool.Controllers;

using CommandLine;

/// <summary>
/// Defines the <see cref="BaseOptions" />
/// </summary>
internal class BaseOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether verbose output should be enabled
    /// </summary>
    [Option('v', HelpText = "Display verbose output", Default = false)]
    public bool Verbose
    {
        get; set;
    }
}
