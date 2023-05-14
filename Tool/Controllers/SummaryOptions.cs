namespace MatchMaker.Tool.Controllers;

using System.Collections.Generic;

using Ardalis.GuardClauses;

using CommandLine;

#pragma warning disable CA1812 // The class is instantiate by the command line parser.

/// <summary>
/// Defines the <see cref="SummaryOptions" />
/// </summary>
[Verb("summary", HelpText = "Generate a summary report for multiple events")]
internal class SummaryOptions : BaseOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SummaryOptions"/> class.
    /// </summary>
    /// <param name="inputPaths">The input paths.</param>
    /// <param name="outputPath">The output path.</param>
    /// <param name="verbose">If set to <c>true</c>, then emit verbose output.</param>
    public SummaryOptions(IEnumerable<string> inputPaths, string outputPath, bool verbose) : base(verbose)
    {
        this.InputPaths = inputPaths;
        this.OutputPath = outputPath;
    }

    /// <summary>
    /// Gets or sets the input paths
    /// </summary>
    [Option('i', Required = true, HelpText = "The list of input paths", Separator = ',')]
    public IEnumerable<string> InputPaths
    {
        get;
    }

    /// <summary>
    /// Gets or sets the output path
    /// </summary>
    [Option('o', Required = true, HelpText = "The output path")]
    public string OutputPath
    {
        get;
    }
}
