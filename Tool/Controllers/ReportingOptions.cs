namespace MatchMaker.Tool.Controllers;

using CommandLine;

#pragma warning disable CA1812 // The class is instantiate by the command line parser.

/// <summary>
/// Defines the <see cref="ReportingOptions" /> for the reporting controller.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ReportingOptions"/> class. The parameters must appear in the
/// same order they appear in the class.
/// </remarks>
/// <param name="outputFolder">The output folder.</param>
/// <param name="outputFormat">The output format.</param>
/// <param name="rankingProcedure">The ranking procedure, or <see langword="null"/> to use the default for the schedule's tournament type.</param>
/// <param name="sourceFolder">The source folder of the result files.</param>
/// <param name="name">The name of the tournament.</param>
/// <param name="numberOfTournamentTeams">The number of teams that advance to an elimination tournament.</param>
/// <param name="numberOfAlternateTeams">The number of alternate teams to create from the remaining quizzers.</param>
/// <param name="numberOfAlternateRooms">The number of rooms available for the alternate teams round-robin tournament.</param>
/// <param name="verbose">If set to <c>true</c>, then emit verbose output.</param>
[Verb("report", HelpText = "Generate a report from the results XML files")]
internal class ReportingOptions(
    string outputFolder,
    OutputFormat outputFormat,
    string? rankingProcedure,
    string sourceFolder,
    string name,
    int numberOfTournamentTeams,
    int numberOfAlternateTeams,
    int numberOfAlternateRooms,
    bool verbose) : BaseOptions(verbose)
{
    /// <summary>
    /// Defines the default ranking procedure, used only by the <c>summary</c> verb
    /// (<see cref="SummaryController"/>), which has no associated <see cref="MatchMaker.Models.TournamentType"/>
    /// to derive a default from.
    /// </summary>
    public const string DefaultRankingProcedure = "whse";

    /// <summary>
    /// Gets or sets the output folder. If not specified, the default value is ".", or the current folder.
    /// </summary>
    [Option('o', Default = ".", HelpText = "Output folder for the report.")]
    public string OutputFolder { get; } = outputFolder ?? ".";

    /// <summary>
    /// Gets or sets the output format. The default is to export all formats.
    /// </summary>
    [Option('f', Default = OutputFormat.All, HelpText = "Output format for the report. Possible values are Excel, Html, Pdf, Rtf, and Xml.")]
    public OutputFormat OutputFormat { get; } = outputFormat;

    /// <summary>
    /// Gets or sets the ranking procedure. When not specified, the default ranking policy chain
    /// for the schedule's <see cref="MatchMaker.Models.TournamentType"/> is used (see
    /// <see cref="MatchMaker.Reporting.Policies.TeamRankingPolicyFactory.GetDefaultPolicies"/>).
    /// </summary>
    [Option('r', Required = false, HelpText = "The ranking operations and sequence. Each character represents a ranking operation. Possible operations include 'w' for winning percentage, 'l' for total losses, 'h' for head-to-head competition, 's' for average score, and 'e' for average errors. When not specified, the default ranking for the schedule's tournament type is used.")]
    public string? RankingProcedure { get; } = rankingProcedure;

    /// <summary>
    /// Gets or sets the source folder.
    /// </summary>
    [Option('s', Required = true, HelpText = "Source folder with the score files.")]
    public string SourceFolder { get; } = sourceFolder ?? ".";

    /// <summary>
    /// Gets or sets the name of the tournament.
    /// </summary>
    [Option('n', HelpText = "The name of the tournament")]
    public string Name { get; } = name ?? string.Empty;

    /// <summary>
    /// Gets or sets the number of top-placing teams from the round-robin phase that advance
    /// unchanged to a single-elimination tournament. When combined with a positive
    /// <see cref="NumberOfAlternateTeams"/>, the elimination tournament and alternate teams
    /// artifacts are generated.
    /// </summary>
    [Option('t', Default = 0, HelpText = "The number of top-placing teams that advance to an elimination tournament.")]
    public int NumberOfTournamentTeams { get; } = numberOfTournamentTeams;

    /// <summary>
    /// Gets or sets the number of new, evenly balanced teams to create from the quizzers who did
    /// not qualify for the elimination tournament, for a round-robin consolation tournament.
    /// </summary>
    [Option('m', Default = 0, HelpText = "The number of alternate teams to create from the remaining quizzers.")]
    public int NumberOfAlternateTeams { get; } = numberOfAlternateTeams;

    /// <summary>
    /// Gets or sets the number of rooms available for the alternate teams round-robin tournament.
    /// When not specified (or less than 1), every match in a round is scheduled to run
    /// simultaneously, one match per alternate team pair.
    /// </summary>
    [Option('a', Default = 0, HelpText = "The number of rooms available for the alternate teams round-robin tournament.")]
    public int NumberOfAlternateRooms { get; } = numberOfAlternateRooms;
}
