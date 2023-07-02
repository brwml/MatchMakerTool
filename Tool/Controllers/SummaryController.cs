namespace MatchMaker.Tool.Controllers;

using System.Collections.Generic;
using System.Linq;

using Ardalis.GuardClauses;

using MatchMaker.Reporting.Exporters;
using MatchMaker.Reporting.Models;
using MatchMaker.Reporting.Policies;

internal class SummaryController : ReportingControllerBase, IProcessController<SummaryOptions>
{
    /// <summary>
    /// Processes the specified options.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>
    /// <c>true</c> when processing is successful; otherwise <c>false</c>.
    /// </returns>
    public bool Process(SummaryOptions options)
    {
        Guard.Against.Null(options);

        var policies = LoadRankingPolicies(ReportingOptions.DefaultRankingProcedure);
        var summaries = options.InputPaths.Select(x => CreateSummary(x, policies));
        SummaryExporter.Export(summaries, options.OutputPath);

        return true;
    }

    /// <summary>
    /// Creates the tournament summary.
    /// </summary>
    /// <param name="sourceFolder">The source folder</param>
    /// <param name="policies">The policies</param>
    /// <returns>The tournament <see cref="Summary"/> instance</returns>
    private static Summary CreateSummary(string sourceFolder, IEnumerable<TeamRankingPolicy> policies)
    {
        var schedule = LoadScheduleFromFolder(sourceFolder);
        var result = LoadResultsFromFolder(sourceFolder, schedule);
        return Summary.FromResult(result, policies);
    }
}
