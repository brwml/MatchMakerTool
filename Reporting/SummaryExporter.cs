using System.Collections.Generic;
using System.IO;
using System.Linq;

using ClosedXML.Excel;

namespace MatchMaker.Reporting
{
    public static class SummaryExporter
    {
        public static void Export(IEnumerable<Summary> summaries, string folder)
        {
            using (var workbook = new XLWorkbook())
            {
                Export(workbook, summaries);
                SaveWorkbook(workbook, folder);
            }
        }

        private static InternalQuizzerSummary CreateInternalQuizzerSummary(Summary summary, QuizzerSummary quizzerSummary)
        {
            var quizzer = summary.Result.Schedule.Quizzers[quizzerSummary.QuizzerId];
            var church = summary.Result.Schedule.Churches[quizzer.ChurchId];
            return new InternalQuizzerSummary
            {
                Church = church.Name,
                Name = $"{quizzer.LastName}, {quizzer.FirstName}"
            };
        }

        private static Dictionary<int, InternalQuizzerSummary> CreateQuizzerSummaries(IEnumerable<Summary> summaries)
        {
            var quizzerSummaries = new Dictionary<int, InternalQuizzerSummary>();

            foreach (var summary in summaries)
            {
                foreach (var quizzerSummary in summary.QuizzerSummaries)
                {
                    if (!quizzerSummaries.ContainsKey(quizzerSummary.Key))
                    {
                        quizzerSummaries.Add(quizzerSummary.Key, CreateInternalQuizzerSummary(summary, quizzerSummary.Value));
                    }

                    var quizzer = quizzerSummaries[quizzerSummary.Key];
                    quizzer.Results.Add(summary.Name, quizzerSummary.Value);
                }
            }

            return quizzerSummaries;
        }

        private static void Export(XLWorkbook workbook, IEnumerable<Summary> summaries)
        {
            var summaryNames = summaries.Select(x => x.Name).Distinct().ToArray();
            var worksheet = workbook.AddWorksheet("Summary");

            SetWorksheetHeaders(summaryNames, worksheet);
            SetWorksheetValues(summaries, summaryNames, worksheet);
        }

        private static void SaveWorkbook(XLWorkbook workbook, string folder)
        {
            var filePath = Path.Combine(folder, "summary.xlsx");

            File.Delete(filePath);
            workbook.SaveAs(filePath);
        }

        private static void SetWorksheetHeaders(string[] summaryNames, IXLWorksheet worksheet)
        {
            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Church";

            for (var i = 0; i < summaryNames.Length; i++)
            {
                worksheet.Cell(1, i + 4).Value = summaryNames[i];
            }
        }

        private static void SetWorksheetValues(IEnumerable<Summary> summaries, string[] summaryNames, IXLWorksheet worksheet)
        {
            var quizzerSummaries = CreateQuizzerSummaries(summaries);

            var row = 2;

            foreach (var item in quizzerSummaries)
            {
                worksheet.Cell(row, 1).Value = item.Key;
                worksheet.Cell(row, 2).Value = item.Value.Name;
                worksheet.Cell(row, 3).Value = item.Value.Church;

                for (var i = 0; i < summaryNames.Length; i++)
                {
                    if (item.Value.Results.ContainsKey(summaryNames[i]))
                    {
                        worksheet.Cell(row, i + 4).Value = item.Value.Results[summaryNames[i]].AverageScore;
                    }
                }

                row++;
            }
        }

        private class InternalQuizzerSummary
        {
            public string Church
            {
                get; set;
            }

            public string Name
            {
                get; set;
            }

            public Dictionary<string, QuizzerSummary> Results { get; } = new Dictionary<string, QuizzerSummary>();
        }
    }
}