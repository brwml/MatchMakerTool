namespace MatchMaker.Tool.Controllers;

using CommandLine;

#pragma warning disable CA1812 // The class is instantiate by the command line parser.

/// <summary>
/// Defines the <see cref="ReportingOptions" />
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ReportingOptions"/> class.
/// </remarks>
/// <param name="outputFolder">The output folder.</param>
/// <param name="outputFormat">The output format.</param>
/// <param name="rankingProcedure">The ranking procedure.</param>
/// <param name="sourceFolder">The source folder of the result files.</param>
/// <param name="name">The name of the tournament.</param>
/// <param name="verbose">If set to <c>true</c>, then emit verbose output.</param>
/// 
/// 
[Verb("report", HelpText = "Generate a report from the results XML files")]
internal class ReportingOptions(
    string outputFolder,
    OutputFormat outputFormat,
    string rankingProcedure,
    string sourceFolder,
    string name,
    bool verbose) : BaseOptions(verbose)
{
    /// <summary>
    /// Defines the default ranking procedure
    /// </summary>
    public const string DefaultRankingProcedure = "whse";

    /// <summary>
    /// Gets or sets the output folder
    /// </summary>
    [Option('o', Default = ".", HelpText = "Output folder for the report.")]
    public string OutputFolder { get; } = outputFolder ?? ".";

    /// <summary>
    /// Gets or sets the output format
    /// </summary>
    [Option('f', Default = OutputFormat.All, HelpText = "Output format for the report. Possible values are Excel, Html, Pdf, Rtf, and Xml.")]
    public OutputFormat OutputFormat { get; } = outputFormat;

    /// <summary>
    /// Gets or sets the ranking procedure
    /// </summary>
    [Option('r', Default = DefaultRankingProcedure, HelpText = "The ranking operations and sequence. Each character represents a ranking operation. Possible operations include 'w' for winning percentage, 'l' for total losses, 'h' for head-to-head competition, 's' for average score, and 'e' for average errors.")]
    public string RankingProcedure { get; } = rankingProcedure ?? DefaultRankingProcedure;

    /// <summary>
    /// Gets or sets the source folder
    /// </summary>
    [Option('s', Required = true, HelpText = "Source folder with the score files.")]
    public string SourceFolder { get; } = sourceFolder ?? ".";

    /// <summary>
    /// Gets or sets the name of the tournament.
    /// </summary>
    [Option('n', HelpText = "The name of the tournament")]
    public string Name { get; } = name ?? string.Empty;
}
