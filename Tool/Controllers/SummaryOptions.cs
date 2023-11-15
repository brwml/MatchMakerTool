namespace MatchMaker.Tool.Controllers;

using System.Collections.Generic;

using CommandLine;

#pragma warning disable CA1812 // The class is instantiate by the command line parser.

/// <summary>
/// Defines the <see cref="SummaryOptions" />
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SummaryOptions"/> class.
/// </remarks>
/// <param name="inputPaths">The input paths.</param>
/// <param name="outputPath">The output path.</param>
/// <param name="verbose">If set to <c>true</c>, then emit verbose output.</param>
[Verb("summary", HelpText = "Generate a summary report for multiple events")]
internal class SummaryOptions(IEnumerable<string> inputPaths, string outputPath, bool verbose) : BaseOptions(verbose)
{
    /// <summary>
    /// Gets or sets the input paths
    /// </summary>
    [Option('i', Required = true, HelpText = "The list of input paths", Separator = ',')]
    public IEnumerable<string> InputPaths { get; } = inputPaths;

    /// <summary>
    /// Gets or sets the output path
    /// </summary>
    [Option('o', Required = true, HelpText = "The output path")]
    public string OutputPath { get; } = outputPath;
}
