namespace MatchMaker.Reporting.Exporters;

using System.Collections.Generic;
using System.IO;
using System.Linq;

using Ardalis.GuardClauses;

using ClosedXML.Excel;

using MatchMaker.Reporting.Models;

/// <summary>
/// Defines the <see cref="SummaryExporter" />
/// </summary>
public static class SummaryExporter
{
    /// <summary>
    /// Exports a summary to Excel.
    /// </summary>
    /// <param name="summaries">The <see cref="IEnumerable{Summary}"/></param>
    /// <param name="folder">The output folder</param>
    public static void Export(IEnumerable<Summary> summaries, string folder)
    {
        Guard.Against.NullOrEmpty(summaries, nameof(summaries));
        Guard.Against.NullOrWhiteSpace(folder, nameof(folder));

        using var workbook = new XLWorkbook();
        Export(workbook, summaries);
        SaveWorkbook(workbook, folder);
    }

    /// <summary>
    /// Creates a <see cref="InternalQuizzerSummary"/> instance
    /// </summary>
    /// <param name="summary">The <see cref="Summary"/></param>
    /// <param name="quizzerSummary">The <see cref="QuizzerSummary"/></param>
    /// <returns>The <see cref="InternalQuizzerSummary"/></returns>
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

    /// <summary>
    /// Creates a collection of <see cref="InternalQuizzerSummary"/> instances
    /// </summary>
    /// <param name="summaries">The <see cref="IEnumerable{Summary}"/></param>
    /// <returns>The <see cref="Dictionary{int, InternalQuizzerSummary}"/></returns>
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

    /// <summary>
    /// Exports the summaries to a workbook.
    /// </summary>
    /// <param name="workbook">The <see cref="XLWorkbook"/></param>
    /// <param name="summaries">The <see cref="IEnumerable{Summary}"/></param>
    private static void Export(XLWorkbook workbook, IEnumerable<Summary> summaries)
    {
        var summaryNames = summaries.Select(x => x.Name).Distinct().ToArray();
        var worksheet = workbook.AddWorksheet("Summary");

        SetWorksheetHeaders(summaryNames, worksheet);
        SetWorksheetValues(summaries, summaryNames, worksheet);
    }

    /// <summary>
    /// Saves the workbook to the output folder.
    /// </summary>
    /// <param name="workbook">The <see cref="XLWorkbook"/></param>
    /// <param name="folder">The output folder</param>
    private static void SaveWorkbook(XLWorkbook workbook, string folder)
    {
        var filePath = Path.Combine(folder, "summary.xlsx");

        File.Delete(filePath);
        workbook.SaveAs(filePath);
    }

    /// <summary>
    /// Sets the worksheet headers
    /// </summary>
    /// <param name="summaryNames">The summary names</param>
    /// <param name="worksheet">The <see cref="IXLWorksheet"/></param>
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

    /// <summary>
    /// Sets the worksheet values
    /// </summary>
    /// <param name="summaries">The <see cref="IEnumerable{Summary}"/></param>
    /// <param name="summaryNames">The summary names</param>
    /// <param name="worksheet">The <see cref="IXLWorksheet"/></param>
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

    /// <summary>
    /// Defines the <see cref="InternalQuizzerSummary" />
    /// </summary>
    private class InternalQuizzerSummary
    {
        /// <summary>
        /// Gets or sets the Church
        /// </summary>
        public string Church { get; set; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the Results
        /// </summary>
        public Dictionary<string, QuizzerSummary> Results { get; } = new Dictionary<string, QuizzerSummary>();
    }
}
