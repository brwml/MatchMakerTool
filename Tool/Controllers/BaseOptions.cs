namespace MatchMaker.Tool.Controllers;

using CommandLine;

/// <summary>
/// Defines the <see cref="BaseOptions" />
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BaseOptions"/> class.
/// </remarks>
/// <param name="verbose">If set to <c>true</c>, then emit verbose output.</param>
internal class BaseOptions(bool verbose)
{
    /// <summary>
    /// Gets or sets a value indicating whether verbose output should be enabled
    /// </summary>
    [Option('v', HelpText = "Display verbose output", Default = false)]
    public bool Verbose { get; } = verbose;
}
