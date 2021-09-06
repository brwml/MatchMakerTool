namespace MatchMaker.Reporting.Exporters
{
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
    using MatchMaker.Utilities;

    /// <summary>
    /// Defines the <see cref="PdfExporter" />
    /// </summary>
    public class PdfExporter : IExporter
    {
        /// <summary>
        /// Defines the header cell font
        /// </summary>
        //private static readonly PdfFont HeaderCellFont = new( PdfFont.FontFamily.HELVETICA, 9f, Font.BOLD | Font.UNDERLINE);

        /// <summary>
        /// Defines the regular cell font
        /// </summary>
        //private static readonly Font RegularCellFont = new Font(Font.FontFamily.HELVETICA, 9f);

        /// <summary>
        /// Defines the subtitle font
        /// </summary>
        //private static readonly Font SubtitleFont = new Font(Font.FontFamily.TIMES_ROMAN, 14f, Font.BOLDITALIC);

        /// <summary>
        /// Defines the title font
        /// </summary>
        //private static readonly Font TitleFont = new Font(Font.FontFamily.TIMES_ROMAN, 20f, Font.BOLDITALIC);

        /// <summary>
        /// Exports the <see cref="Summary"/> to a PDF file.
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="folder">The output folder</param>
        public void Export(Summary summary, string folder)
        {
            Arg.NotNull(summary, nameof(summary));

            var fileName = Path.Combine(folder, $"{summary.Name}.pdf");

            using var document = OpenDocument(fileName);
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
            return new[]
            {
                CreateHeaderCell(string.Empty),
                CreateHeaderCell("Quizzer Name"),
                CreateHeaderCell("Church"),
                CreateHeaderCell("Score"),
                CreateHeaderCell("Errors")
            };
        }

        /// <summary>
        /// Creates the quizzer page title
        /// </summary>
        /// <param name="document">The <see cref="Document"/> instance</param>
        private static void CreateQuizzerPageTitle(Document document)
        {
            document.Add(new AreaBreak());
            document.Add(new Paragraph("Quizzer Results").SetSubtitleFont());
        }

        /// <summary>
        /// Creates a quizzer row
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="quizzer">The <see cref="QuizzerSummary"/> instance</param>
        /// <param name="showPlace">A <see cref="bool"/> indicating whether to show the placement</param>
        /// <returns>The <see cref="PdfPRow"/> instance</returns>
        private static Cell[] CreateQuizzerRow(Summary summary, QuizzerSummary quizzer, bool showPlace)
        {
            var quizzerInfo = summary.Result.Schedule.Quizzers[quizzer.QuizzerId];
            var churchInfo = summary.Result.Schedule.Churches.Values.FirstOrDefault(c => c.Id == quizzerInfo.ChurchId);
            return new[]
            {
                CreateCell(showPlace ? quizzer.Place.ToString(CultureInfo.CurrentCulture) : string.Empty),
                CreateCell($"{quizzerInfo.FirstName} {quizzerInfo.LastName}"),
                CreateCell($"{churchInfo?.Name ?? string.Empty}"),
                CreateCell($"{quizzer.AverageScore.ToString("N2", CultureInfo.CurrentCulture)}"),
                CreateCell($"{quizzer.AverageErrors.ToString("N2", CultureInfo.CurrentCulture)}")
            };
        }

        /// <summary>
        /// Creates the team header row
        /// </summary>
        /// <returns>The <see cref="PdfPRow"/> instance</returns>
        private static Cell[] CreateTeamHeaderRow()
        {
            return new[]
            {
                CreateHeaderCell(string.Empty),
                CreateHeaderCell("Team Name"),
                CreateHeaderCell("W"),
                CreateHeaderCell("L"),
                CreateHeaderCell("Score"),
                CreateHeaderCell("Errors"),
                CreateHeaderCell("Tie Breaker")
            };
        }

        /// <summary>
        /// Creates the team page title.
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="document">The <see cref="Document"/> instance</param>
        private static void CreateTeamPageTitle(Summary summary, Document document)
        {
            document.Add(new Paragraph(summary.Name).SetTitleFont());
            document.Add(new AreaBreak());
            document.Add(new Paragraph("Team Results").SetSubtitleFont());
        }

        /// <summary>
        /// Creates a team row
        /// </summary>
        /// <param name="team">The <see cref="Team"/> instance</param>
        /// <param name="summary">The <see cref="TeamSummary"/> instance</param>
        /// <param name="showPlace">A <see cref="bool"/> indicating whether to show the placement</param>
        /// <returns>The <see cref="PdfPRow"/> instance</returns>
        private static Cell[] CreateTeamRow(Team team, TeamSummary summary, bool showPlace)
        {
            return new[]
            {
                CreateCell(showPlace ? summary.Place.ToString(CultureInfo.CurrentCulture) : string.Empty),
                CreateCell(team.Name),
                CreateCell(summary.Wins.ToString(CultureInfo.CurrentCulture)),
                CreateCell(summary.Losses.ToString(CultureInfo.CurrentCulture)),
                CreateCell(summary.AverageScore.ToString("N2", CultureInfo.CurrentCulture)),
                CreateCell(summary.AverageErrors.ToString("N2", CultureInfo.CurrentCulture)),
                CreateCell(summary.TieBreak.ToString())
            };
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

            var quizzers = summary.QuizzerSummaries.OrderBy(x => x.Value.Place).Select(x => x.Value).ToArray();

            for (var i = 0; i < quizzers.Length; i++)
            {
                var showPlace = i == 0 || quizzers[i].Place != quizzers[i - 1].Place;
                table.AddRow(CreateQuizzerRow(summary, quizzers[i], showPlace));
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

            var teams = summary.TeamSummaries.OrderBy(x => x.Value.Place).Select(x => x.Value).ToArray();

            for (var i = 0; i < teams.Length; i++)
            {
                var showPlace = i == 0 || teams[i].Place != teams[i - 1].Place;
                table.AddRow(CreateTeamRow(summary.Result.Schedule.Teams[teams[i].TeamId], teams[i], showPlace));
            }

            document.Add(table);
        }

        /// <summary>
        /// Opens the PDF <see cref="Document"/> instance.
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <returns>The <see cref="Document"/> instance</returns>
        private static Document OpenDocument(string fileName)
        {
            var writer = new PdfWriter(fileName);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);
            return document;
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
            return paragraph.SetFont(PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLDITALIC)).SetFontSize(27).SetTextAlignment(TextAlignment.CENTER);
        }

        /// <summary>
        /// Sets the subtitle font.
        /// </summary>
        /// <param name="paragraph">The paragraph</param>
        /// <returns>The paragraph</returns>
        public static Paragraph SetSubtitleFont(this Paragraph paragraph)
        {
            return paragraph.SetFont(PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLDITALIC)).SetFontSize(18).SetTextAlignment(TextAlignment.CENTER);
        }
    }
}
