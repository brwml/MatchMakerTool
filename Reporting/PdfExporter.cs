using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;
using iTextSharp.text;
using iTextSharp.text.pdf;
using QuickGraph;
using QuickGraph.Graphviz;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MatchMaker.Reporting
{
    public class PdfExporter : IExporter
    {
        private static readonly Font HeaderCellFont = new Font(Font.FontFamily.HELVETICA, 9f, Font.BOLD | Font.UNDERLINE);

        private static readonly Font RegularCellFont = new Font(Font.FontFamily.HELVETICA, 9f);

        private static readonly Font SubtitleFont = new Font(Font.FontFamily.TIMES_ROMAN, 14f, Font.BOLDITALIC);

        private static readonly Font TitleFont = new Font(Font.FontFamily.TIMES_ROMAN, 20f, Font.BOLDITALIC);

        public void Export(Summary summary, string folder)
        {
            var fileName = Path.Combine(folder, $"{summary.Name}.pdf");
            var document = OpenDocument(fileName);

            CreateTeamPageTitle(summary, document);
            ExportTeamResults(document, summary);

            CreateQuizzerPageTitle(document);
            ExportQuizzerResults(document, summary);

            ExportTieBreakers(fileName, summary);

            document.Close();
        }

        private static string AppendToFileName(string fileName, string append)
        {
            return Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + append + Path.GetExtension(fileName));
        }

        private static PdfPCell CreateCell(string content)
        {
            var phrase = new Phrase(content, RegularCellFont);
            var cell = new PdfPCell(phrase);
            cell.Border = 0;
            cell.BorderWidth = 0;
            return cell;
        }

        private static PdfPHeaderCell CreateHeaderCell(string content)
        {
            var phrase = new Phrase(content, HeaderCellFont);
            var cell = new PdfPHeaderCell();
            cell.AddElement(phrase);
            cell.Border = 0;
            cell.BorderWidth = 0;
            return cell;
        }

        private static PdfPRow CreateQuizzerHeaderRow()
        {
            List<PdfPHeaderCell> cells = new List<PdfPHeaderCell>();
            cells.Add(CreateHeaderCell(string.Empty));
            cells.Add(CreateHeaderCell("Quizzer Name"));
            cells.Add(CreateHeaderCell("Church"));
            cells.Add(CreateHeaderCell("Score"));
            cells.Add(CreateHeaderCell("Errors"));
            return new PdfPRow(cells.ToArray());
        }

        private static void CreateQuizzerPageTitle(Document document)
        {
            document.NewPage();
            var paragraph = new Paragraph(new Phrase("Quizzer Results", SubtitleFont)) { Alignment = Element.ALIGN_CENTER };
            document.Add(paragraph);
            document.Add(Chunk.NEWLINE);
        }

        private static PdfPRow CreateQuizzerRow(Summary summary, QuizzerSummary quizzer, bool showPlace)
        {
            List<PdfPCell> cells = new List<PdfPCell>();
            cells.Add(CreateCell(showPlace ? quizzer.Place.ToString() : string.Empty));
            var quizzerInfo = summary.Result.Schedule.Quizzers[quizzer.QuizzerId];
            var churchInfo = summary.Result.Schedule.Churches.Values.FirstOrDefault(c => c.Id == quizzerInfo.ChurchId);
            cells.Add(CreateCell($"{quizzerInfo.FirstName} {quizzerInfo.LastName}"));
            cells.Add(CreateCell($"{churchInfo?.Name ?? string.Empty}"));
            cells.Add(CreateCell($"{quizzer.AverageScore.ToString("N2")}"));
            cells.Add(CreateCell($"{quizzer.AverageErrors.ToString("N2")}"));
            return new PdfPRow(cells.ToArray());
        }

        private static PdfPRow CreateTeamHeaderRow()
        {
            List<PdfPHeaderCell> cells = new List<PdfPHeaderCell>();
            cells.Add(CreateHeaderCell(string.Empty));
            cells.Add(CreateHeaderCell("Team Name"));
            cells.Add(CreateHeaderCell("W"));
            cells.Add(CreateHeaderCell("L"));
            cells.Add(CreateHeaderCell("Score"));
            cells.Add(CreateHeaderCell("Errors"));
            cells.Add(CreateHeaderCell("Tie Breaker"));
            return new PdfPRow(cells.ToArray());
        }

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

        private static PdfPRow CreateTeamRow(Team team, TeamSummary summary, bool showPlace)
        {
            List<PdfPCell> cells = new List<PdfPCell>();
            cells.Add(CreateCell(showPlace ? summary.Place.ToString() : string.Empty));
            cells.Add(CreateCell(team.Name));
            cells.Add(CreateCell(summary.Wins.ToString()));
            cells.Add(CreateCell(summary.Losses.ToString()));
            cells.Add(CreateCell(summary.AverageScore.ToString("N2")));
            cells.Add(CreateCell(summary.AverageErrors.ToString("N2")));
            cells.Add(CreateCell(summary.TieBreak.ToString()));
            return new PdfPRow(cells.ToArray());
        }

        private static void ExportQuizzerResults(Document document, Summary summary)
        {
            var table = new PdfPTable(new[] { 1f, 5f, 5f, 2f, 2f });

            table.Rows.Add(CreateQuizzerHeaderRow());

            var quizzers = summary.QuizzerSummaries.OrderBy(x => x.Value.Place).Select(x => x.Value).ToArray();

            for (int i = 0; i < quizzers.Length; i++)
            {
                var showPlace = i == 0 || quizzers[i].Place != quizzers[i - 1].Place;
                table.Rows.Add(CreateQuizzerRow(summary, quizzers[i], showPlace));
            }

            document.Add(table);
        }

        private static void ExportTeamResults(Document document, Summary summary)
        {
            var table = new PdfPTable(new[] { 1f, 5f, 1f, 1f, 2f, 2f, 4f });

            table.Rows.Add(CreateTeamHeaderRow());

            var teams = summary.TeamSummaries.OrderBy(x => x.Value.Place).Select(x => x.Value).ToArray();

            for (int i = 0; i < teams.Length; i++)
            {
                var showPlace = i == 0 || teams[i].Place != teams[i - 1].Place;
                table.Rows.Add(CreateTeamRow(summary.Result.Schedule.Teams[teams[i].TeamId], teams[i], showPlace));
            }

            document.Add(table);
        }

        private static Document OpenDocument(string fileName)
        {
            var document = new Document();
            PdfWriter.GetInstance(document, File.Create(fileName));
            document.Open();
            return document;
        }

        private void ExportTieBreakers(string fileName, Summary summary)
        {
            var groups = summary.TeamSummaries.Select(x => x.Value).GroupBy(x => x.Losses).Select(x => x.Select(y => y));

            var document = OpenDocument(AppendToFileName(fileName, "_TieBreakers_HTH"));

            foreach (var group in groups)
            {
                if (group.Count() > 1)
                {
                    var teams = group.Select(x => summary.Result.Schedule.Teams[x.TeamId]).ToDictionary(k => k.Id, v => v);
                    var matches = summary.Result.Matches.Where(x => x.Value.TeamResults.All(y => teams.ContainsKey(y.TeamId))).Select(x => x.Value);

                    var graph = new AdjacencyGraph<string, Edge<string>>();
                    graph.AddVertexRange(teams.Select(x => x.Value.Abbreviation));
                    graph.AddEdgeRange(matches.Select(x => new Edge<string>(teams[x.TeamResults.First(t => t.Place == 1).TeamId].Abbreviation, teams[x.TeamResults.First(t => t.Place == 2).TeamId].Abbreviation)));

                    var dot = graph.ToGraphviz(algorithm =>
                    {
                        algorithm.FormatVertex += (s, e) => { };
                    });

                    var start = new GetStartProcessQuery();
                    var process = new GetProcessStartInfoQuery();
                    var command = new RegisterLayoutPluginCommand(process, start);
                    var wrapper = new GraphGeneration(start, process, command);

                    var bytes = wrapper.GenerateGraph(dot, Enums.GraphReturnType.Png);

                    document.NewPage();
                    document.Add(Image.GetInstance(bytes));
                }
            }

            document.Close();
        }
    }
}