﻿namespace MatchMaker.Tool.Controllers;

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

        var directory = Directory.CreateDirectory(options.OutputFolder);

        Parallel.ForEach(ExporterFactory.GetExporters(options.OutputFormat), exporter => exporter.Export(summary, directory.FullName));

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
