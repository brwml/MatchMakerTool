namespace MatchMaker.Tool.Controllers;

using System.IO;
using System.Threading.Tasks;

using MatchMaker.Models;
using MatchMaker.Reporting.Models;

/// <summary>
/// Defines the <see cref="ReportingController" /> processor class.
/// </summary>
internal class ReportingController : ReportingControllerBase, IProcessController<ReportingOptions>
{
    /// <summary>
    /// Processes the reporting options.
    /// </summary>
    /// <param name="options">The reporting options</param>
    /// <returns><c>true</c> if the reporting options are processed; otherwise <c>false</c>.</returns>
    public bool Process(ReportingOptions options)
    {
        var summary = CreateSummary(options);

        // Treat the feature as "requested" when either option is non-zero, so an invalid or
        // incomplete combination (for example, specifying only one of the two) is rejected with a
        // clear error instead of being silently ignored.
        var tournamentTeamsRequested = options.NumberOfTournamentTeams != 0 || options.NumberOfAlternateTeams != 0;

        if (tournamentTeamsRequested)
        {
            TournamentTeamsProcessor.Validate(summary, options.NumberOfTournamentTeams, options.NumberOfAlternateTeams);
            summary.NumberOfEliminationTeams = options.NumberOfTournamentTeams;
        }

        var directory = Directory.CreateDirectory(options.OutputFolder);

        Parallel.ForEach(ExporterFactory.GetExporters(options.OutputFormat), exporter => exporter.Export(summary, directory.FullName));

        if (tournamentTeamsRequested)
        {
            TournamentTeamsProcessor.Process(
                summary,
                options.NumberOfTournamentTeams,
                options.NumberOfAlternateTeams,
                options.NumberOfAlternateRooms,
                directory.FullName);
        }

        return true;
    }

    /// <summary>
    /// Create the tournament summary.
    /// </summary>
    /// <param name="options">The reporting options</param>
    /// <returns>The tournament <see cref="Summary"/> instance</returns>
    private static Summary CreateSummary(ReportingOptions options)
    {
        var sourceFolder = options.SourceFolder;
        var schedule = LoadScheduleFromFolder(sourceFolder).WithName(options.Name);
        var result = LoadResultsFromFolder(sourceFolder, schedule);
        return Summary.FromResult(result, LoadRankingPolicies(options.RankingProcedure));
    }
}
