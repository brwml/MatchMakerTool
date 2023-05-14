namespace MatchMaker.Reporting.Exporters;

using System.Globalization;
using System.IO;
using System.Reflection;

using Antlr4.StringTemplate;

using Ardalis.GuardClauses;

using MatchMaker.Reporting.Models;

/// <summary>
/// Exports the <see cref="Summary"/> results as an RTF file.
/// </summary>
/// <seealso cref="MatchMaker.Reporting.Exporters.IExporter" />
public class RtfExporter : BaseExporter
{
    /// <summary>
    /// The RTF template
    /// </summary>
    private const string RtfTemplate = "MatchMaker.Reporting.Templates.Rtf.Document.stg";

    /// <summary>
    /// The root element
    /// </summary>
    private const string RootElement = "file";

    /// <summary>
    /// Exports the tournament <see cref="Summary" /> to a file or collection of files.
    /// </summary>
    /// <param name="summary">The <see cref="Summary" /> instance</param>
    /// <param name="folder">The output folder</param>
    public override void Export(Summary summary, string folder)
    {
        Guard.Against.Null(summary);
        Guard.Against.NullOrWhiteSpace(folder);

        var template = LoadTemplate();
        template.Add("summary", summary);
        template.Add("teams", GetTeamInfo(summary));
        template.Add("quizzers", GetQuizzerInfo(summary));

        var path = Path.Combine(folder, FormattableString.Invariant($"{summary.Name}.rtf"));
        File.WriteAllText(path, template.Render(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Loads the template.
    /// </summary>
    /// <returns>The <see cref="Template"/> instance</returns>
    private static Template LoadTemplate()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(RtfTemplate) ?? throw new InvalidOperationException(FormattableString.Invariant($"The template {RtfTemplate} was not found."));
        using var reader = new StreamReader(stream);
        var group = new TemplateGroupString(reader.ReadToEnd());
        group.RegisterRenderer(typeof(decimal), new DecimalAttributeRenderer());
        return group.GetInstanceOf(RootElement);
    }
}
