namespace MatchMaker.Reporting.Exporters;

using System.Globalization;
using System.IO;
using System.Linq;

using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

using MatchMaker.Reporting.Models;

/// <summary>
/// Defines the <see cref="PdfSummaryExporter" />
/// </summary>
public class PdfSummaryExporter : BaseSummaryExporter
{
    /// <summary>
    /// Exports the <see cref="Summary"/> to a PDF file.
    /// </summary>
    /// <param name="summary">The <see cref="Summary"/> instance</param>
    /// <param name="folder">The output folder</param>
    public override void Export(Summary summary, string folder)
    {
        var fileName = Path.Combine(folder, FormattableString.Invariant($"{summary.Name}.pdf"));

        using var writer = new PdfWriter(fileName);
        using var pdfDoc = new PdfDocument(writer);
        using var document = new Document(pdfDoc);

        CreateTeamPageTitle(summary, document);
        ExportTeamResults(document, summary);

        CreateQuizzerPageTitle(document);
        ExportQuizzerResults(document, summary);

        document.Close();
    }

    /// <summary>
    /// Creates a regular cell.
    /// </summary>
    /// <param name="content">The <see cref="string"/> content of the cell</param>
    /// <returns>The <see cref="PdfPCell"/> instance</returns>
    private static Cell CreateCell(string content)
    {
        return new Cell()
            .Add(new Paragraph(content).SetNormalFont())
            .SetBorder(Border.NO_BORDER)
            .SetPaddingRight(10f)
            ;
    }

    /// <summary>
    /// Creates a header cell
    /// </summary>
    /// <param name="content">The <see cref="string"/> content</param>
    /// <returns>The <see cref="PdfPHeaderCell"/> instance</returns>
    private static Cell CreateHeaderCell(string content)
    {
        return new Cell()
            .Add(new Paragraph(content).SetNormalFont())
            .SetBold()
            .SetUnderline()
            .SetBorder(Border.NO_BORDER)
            .SetPaddingRight(10f)
            ;
    }

    /// <summary>
    /// Creates the quizzer header row
    /// </summary>
    /// <returns>The <see cref="PdfPRow"/> instance</returns>
    private static Cell[] CreateQuizzerHeaderRow()
    {
        return
        [
            CreateHeaderCell(string.Empty),
            CreateHeaderCell("Quizzer Name"),
            CreateHeaderCell("Church"),
            CreateHeaderCell("Score"),
            CreateHeaderCell("Errors")
        ];
    }

    /// <summary>
    /// Creates the quizzer page title
    /// </summary>
    /// <param name="document">The <see cref="Document"/> instance</param>
    private static void CreateQuizzerPageTitle(Document document)
    {
        document
            .Add(new AreaBreak())
            .Add(new Paragraph("Quizzer Results").SetSubtitleFont());
    }

    /// <summary>
    /// Creates a quizzer row
    /// </summary>
    /// <param name="quizzer">The <see cref="QuizzerSummary"/> instance</param>
    /// <returns>The <see cref="PdfPRow"/> instance</returns>
    private static Cell[] CreateQuizzerRow(QuizzerInfo quizzer)
    {
        return
        [
            CreateCell(quizzer.ShowPlace ? quizzer.Place.ToString(CultureInfo.CurrentCulture) : string.Empty),
            CreateCell(FormattableString.Invariant($"{quizzer.FullName}{GetRookieTag(quizzer.IsRookie)}")),
            CreateCell(FormattableString.Invariant($"{quizzer.Church.Name}")),
            CreateCell(FormattableString.Invariant($"{quizzer.AverageScore.ToString("N2", CultureInfo.CurrentCulture)}")),
            CreateCell(FormattableString.Invariant($"{quizzer.AverageErrors.ToString("N2", CultureInfo.CurrentCulture)}"))
        ];
    }

    /// <summary>
    /// Gets the tag text indicating that a quizzer is a rookie.
    /// </summary>
    /// <param name="isRookie">A value indicating whether the quizzer is a rookie</param>
    /// <returns>The tag text</returns>
    private static string GetRookieTag(bool isRookie)
    {
        return isRookie ? " (R)" : string.Empty;
    }

    /// <summary>
    /// Creates the team header row
    /// </summary>
    /// <returns>The <see cref="PdfPRow"/> instance</returns>
    private static Cell[] CreateTeamHeaderRow()
    {
        return
        [
            CreateHeaderCell(string.Empty),
            CreateHeaderCell("Team Name"),
            CreateHeaderCell("W"),
            CreateHeaderCell("L"),
            CreateHeaderCell("Score"),
            CreateHeaderCell("Errors"),
            CreateHeaderCell("Tie Breaker")
        ];
    }

