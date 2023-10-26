namespace MatchMaker.Tool.Controllers;

using System.Globalization;
using System.Xml.Linq;

using MatchMaker.Models;
using MatchMaker.Reporting.Policies;

/// <summary>
/// Implements the base reporting functionality.
/// </summary>
internal abstract class ReportingControllerBase
{
    /// <summary>
    /// Loads the ranking policies
    /// </summary>
    /// <param name="procedure">The team ranking policy procedure definition</param>
    /// <returns>The <see cref="IEnumerable{TeamRankingPolicy}"/> instance</returns>
    protected static IEnumerable<TeamRankingPolicy> LoadRankingPolicies(string procedure)
    {
        return TeamRankingPolicyFactory.GetTeamRankingPolicies(procedure).ToArray();
    }

    /// <summary>
    /// Loads the results from folder.
    /// </summary>
    /// <param name="sourceFolder">The source folder</param>
    /// <param name="schedule">The tournament schedule</param>
    /// <returns>The tournament <see cref="Result"/> instance</returns>
    protected static Result LoadResultsFromFolder(string sourceFolder, Schedule schedule)
    {
        return LoadResultsFromFiles(FindResultFiles(sourceFolder), schedule);
    }

    /// <summary>
    /// Loads the schedule from a folder.
    /// </summary>
    /// <param name="folder">The folder</param>
    /// <returns>The tournament <see cref="Schedule"/> instance</returns>
    protected static Schedule LoadScheduleFromFolder(string folder)
    {
        return LoadScheduleFromFile(FindScheduleFile(folder));
    }

    /// <summary>
    /// Loads the <see cref="XDocument"/> file.
    /// </summary>
    /// <param name="file">The file</param>
    /// <returns>The <see cref="XDocument"/> instance</returns>
    protected static XDocument LoadXml(FileInfo file)
    {
        using var stream = file.OpenRead();
        return XDocument.Load(stream);
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
            .Where(x => !x.EndsWith("export.results.xml", StringComparison.OrdinalIgnoreCase))
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
            .Where(x => !x.EndsWith("export.schedule.xml", StringComparison.OrdinalIgnoreCase))
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
    /// Loads the schedule from file.
    /// </summary>
    /// <param name="file">The tournament file information</param>
    /// <returns>The tournament <see cref="Schedule"/> instance</returns>
    private static Schedule LoadScheduleFromFile(FileInfo file)
    {
        return Schedule.FromXml(LoadXml(file), GetScheduleName(file.Name));
    }
}
