namespace MatchMaker.Reporting.Exporters;

using System.IO;
using System.Text.Json;

using Ardalis.GuardClauses;

using MatchMaker.Reporting.Models;

/// <summary>
/// Defines the <see cref="DefaultExporter" />
/// </summary>
public class DefaultExporter : IExporter
{
    /// <summary>
    /// Exports the <see cref="Summary"/> instance using the default serializer.
    /// </summary>
    /// <param name="summary">The summary</param>
    /// <param name="folder">The folder</param>
    public void Export(Summary summary, string folder)
    {
        Guard.Against.Null(summary, nameof(summary));
        Guard.Against.NullOrWhiteSpace(folder, nameof(folder));

        var path = Path.Combine(folder, FormattableString.Invariant($"{summary.Name}.summary.json"));
        var options = new JsonSerializerOptions
        {
            IgnoreReadOnlyProperties = false,
            IncludeFields = false,
            WriteIndented = true
        };
        File.WriteAllText(path, JsonSerializer.Serialize(summary, options));
    }
}
