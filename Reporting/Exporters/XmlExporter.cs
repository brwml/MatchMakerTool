namespace MatchMaker.Reporting.Exporters;

using System;

using Ardalis.GuardClauses;

using MatchMaker.Reporting.Models;

/// <summary>
/// Exports tournament results to XML files. These result files also contain the raw data as it was provided.
/// </summary>
public class XmlExporter : IExporter
{
    /// <summary>
    /// Exports the tournament <paramref name="summary"/> to the specified <paramref name="folder"/>.
    /// </summary>
    /// <param name="summary">The tournament summary</param>
    /// <param name="folder">The output folder</param>
    public void Export(Summary summary, string folder)
    {
        Guard.Against.Null(summary);
        Guard.Against.NullOrWhiteSpace(folder);

        var filePathSchedule = Path.Combine(folder, FormattableString.Invariant($"{summary.Name}.export.schedule.xml"));
        File.WriteAllText(filePathSchedule, summary.Result.Schedule.ToXml().ToString());

        var filePathResults = Path.Combine(folder, FormattableString.Invariant($"{summary.Name}.export.results.xml"));
        File.WriteAllText(filePathResults, summary.Result.ToXml().ToString());

        var filePathSummary = Path.Combine(folder, FormattableString.Invariant($"{summary.Name}.export.xml"));
        File.WriteAllText(filePathSummary, summary.ToXml().ToString());
    }
}
