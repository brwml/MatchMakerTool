namespace MatchMaker.Tool;

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
/// Defines the <see cref="Reporting" /> processor class.
/// </summary>
internal static class Reporting
{
    /// <summary>
    /// Processes the reporting options
    /// </summary>
    /// <param name="options">The options<see cref="ReportingOptions"/></param>
    /// <returns>The <see cref="bool"/></returns>
    internal static bool Process(ReportingOptions options)
    {
        Guard.Against.Null(options, nameof(options));

        var summary = CreateSummary(options);

        var directory = Directory.CreateDirectory(options.OutputFolder);

        Parallel.ForEach(GetExporters(options.OutputFormat), exporter =>
        {
            exporter.Export(summary, directory.FullName);
        });

        if (options.NumberOfAlternateTeams > 0)
        {
            TournamentExporter.Create(summary, options.NumberOfTournamentTeams, options.NumberOfAlternateTeams, directory.FullName);
        }

        return true;
    }

    /// <summary>
    /// Processes summary options
    /// </summary>
    /// <param name="options">The options<see cref="SummaryOptions"/></param>
    /// <returns>The <see cref="bool"/></returns>
    internal static bool Process(SummaryOptions options)
    {
        Guard.Against.Null(options, nameof(options));

        var policies = LoadRankingPolicies(ReportingOptions.DefaultRankingProcedure);
        var summaries = options.InputPaths.Select(x => CreateSummary(x, policies));
        SummaryExporter.Export(summaries, options.OutputPath);

        return true;
    }

    /// <summary>
    /// Creates the tournament summary
    /// </summary>
    /// <param name="sourceFolder">The source folder</param>
    /// <param name="policies">The policies</param>
    /// <returns>The <see cref="Summary"/></returns>
    private static Summary CreateSummary(string sourceFolder, TeamRankingPolicy[] policies)
    {
        var schedule = LoadScheduleFromFolder(sourceFolder);
        var result = LoadResultsFromFolder(sourceFolder, schedule);
        return Summary.FromResult(result, policies);
    }

    /// <summary>
    /// Create the tournament summary
    /// </summary>
    /// <param name="options">The reporting options</param>
    /// <returns>The <see cref="Summary"/> instance</returns>
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
    /// <returns>The <see cref="IEnumerable{FileInfo}"/></returns>
    private static IEnumerable<FileInfo> FindResultFiles(string sourceFolder)
    {
        return Directory
            .EnumerateFiles(sourceFolder, "*.results.xml", SearchOption.AllDirectories)
            .Select(x => new FileInfo(x));
    }

    /// <summary>
    /// Finds the schedule file
    /// </summary>
    /// <param name="folder">The folder<see cref="string"/></param>
    /// <returns>The <see cref="FileInfo"/></returns>
    private static FileInfo FindScheduleFile(string folder)
    {
        return Directory
            .EnumerateFiles(folder, "*.schedule.xml", SearchOption.TopDirectoryOnly)
            .Select(x => new FileInfo(x))
            .First();
    }

    /// <summary>
    /// Gets the exporters
    /// </summary>
    /// <param name="format">The <see cref="OutputFormat"/></param>
    /// <returns>The <see cref="IEnumerable{IExporter}"/></returns>
    private static IEnumerable<IExporter> GetExporters(OutputFormat format)
    {
        var list = new List<IExporter> { new DefaultExporter() };

        if (format.HasFlag(OutputFormat.Excel))
        {
            list.Add(new ExcelExporter());
        }

        if (format.HasFlag(OutputFormat.Html))
        {
            list.Add(new HtmlExporter());
        }

        if (format.HasFlag(OutputFormat.Pdf))
        {
            list.Add(new PdfExporter());
        }

        if (format.HasFlag(OutputFormat.Rtf))
        {
            list.Add(new RtfExporter());
        }

        return list;
    }

    /// <summary>
    /// Gets the schedule name
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
    /// <param name="procedure">The procedure <see cref="string"/></param>
    /// <returns>The <see cref="TeamRankingPolicy[]"/></returns>
    private static TeamRankingPolicy[] LoadRankingPolicies(string procedure)
    {
        return procedure.Select(TeamRankingPolicyFromChar).ToArray();
    }

    /// <summary>
    /// Loads the results from files
    /// </summary>
    /// <param name="files">The files<see cref="IEnumerable{FileInfo}"/></param>
    /// <param name="schedule">The schedule<see cref="Schedule"/></param>
    /// <returns>The <see cref="Result"/></returns>
    private static Result LoadResultsFromFiles(IEnumerable<FileInfo> files, Schedule schedule)
    {
        return Result.FromXml(files.Select(x => LoadXml(x)), schedule);
    }

    /// <summary>
    /// Loads the results from folder
    /// </summary>
    /// <param name="sourceFolder">The sourceFolder<see cref="string"/></param>
    /// <param name="schedule">The schedule<see cref="Schedule"/></param>
    /// <returns>The <see cref="Result"/></returns>
    private static Result LoadResultsFromFolder(string sourceFolder, Schedule schedule)
    {
        return LoadResultsFromFiles(FindResultFiles(sourceFolder), schedule);
    }

    /// <summary>
    /// Loads the schedule from file
    /// </summary>
    /// <param name="file">The file<see cref="FileInfo"/></param>
    /// <returns>The <see cref="Schedule"/></returns>
    private static Schedule LoadScheduleFromFile(FileInfo file)
    {
        return Schedule.FromXml(LoadXml(file), GetScheduleName(file.Name));
    }

    /// <summary>
    /// Loads the schedule from a folder
    /// </summary>
    /// <param name="folder">The folder<see cref="string"/></param>
    /// <returns>The <see cref="Schedule"/></returns>
    private static Schedule LoadScheduleFromFolder(string folder)
    {
        return LoadScheduleFromFile(FindScheduleFile(folder));
    }

    /// <summary>
    /// Loads the <see cref="XDocument"/>
    /// </summary>
    /// <param name="file">The <see cref="FileInfo"/></param>
    /// <returns>The <see cref="XDocument"/></returns>
    private static XDocument LoadXml(FileInfo file)
    {
        using var stream = file.OpenRead();
        return XDocument.Load(stream);
    }

    /// <summary>
    /// Gets the team ranking policy for a character
    /// </summary>
    /// <param name="c">The <see cref="char"/></param>
    /// <returns>The <see cref="TeamRankingPolicy"/></returns>
    private static TeamRankingPolicy TeamRankingPolicyFromChar(char c)
    {
        return char.ToUpperInvariant(c) switch
        {
            'W' => new WinPercentageTeamRankingPolicy(),
            'S' => new ScoreTeamRankingPolicy(),
            'E' => new ErrorTeamRankingPolicy(),
            'H' => new HeadToHeadTeamRankingPolicy(),
            'L' => new LossCountTeamRankingPolicy(),
            _ => new NullTeamRankingPolicy(),
        };
    }
}
