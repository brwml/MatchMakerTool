namespace MatchMaker.Tool.Controllers;

using CommandLine;

using MatchMaker.Models;

#pragma warning disable CA1812 // The class is instantiate by the command line parser.

/// <summary>
/// Defines the <see cref="ScheduleOptions" /> for the scheduling controller.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ScheduleOptions"/> class. The parameters must appear in the same
/// order they appear in the class.
/// </remarks>
/// <param name="inputSchedulePath">The source schedule file path.</param>
/// <param name="outputFolderPath">The output folder path.</param>
/// <param name="outputFormat">The output format.</param>
/// <param name="rooms">The number of rooms available.</param>
/// <param name="tournamentType">Type of the tournament.</param>
/// <param name="verbose">If set to <c>true</c>, then emit verbose output.</param>
[Verb("schedule", HelpText = "Generates the initial state of a tournament schedule")]
internal class ScheduleOptions(
    string inputSchedulePath,
    string outputFolderPath,
    OutputFormat outputFormat,
    int rooms,
    TournamentType tournamentType,
    bool verbose) : BaseOptions(verbose)
{
    /// <summary>
    /// Gets the source schedule file path.
    /// </summary>
    [Option('i', Required = true, HelpText = "The source schedule file path")]
    public string InputSchedulePath { get; } = inputSchedulePath;

    /// <summary>
    /// Gets the output folder path.
    /// </summary>
    [Option('o', Required = true, HelpText = "The output folder path")]
    public string OutputFolderPath { get; } = outputFolderPath;

    /// <summary>
    /// Gets the output formats. XML is always produced. Supported values: Html, Markdown, Pdf, Rtf, Xml.
    /// </summary>
    [Option('f', Default = OutputFormat.Html | OutputFormat.Pdf | OutputFormat.Rtf | OutputFormat.Xml | OutputFormat.Markdown, HelpText = "Output format for the schedule. Possible values are Html, Markdown, Pdf, Rtf, and Xml.")]
    public OutputFormat OutputFormat { get; } = outputFormat;

    /// <summary>
    /// Gets the number of rooms available.
    /// </summary>
    /// <remarks>
    /// Only used when <see cref="TournamentType"/> is <see cref="TournamentType.RoundRobin"/>.
    /// </remarks>
    [Option('n', Required = false, HelpText = "The number of rooms available. Only used for the RoundRobin tournament type.")]
    public int Rooms { get; } = rooms;

    /// <summary>
    /// Gets the tournament type.
    /// </summary>
    /// <remarks>
    /// For <see cref="TournamentType.RoundRobin"/>, the entire schedule is generated. For every
    /// other tournament type, only the initial round is generated; a separate application is
    /// expected to generate subsequent rounds as results become available (see
    /// <c>MatchMaker.Scheduling.Tournaments.TournamentRoundGeneratorFactory</c>).
    /// </remarks>
    [Option('t', Default = TournamentType.RoundRobin, Required = false, HelpText = "The tournament type. Possible values are RoundRobin, SingleElimination, DoubleElimination, TripleElimination, Swiss, and SwissWithTopCut. For any value other than RoundRobin, only the initial round is generated.")]
    public TournamentType TournamentType { get; } = tournamentType;
}
