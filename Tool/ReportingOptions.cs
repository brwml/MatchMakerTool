namespace MatchMaker.Tool;

#pragma warning disable CS8618

using CommandLine;

/// <summary>
/// Defines the <see cref="ReportingOptions" />
/// </summary>
[Verb("reporting", HelpText = "Generate a report from the results XML files")]
internal class ReportingOptions : BaseOptions
{
    /// <summary>
    /// Defines the default ranking procedure
    /// </summary>
    public const string DefaultRankingProcedure = "whse";

    /// <summary>
    /// Gets or sets the number of alternate teams
    /// </summary>
    [Option('m', Default = 0, HelpText = "The number of alternate tournament teams to create.")]
    public int NumberOfAlternateTeams { get; set; }

    /// <summary>
    /// Gets or sets the number of tournament teams
    /// </summary>
    [Option('t', Default = 0, HelpText = "The number of teams in the tournament.")]
    public int NumberOfTournamentTeams { get; set; }

    /// <summary>
    /// Gets or sets the output folder
    /// </summary>
    [Option('o', Default = ".", HelpText = "Output folder for the report.")]
    public string OutputFolder { get; set; }

    /// <summary>
    /// Gets or sets the output format
    /// </summary>
    [Option('f', Default = OutputFormat.All, HelpText = "Output format for the report. Possible values are Excel, Html, Pdf, Rtf, and Xml.")]
    public OutputFormat OutputFormat { get; set; }

    /// <summary>
    /// Gets or sets the ranking procedure
    /// </summary>
    [Option('r', Default = DefaultRankingProcedure, HelpText = "The ranking operations and sequence. Each character represents a ranking operation. Possible operations include 'w' for winning percentage, 'l' for total losses, 'h' for head-to-head competition, 's' for average score, and 'e' for average errors.")]
    public string RankingProcedure { get; set; }

    /// <summary>
    /// Gets or sets the source folder
    /// </summary>
    [Option('s', Required = true, HelpText = "Source folder with the score files.")]
    public string SourceFolder { get; set; }

    /// <summary>
    /// Gets or sets the name of the tournament.
    /// </summary>
    [Option('n', HelpText = "The name of the tournament")]
    public string Name { get; set; }
}