    /// <summary>
    /// Creates the team page title.
    /// </summary>
    /// <param name="summary">The <see cref="Summary"/> instance</param>
    /// <param name="document">The <see cref="Document"/> instance</param>
    private static void CreateTeamPageTitle(Summary summary, Document document)
    {
        document
            .Add(new Paragraph(summary.Name).SetTitleFont())
            .Add(new AreaBreak())
            .Add(new Paragraph("Team Results").SetSubtitleFont());
    }

    /// <summary>
    /// Creates a team row
    /// </summary>
    /// <param name="team">The <see cref="Team"/> instance</param>
    /// <param name="summary">The <see cref="TeamSummary"/> instance</param>
    /// <param name="showPlace">A <see cref="bool"/> indicating whether to show the placement</param>
    /// <returns>The <see cref="PdfPRow"/> instance</returns>
    private static Cell[] CreateTeamRow(TeamInfo team)
    {
        return
        [
            CreateCell(team.ShowPlace ? team.Place.ToString(CultureInfo.CurrentCulture) : string.Empty),
            CreateCell(team.Name),
            CreateCell(team.Wins.ToString(CultureInfo.CurrentCulture)),
            CreateCell(team.Losses.ToString(CultureInfo.CurrentCulture)),
            CreateCell(team.AverageScore.ToString("N2", CultureInfo.CurrentCulture)),
            CreateCell(team.AverageErrors.ToString("N2", CultureInfo.CurrentCulture)),
            CreateCell(team.TieBreak.ToString())
        ];
    }

    /// <summary>
    /// Exports the quizzer results
    /// </summary>
    /// <param name="document">The <see cref="Document"/> instance</param>
    /// <param name="summary">The <see cref="Summary"/> instance</param>
    private static void ExportQuizzerResults(Document document, Summary summary)
    {
        var table = new Table(5).SetHorizontalAlignment(HorizontalAlignment.CENTER);

        table.AddHeader(CreateQuizzerHeaderRow());

        var quizzers = GetQuizzerInfo(summary).ToArray();

        foreach (var quizzer in quizzers)
        {
            table.AddRow(CreateQuizzerRow(quizzer));
        }

        document.Add(table);
    }

    /// <summary>
    /// Exports the team results.
    /// </summary>
    /// <param name="document">The <see cref="Document"/> instance</param>
    /// <param name="summary">The <see cref="Summary"/> instance</param>
    private static void ExportTeamResults(Document document, Summary summary)
    {
        var table = new Table(7).SetHorizontalAlignment(HorizontalAlignment.CENTER);

        table.AddHeader(CreateTeamHeaderRow());

        var teams = GetTeamInfo(summary);

        foreach (var team in teams)
        {
            table.AddRow(CreateTeamRow(team));
        }

        document.Add(table);
    }
}

/// <summary>
/// Extensions for manipulating PDF tables.
/// </summary>
internal static class TableExtensions
{
    /// <summary>
    /// Add a header row.
    /// </summary>
    /// <param name="table">The table</param>
    /// <param name="cells">The header row cells</param>
    /// <returns>The table</returns>
    public static Table AddHeader(this Table table, Cell[] cells)
    {
        foreach (var cell in cells)
        {
            table.AddHeaderCell(cell);
        }

        return table;
    }

    /// <summary>
    /// Adds a table row
    /// </summary>
    /// <param name="table">The table</param>
    /// <param name="cells">The row cells</param>
    /// <returns>The table</returns>
    public static Table AddRow(this Table table, Cell[] cells)
    {
        foreach (var cell in cells)
        {
            table.AddCell(cell);
        }

        return table;
    }
}

/// <summary>
/// Extensions methods for manipulating paragraphs.
/// </summary>
internal static class ParagraphExtensions
{
    /// <summary>
    /// Sets the normal font on the paragraph.
    /// </summary>
    /// <param name="paragraph">The paragraph</param>
    /// <returns>The paragraph</returns>
    public static Paragraph SetNormalFont(this Paragraph paragraph)
    {
        return paragraph.SetFontSize(9);
    }

    /// <summary>
    /// Sets the title font.
    /// </summary>
    /// <param name="paragraph">The paragraph</param>
    /// <returns>The paragraph</returns>
    public static Paragraph SetTitleFont(this Paragraph paragraph)
    {
        return paragraph.SetFont(PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLDITALIC))
                        .SetFontSize(27)
                        .SetTextAlignment(TextAlignment.CENTER);
    }

    /// <summary>
    /// Sets the subtitle font.
    /// </summary>
    /// <param name="paragraph">The paragraph</param>
    /// <returns>The paragraph</returns>
    public static Paragraph SetSubtitleFont(this Paragraph paragraph)
    {
        return paragraph.SetFont(PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLDITALIC))
                        .SetFontSize(18)
                        .SetTextAlignment(TextAlignment.CENTER);
    }
}
