namespace MatchMaker.Tool.Controllers;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

using Ardalis.GuardClauses;

using MatchMaker.Reporting.Exporters;
using MatchMaker.Reporting.Models;
using MatchMaker.Reporting.Policies;

/// <summary>
/// Defines the <see cref="ReportingController" /> processor class.
/// </summary>
internal static class ReportingController
{
    /// <summary>
    /// Processes the reporting options.
    /// </summary>
    /// <param name="options">The reporting options</param>
    /// <returns><c>true</c> if the reporting options are processed; otherwise <c>false</c>.</returns>
    internal static bool Process(ReportingOptions options)
    {
        Guard.Against.Null(options);

        var summary = CreateSummary(options);

        var directory = Directory.CreateDirectory(options.OutputFolder);

        Parallel.ForEach(ExporterFactory.GetExporters(options.OutputFormat), exporter => exporter.Export(summary, directory.FullName));

        if (options.NumberOfAlternateTeams > 0)
        {
            TournamentExporter.Create(summary, options.NumberOfTournamentTeams, options.NumberOfAlternateTeams, directory.FullName);
        }

        return true;
    }

    /// <summary>
    /// Processes the summary options.
    /// </summary>
    /// <param name="options">The summary options</param>
    /// <returns><c>true</c> if the summary options are processed; otherwise <c>false</c>.</returns>
    internal static bool Process(SummaryOptions options)
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

    /// <summary>
    /// Finds the result files
    /// </summary>
    /// <param name="sourceFolder">The source folder</param>
    /// <returns>The <see cref="IEnumerable{FileInfo}"/> instance</returns>
    private static IEnumerable<FileInfo> FindResultFiles(string sourceFolder)
    {
        return Directory
            .EnumerateFiles(sourceFolder, "*.results.xml", SearchOption.AllDirectories)
            .Select(x => new FileInfo(x));
    }

    /// <summary>
    /// Finds the schedule file information.
    /// </summary>
    /// <param name="folder">The folder</param>
    /// <returns>The <see cref="FileInfo"/> instance</returns>
    private static FileInfo FindScheduleFile(string folder)
    {
        return Directory
            .EnumerateFiles(folder, "*.schedule.xml", SearchOption.TopDirectoryOnly)
            .Select(x => new FileInfo(x))
            .First();
    }

    /// <summary>
    /// Gets the schedule name.
    /// </summary>
    /// <param name="name">The tournament name</param>
    /// <returns>The schedule file name</returns>
    private static string GetScheduleName(string name)
    {
        var ti = CultureInfo.CurrentCulture.TextInfo;
        return ti.ToTitleCase(name[..name.IndexOf(".schedule", StringComparison.OrdinalIgnoreCase)]);
    }

    /// <summary>
    /// Loads the ranking policies
    /// </summary>
    /// <param name="procedure">The team ranking policy procedure definition</param>
    /// <returns>The <see cref="IEnumerable{TeamRankingPolicy}"/> instance</returns>
    private static IEnumerable<TeamRankingPolicy> LoadRankingPolicies(string procedure)
    {
        return TeamRankingPolicyFactory.GetTeamRankingPolicies(procedure).ToArray();
    }

    /// <summary>
    /// Loads the results from files.
    /// </summary>
    /// <param name="files">The files</param>
    /// <param name="schedule">The schedule</param>
    /// <returns>The tournament <see cref="Result"/> instance</returns>
    private static Result LoadResultsFromFiles(IEnumerable<FileInfo> files, Schedule schedule)
    {
        return Result.FromXml(files.Select(x => LoadXml(x)), schedule);
    }

    /// <summary>
    /// Loads the results from folder.
    /// </summary>
    /// <param name="sourceFolder">The source folder</param>
    /// <param name="schedule">The tournament schedule</param>
    /// <returns>The tournament <see cref="Result"/> instance</returns>
    private static Result LoadResultsFromFolder(string sourceFolder, Schedule schedule)
    {
        return LoadResultsFromFiles(FindResultFiles(sourceFolder), schedule);
    }

    /// <summary>
    /// Loads the schedule from file.
    /// </summary>
    /// <param name="file">The tournament file information</param>
    /// <returns>The tournament <see cref="Schedule"/> instance</returns>
    private static Schedule LoadScheduleFromFile(FileInfo file)
    {
        return Schedule.FromXml(LoadXml(file), GetScheduleName(file.Name));
    }

    /// <summary>
    /// Loads the schedule from a folder.
    /// </summary>
    /// <param name="folder">The folder</param>
    /// <returns>The tournament <see cref="Schedule"/> instance</returns>
    private static Schedule LoadScheduleFromFolder(string folder)
    {
        return LoadScheduleFromFile(FindScheduleFile(folder));
    }

    /// <summary>
    /// Loads the <see cref="XDocument"/> file.
    /// </summary>
    /// <param name="file">The file</param>
    /// <returns>The <see cref="XDocument"/> instance</returns>
    private static XDocument LoadXml(FileInfo file)
    {
        using var stream = file.OpenRead();
        return XDocument.Load(stream);
    }
}
