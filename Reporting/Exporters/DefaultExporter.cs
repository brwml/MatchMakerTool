namespace MatchMaker.Reporting.Exporters;

using System.IO;

using Ardalis.GuardClauses;

using MatchMaker.Reporting.Models;

using Newtonsoft.Json;

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

        var name = summary.Name;
        var path = Path.Combine(folder, name + ".summary.json");
        File.WriteAllText(path, JsonConvert.SerializeObject(summary, Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));
    }
}
