namespace MatchMaker.Reporting.Exporters;

using MatchMaker.Reporting.Models;

/// <summary>
/// Defines the <see cref="ISummaryExporter" /> interface
/// </summary>
public interface ISummaryExporter
{
    /// <summary>
    /// Exports the tournament <see cref="Summary"/> to a file or collection of files.
    /// </summary>
    /// <param name="summary">The <see cref="Summary"/> instance</param>
    /// <param name="folder">The output folder</param>
    void Export(Summary summary, string folder);
}
