namespace MatchMaker.Reporting.Exporters;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using ClosedXML.Excel;

using MatchMaker.Models;

/// <summary>
/// Exports a roster of teams and their quizzers, arranged with one team per column, to an Excel
/// workbook. Used to export the alternate teams created for a consolation tournament.
/// </summary>
public static class AlternateTeamsExcelExporter
{
    /// <summary>
    /// Defines the roster column width.
    /// </summary>
    private const double ColumnWidth = 20.0;

    /// <summary>
    /// Defines the worksheet name.
    /// </summary>
    private const string WorksheetName = "Teams";

    /// <summary>
    /// Exports the teams and quizzers in the given <see cref="Schedule"/> to an Excel workbook,
    /// with one column per team, headed by the team name, and the team's quizzers listed
    /// alphabetically underneath.
    /// </summary>
    /// <param name="schedule">The schedule containing the teams and quizzers to export.</param>
    /// <param name="folder">The output folder.</param>
    public static void Export(Schedule schedule, string folder)
    {
        ArgumentNullException.ThrowIfNull(schedule);
        ArgumentNullException.ThrowIfNull(folder);

        Trace.WriteLine($"Exporting alternate team roster '{schedule.Name}' to Excel format");
        Trace.Indent();

        try
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.AddWorksheet(WorksheetName);

            FillRoster(worksheet, schedule);

            SaveFile(workbook, schedule, folder);
            Trace.WriteLine("Alternate team roster export completed successfully");
        }
        catch (Exception ex)
        {
            Trace.TraceError($"Error during alternate team roster export: {ex.Message}");
            throw;
        }
        finally
        {
            Trace.Unindent();
        }
    }

    /// <summary>
    /// Fills the worksheet with one column per team, containing the team's name as the header
    /// and its quizzers listed alphabetically by last name, then first name.
    /// </summary>
    /// <param name="worksheet">The worksheet to fill.</param>
    /// <param name="schedule">The schedule containing the teams and quizzers.</param>
    private static void FillRoster(IXLWorksheet worksheet, Schedule schedule)
    {
        // Ordered by team identifier rather than name, since team names beyond "Team Z" use
        // multi-letter suffixes ("Team AA", "Team AB", ...) that do not sort correctly as strings.
        var teams = schedule.Teams.Values.OrderBy(x => x.Id).ToArray();

        for (var column = 0; column < teams.Length; column++)
        {
            var team = teams[column];
            var columnIndex = column + 1;

            var headerCell = worksheet.Cell(1, columnIndex);
            headerCell.SetValue(team.Name);
            headerCell.WorksheetColumn().Width = ColumnWidth;

            var roster = schedule.Quizzers.Values
                .Where(x => x.TeamId == team.Id)
                .OrderBy(x => x.LastName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(x => x.FirstName, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            for (var row = 0; row < roster.Length; row++)
            {
                var quizzer = roster[row];
                worksheet.Cell(row + 2, columnIndex).SetValue(FormattableString.Invariant($"{quizzer.FirstName} {quizzer.LastName}"));
            }
        }
    }

    /// <summary>
    /// Saves the workbook to the output folder, using the schedule name as the file name.
    /// </summary>
    /// <param name="workbook">The workbook to save.</param>
    /// <param name="schedule">The schedule providing the file name.</param>
    /// <param name="folder">The output folder.</param>
    private static void SaveFile(XLWorkbook workbook, Schedule schedule, string folder)
    {
        var fileName = Path.Combine(folder, FormattableString.Invariant($"{schedule.Name}.xlsx"));

        Trace.WriteLine($"Saving alternate team roster workbook to: {fileName}");

        if (File.Exists(fileName))
        {
            File.Delete(fileName);
            Trace.WriteLine("Existing file deleted");
        }

        workbook.SaveAs(fileName);
        Trace.WriteLine("Alternate team roster workbook saved successfully");
    }
}
