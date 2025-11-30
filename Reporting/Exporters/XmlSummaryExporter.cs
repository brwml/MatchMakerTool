namespace MatchMaker.Reporting.Exporters;

using System;
using System.Diagnostics;

using MatchMaker.Reporting.Models;

/// <summary>
/// Exports tournament results to XML files. These result files also contain the raw data as it was provided.
/// </summary>
public class XmlSummaryExporter : ISummaryExporter
{
    /// <summary>
    /// Exports the tournament <paramref name="summary"/> to the specified <paramref name="folder"/>.
    /// </summary>
    /// <param name="summary">The tournament summary</param>
    /// <param name="folder">The output folder</param>
    public void Export(Summary summary, string folder)
    {
        Trace.WriteLine($"Exporting tournament '{summary.Name}' to XML format");
        Trace.Indent();

        try
        {
            var filePathSchedule = Path.Combine(folder, FormattableString.Invariant($"{summary.Name}.export.schedule.xml"));
            Trace.WriteLine($"Writing schedule XML to: {filePathSchedule}");
            File.WriteAllText(filePathSchedule, summary.Result.Schedule.ToXml().ToString());

            var filePathResults = Path.Combine(folder, FormattableString.Invariant($"{summary.Name}.export.results.xml"));
            Trace.WriteLine($"Writing results XML to: {filePathResults}");
            File.WriteAllText(filePathResults, summary.Result.ToXml().ToString());

            var filePathSummary = Path.Combine(folder, FormattableString.Invariant($"{summary.Name}.export.xml"));
            Trace.WriteLine($"Writing summary XML to: {filePathSummary}");
            File.WriteAllText(filePathSummary, summary.ToXml().ToString());

            Trace.WriteLine("XML export completed successfully");
        }
        catch (Exception ex)
        {
            Trace.TraceError($"Error during XML export: {ex.Message}");
            throw;
        }
        finally
        {
            Trace.Unindent();
        }
    }
}
