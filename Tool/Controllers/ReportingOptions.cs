namespace MatchMaker.Tool.Controllers;

using Ardalis.GuardClauses;

using CommandLine;

#pragma warning disable CA1812 // The class is instantiate by the command line parser.

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
    /// Initializes a new instance of the <see cref="ReportingOptions"/> class.
    /// </summary>
    /// <param name="numberOfAlternateTeams">The number of alternate teams.</param>
    /// <param name="numberOfTournamentTeams">The number of tournament teams.</param>
    /// <param name="outputFolder">The output folder.</param>
    /// <param name="outputFormat">The output format.</param>
    /// <param name="rankingProcedure">The ranking procedure.</param>
    /// <param name="sourceFolder">The source folder of the result files.</param>
    /// <param name="name">The name of the tournament.</param>
    /// <param name="verbose">If set to <c>true</c>, then emit verbose output.</param>
    public ReportingOptions(
        int numberOfAlternateTeams,
        int numberOfTournamentTeams,
        string outputFolder,
        OutputFormat outputFormat,
        string rankingProcedure,
        string sourceFolder,
        string name,
        bool verbose)
        : base(verbose)
    {
        this.NumberOfAlternateTeams = numberOfAlternateTeams;
        this.NumberOfTournamentTeams = numberOfTournamentTeams;
        this.OutputFolder = Guard.Against.NullOrWhiteSpace(outputFolder);
        this.OutputFormat = outputFormat;
        this.RankingProcedure = Guard.Against.NullOrWhiteSpace(rankingProcedure);
        this.SourceFolder = Guard.Against.NullOrWhiteSpace(sourceFolder);
        this.Name = name ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets the number of alternate teams
    /// </summary>
    [Option('m', Default = 0, HelpText = "The number of alternate tournament teams to create.")]
    public int NumberOfAlternateTeams
    {
        get;
    }

    /// <summary>
    /// Gets or sets the number of tournament teams
    /// </summary>
    [Option('t', Default = 0, HelpText = "The number of teams in the tournament.")]
    public int NumberOfTournamentTeams
    {
        get;
    }

    /// <summary>
    /// Gets or sets the output folder
    /// </summary>
    [Option('o', Default = ".", HelpText = "Output folder for the report.")]
    public string OutputFolder
    {
        get;
    }

    /// <summary>
    /// Gets or sets the output format
    /// </summary>
    [Option('f', Default = OutputFormat.All, HelpText = "Output format for the report. Possible values are Excel, Html, Pdf, Rtf, and Xml.")]
    public OutputFormat OutputFormat
    {
        get;
    }

    /// <summary>
    /// Gets or sets the ranking procedure
    /// </summary>
    [Option('r', Default = DefaultRankingProcedure, HelpText = "The ranking operations and sequence. Each character represents a ranking operation. Possible operations include 'w' for winning percentage, 'l' for total losses, 'h' for head-to-head competition, 's' for average score, and 'e' for average errors.")]
    public string RankingProcedure
    {
        get;
    }

    /// <summary>
    /// Gets or sets the source folder
    /// </summary>
    [Option('s', Required = true, HelpText = "Source folder with the score files.")]
    public string SourceFolder
    {
        get;
    }

    /// <summary>
    /// Gets or sets the name of the tournament.
    /// </summary>
    [Option('n', HelpText = "The name of the tournament")]
    public string Name
    {
        get;
    }
}
