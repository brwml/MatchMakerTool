namespace MatchMaker.Reporting.Exporters
{
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using iTextSharp.text;
    using iTextSharp.text.pdf;

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
        private static readonly Font HeaderCellFont = new Font(Font.FontFamily.HELVETICA, 9f, Font.BOLD | Font.UNDERLINE);

        /// <summary>
        /// Defines the regular cell font
        /// </summary>
        private static readonly Font RegularCellFont = new Font(Font.FontFamily.HELVETICA, 9f);

        /// <summary>
        /// Defines the subtitle font
        /// </summary>
        private static readonly Font SubtitleFont = new Font(Font.FontFamily.TIMES_ROMAN, 14f, Font.BOLDITALIC);

        /// <summary>
        /// Defines the title font
        /// </summary>
        private static readonly Font TitleFont = new Font(Font.FontFamily.TIMES_ROMAN, 20f, Font.BOLDITALIC);

        /// <summary>
        /// Exports the <see cref="Summary"/> to a PDF file.
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="folder">The output folder</param>
        public void Export(Summary summary, string folder)
        {
            Arg.NotNull(summary, nameof(summary));

            var fileName = Path.Combine(folder, $"{summary.Name}.pdf");

            using (var stream = File.Create(fileName))
            using (var document = OpenDocument(stream))
            {
                CreateTeamPageTitle(summary, document);
                ExportTeamResults(document, summary);

                CreateQuizzerPageTitle(document);
                ExportQuizzerResults(document, summary);

                document.Close();
            }
        }

        /// <summary>
        /// Creates a regular cell.
        /// </summary>
        /// <param name="content">The <see cref="string"/> content of the cell</param>
        /// <returns>The <see cref="PdfPCell"/> instance</returns>
        private static PdfPCell CreateCell(string content)
        {
            var phrase = new Phrase(content, RegularCellFont);
            return new PdfPCell(phrase)
            {
                Border = 0,
                BorderWidth = 0
            };
        }

        /// <summary>
        /// Creates a header cell
        /// </summary>
        /// <param name="content">The <see cref="string"/> content</param>
        /// <returns>The <see cref="PdfPHeaderCell"/> instance</returns>
        private static PdfPHeaderCell CreateHeaderCell(string content)
        {
            var phrase = new Phrase(content, HeaderCellFont);
            var cell = new PdfPHeaderCell();
            cell.AddElement(phrase);
            cell.Border = 0;
            cell.BorderWidth = 0;
            return cell;
        }

        /// <summary>
        /// Creates the quizzer header row
        /// </summary>
        /// <returns>The <see cref="PdfPRow"/> instance</returns>
        private static PdfPRow CreateQuizzerHeaderRow()
        {
            var cells = new[]
            {
                CreateHeaderCell(string.Empty),
                CreateHeaderCell("Quizzer Name"),
                CreateHeaderCell("Church"),
                CreateHeaderCell("Score"),
                CreateHeaderCell("Errors")
            };
            return new PdfPRow(cells);
        }

        /// <summary>
        /// Creates the quizzer page title
        /// </summary>
        /// <param name="document">The <see cref="Document"/> instance</param>
        private static void CreateQuizzerPageTitle(Document document)
        {
            document.NewPage();
            var paragraph = new Paragraph(new Phrase("Quizzer Results", SubtitleFont)) { Alignment = Element.ALIGN_CENTER };
            document.Add(paragraph);
            document.Add(Chunk.NEWLINE);
        }

        /// <summary>
        /// Creates a quizzer row
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="quizzer">The <see cref="QuizzerSummary"/> instance</param>
        /// <param name="showPlace">A <see cref="bool"/> indicating whether to show the placement</param>
        /// <returns>The <see cref="PdfPRow"/> instance</returns>
        private static PdfPRow CreateQuizzerRow(Summary summary, QuizzerSummary quizzer, bool showPlace)
        {
            var quizzerInfo = summary.Result.Schedule.Quizzers[quizzer.QuizzerId];
            var churchInfo = summary.Result.Schedule.Churches.Values.FirstOrDefault(c => c.Id == quizzerInfo.ChurchId);
            var cells = new[]
            {
                CreateCell(showPlace ? quizzer.Place.ToString(CultureInfo.CurrentCulture) : string.Empty),
                CreateCell($"{quizzerInfo.FirstName} {quizzerInfo.LastName}"),
                CreateCell($"{churchInfo?.Name ?? string.Empty}"),
                CreateCell($"{quizzer.AverageScore.ToString("N2", CultureInfo.CurrentCulture)}"),
                CreateCell($"{quizzer.AverageErrors.ToString("N2", CultureInfo.CurrentCulture)}")
            };
            return new PdfPRow(cells);
        }

        /// <summary>
        /// Creates the team header row
        /// </summary>
        /// <returns>The <see cref="PdfPRow"/> instance</returns>
        private static PdfPRow CreateTeamHeaderRow()
        {
            var cells = new[]
            {
                CreateHeaderCell(string.Empty),
                CreateHeaderCell("Team Name"),
                CreateHeaderCell("W"),
                CreateHeaderCell("L"),
                CreateHeaderCell("Score"),
                CreateHeaderCell("Errors"),
                CreateHeaderCell("Tie Breaker")
            };
            return new PdfPRow(cells);
        }

        /// <summary>
        /// Creates the team page title.
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="document">The <see cref="Document"/> instance</param>
        private static void CreateTeamPageTitle(Summary summary, Document document)
        {
            document.AddTitle(summary.Name);
            var title = new Paragraph(new Phrase(summary.Name, TitleFont)) { Alignment = Element.ALIGN_CENTER };
            document.Add(title);
            document.Add(Chunk.NEWLINE);
            var subtitle = new Paragraph(new Phrase("Team Results", SubtitleFont)) { Alignment = Element.ALIGN_CENTER };
            document.Add(subtitle);
            document.Add(Chunk.NEWLINE);
        }

        /// <summary>
        /// Creates a team row
        /// </summary>
        /// <param name="team">The <see cref="Team"/> instance</param>
        /// <param name="summary">The <see cref="TeamSummary"/> instance</param>
        /// <param name="showPlace">A <see cref="bool"/> indicating whether to show the placement</param>
        /// <returns>The <see cref="PdfPRow"/> instance</returns>
        private static PdfPRow CreateTeamRow(Team team, TeamSummary summary, bool showPlace)
        {
            var cells = new[]
            {
                CreateCell(showPlace ? summary.Place.ToString(CultureInfo.CurrentCulture) : string.Empty),
                CreateCell(team.Name),
                CreateCell(summary.Wins.ToString(CultureInfo.CurrentCulture)),
                CreateCell(summary.Losses.ToString(CultureInfo.CurrentCulture)),
                CreateCell(summary.AverageScore.ToString("N2", CultureInfo.CurrentCulture)),
                CreateCell(summary.AverageErrors.ToString("N2", CultureInfo.CurrentCulture)),
                CreateCell(summary.TieBreak.ToString())
            };
            return new PdfPRow(cells);
        }

        /// <summary>
        /// Exports the quizzer results
        /// </summary>
        /// <param name="document">The <see cref="Document"/> instance</param>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        private static void ExportQuizzerResults(Document document, Summary summary)
        {
            var table = new PdfPTable(new[] { 1f, 5f, 5f, 2f, 2f });

            table.Rows.Add(CreateQuizzerHeaderRow());

            var quizzers = summary.QuizzerSummaries.OrderBy(x => x.Value.Place).Select(x => x.Value).ToArray();

            for (var i = 0; i < quizzers.Length; i++)
            {
                var showPlace = i == 0 || quizzers[i].Place != quizzers[i - 1].Place;
                table.Rows.Add(CreateQuizzerRow(summary, quizzers[i], showPlace));
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
            var table = new PdfPTable(new[] { 1f, 5f, 1f, 1f, 2f, 2f, 4f });

            table.Rows.Add(CreateTeamHeaderRow());

            var teams = summary.TeamSummaries.OrderBy(x => x.Value.Place).Select(x => x.Value).ToArray();

            for (var i = 0; i < teams.Length; i++)
            {
                var showPlace = i == 0 || teams[i].Place != teams[i - 1].Place;
                table.Rows.Add(CreateTeamRow(summary.Result.Schedule.Teams[teams[i].TeamId], teams[i], showPlace));
            }

            document.Add(table);
        }

        /// <summary>
        /// Opens the PDF <see cref="Document"/> instance.
        /// </summary>
        /// <param name="stream">The file <see cref="Stream"/></param>
        /// <returns>The <see cref="Document"/> instance</returns>
        private static Document OpenDocument(Stream stream)
        {
            var document = new Document();

            PdfWriter.GetInstance(document, stream);
            document.Open();
            return document;
        }
    }
}
