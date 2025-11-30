namespace MatchMaker.Reporting.Exporters;

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
        Trace.WriteLine("Starting summary export to Excel");
        Trace.Indent();

        try
        {
            using var workbook = new XLWorkbook();
            Export(workbook, summaries);
            SaveWorkbook(workbook, folder);
            Trace.WriteLine("Summary export completed successfully");
        }
        catch (Exception ex)
        {
            Trace.TraceError($"Error during summary export: {ex.Message}");
            throw;
        }
        finally
        {
            Trace.Unindent();
        }
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
        return new InternalQuizzerSummary(FormattableString.Invariant($"{quizzer.LastName}, {quizzer.FirstName}"), church.Name);
    }

    /// <summary>
    /// Creates a collection of <see cref="InternalQuizzerSummary"/> instances
    /// </summary>
    /// <param name="summaries">The <see cref="IEnumerable{Summary}"/></param>
    /// <returns>The <see cref="Dictionary{int, InternalQuizzerSummary}"/></returns>
    private static Dictionary<int, InternalQuizzerSummary> CreateQuizzerSummaries(IEnumerable<Summary> summaries)
    {
        Trace.WriteLine("Creating quizzer summaries");
        var quizzerSummaries = new Dictionary<int, InternalQuizzerSummary>();

        foreach (var summary in summaries)
        {
            foreach (var quizzerSummary in summary.QuizzerSummaries)
            {
                if (quizzerSummaries.TryGetValue(quizzerSummary.Key, out var quizzer))
                {
                    quizzer.Results.Add(summary.Name, quizzerSummary.Value);
                }
                else
                {
                    quizzerSummaries.Add(quizzerSummary.Key, CreateInternalQuizzerSummary(summary, quizzerSummary.Value));
                }
            }
        }

        Trace.WriteLine($"Created summaries for {quizzerSummaries.Count} quizzers");
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
        Trace.WriteLine($"Exporting {summaryNames.Length} tournament summaries to workbook");
        
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

        Trace.WriteLine($"Saving workbook to: {filePath}");

        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Trace.WriteLine("Existing file deleted");
            }

            workbook.SaveAs(filePath);
            Trace.WriteLine("Workbook saved successfully");
        }
        catch (Exception ex)
        {
            Trace.TraceError($"Error saving workbook: {ex.Message}");
            throw;
        }
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

        Trace.WriteLine($"Worksheet headers set with {summaryNames.Length + 3} columns");
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
                if (item.Value.Results.TryGetValue(summaryNames[i], out var results))
                {
                    worksheet.Cell(row, i + 4).Value = results.AverageScore;
                }
            }

            row++;
        }

        Trace.WriteLine($"Populated worksheet with {row - 2} quizzer records");
    }

    /// <summary>
    /// Defines the <see cref="InternalQuizzerSummary" />
    /// </summary>
    /// <remarks>
    /// Initializes an instance of the <see cref="InternalQuizzerSummary"/> class.
    /// </remarks>
    /// <param name="name">The quizzer name</param>
    /// <param name="church">The quizzer church</param>
    private class InternalQuizzerSummary(string name, string church)
    {
        /// <summary>
        /// Gets or sets the Church
        /// </summary>
        public string Church { get; set; } = church;

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name { get; set; } = name;

        /// <summary>
        /// Gets the Results
        /// </summary>
        public Dictionary<string, QuizzerSummary> Results { get; } = [];
    }
}
