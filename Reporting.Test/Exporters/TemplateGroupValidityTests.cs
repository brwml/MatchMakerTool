namespace Reporting.Test.Exporters;

using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Antlr4.StringTemplate;
using Antlr4.StringTemplate.Misc;

using MatchMaker.Reporting.Exporters;

using Xunit;

/// <summary>
/// Verifies that every embedded StringTemplate group compiles without error. This is a fast,
/// fixture-free guard against template syntax defects (for example, unescaped literal braces
/// inside anonymous subtemplates) that would otherwise only surface when a specific rendering
/// path happens to exercise the broken template.
/// </summary>
public class TemplateGroupValidityTests
{
    public static IEnumerable<object[]> TemplateResourceNames()
    {
        yield return ["MatchMaker.Reporting.Templates.Html.Index.stg"];
        yield return ["MatchMaker.Reporting.Templates.Html.QuizzerDetail.stg"];
        yield return ["MatchMaker.Reporting.Templates.Html.QuizzerSummary.stg"];
        yield return ["MatchMaker.Reporting.Templates.Html.TeamDetail.stg"];
        yield return ["MatchMaker.Reporting.Templates.Html.TeamSummary.stg"];
        yield return ["MatchMaker.Reporting.Templates.Markdown.Index.stg"];
        yield return ["MatchMaker.Reporting.Templates.Markdown.QuizzerDetail.stg"];
        yield return ["MatchMaker.Reporting.Templates.Markdown.QuizzerSummary.stg"];
        yield return ["MatchMaker.Reporting.Templates.Markdown.TeamDetail.stg"];
        yield return ["MatchMaker.Reporting.Templates.Markdown.TeamSummary.stg"];
        yield return ["MatchMaker.Reporting.Templates.Rtf.Document.stg"];
    }

    [Theory]
    [MemberData(nameof(TemplateResourceNames))]
    public void TemplateGroup_Loads_WithoutCompileErrors(string resourceName)
    {
        var assembly = typeof(HtmlSummaryExporter).Assembly;
        using var stream = assembly.GetManifestResourceStream(resourceName);
        Assert.NotNull(stream);

        using var reader = new StreamReader(stream);
        var listener = new CollectingErrorListener();
        var group = new TemplateGroupString(reader.ReadToEnd())
        {
            ErrorManager = new ErrorManager(listener)
        };

        group.Load();

        Assert.Empty(listener.Messages);
    }

    private sealed class CollectingErrorListener : ITemplateErrorListener
    {
        public List<string> Messages { get; } = [];

        public void CompiletimeError(TemplateMessage msg) => this.Messages.Add(msg.ToString());

        public void RuntimeError(TemplateMessage msg) => this.Messages.Add(msg.ToString());

        public void IOError(TemplateMessage msg) => this.Messages.Add(msg.ToString());

        public void InternalError(TemplateMessage msg) => this.Messages.Add(msg.ToString());
    }
}
