namespace MatchMaker.Reporting.Exporters;

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

using Antlr4.StringTemplate;

using MatchMaker.Reporting.Models;

/// <summary>
/// Exports the <see cref="Summary"/> results as an RTF file.
/// </summary>
/// <seealso cref="MatchMaker.Reporting.Exporters.ISummaryExporter" />
public class RtfSummaryExporter : BaseSummaryExporter
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
        Trace.WriteLine($"Exporting tournament '{summary.Name}' to RTF format");
        Trace.Indent();

        try
        {
            var template =
                LoadTemplate()
                    .Add("summary", summary)
                    .Add("teams", GetTeamInfo(summary))
                    .Add("quizzers", GetQuizzerInfo(summary));

            var path = Path.Combine(folder, FormattableString.Invariant($"{summary.Name}.rtf"));
            Trace.WriteLine($"Writing RTF file to: {path}");
            File.WriteAllText(path, template.Render(CultureInfo.InvariantCulture));
            Trace.WriteLine("RTF export completed successfully");
        }
        catch (Exception ex)
        {
            Trace.TraceError($"Error during RTF export: {ex.Message}");
            throw;
        }
        finally
        {
            Trace.Unindent();
        }
    }

    /// <summary>
    /// Loads the template.
    /// </summary>
    /// <returns>The <see cref="Template"/> instance</returns>
    private static Template LoadTemplate()
    {
        Trace.WriteLine("Loading RTF template");
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(RtfTemplate) ?? throw new InvalidOperationException(FormattableString.Invariant($"The template {RtfTemplate} was not found."));
        using var reader = new StreamReader(stream);
        var group = new TemplateGroupString(reader.ReadToEnd());
        group.RegisterRenderer(typeof(decimal), new DecimalAttributeRenderer());
        Trace.WriteLine("RTF template loaded successfully");
        return group.GetInstanceOf(RootElement);
    }
}
