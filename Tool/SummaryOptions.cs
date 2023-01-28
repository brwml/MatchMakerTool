namespace MatchMaker.Tool;

using System.Collections.Generic;

using CommandLine;

#pragma warning disable CS8618 // Nullable types are either required or have default values.
#pragma warning disable CA1812 // The class is instantiate by the command line parser.

/// <summary>
/// Defines the <see cref="SummaryOptions" />
/// </summary>
[Verb("summary", HelpText = "Generate a summary report for multiple events")]
internal class SummaryOptions : BaseOptions
{
    /// <summary>
    /// Gets or sets the input paths
    /// </summary>
    [Option('i', Required = true, HelpText = "The list of input paths", Separator = ',')]
    public IEnumerable<string> InputPaths
    {
        get; set;
    }

    /// <summary>
    /// Gets or sets the output path
    /// </summary>
    [Option('o', Required = true, HelpText = "The output path")]
    public string OutputPath
    {
        get; set;
    }
}
