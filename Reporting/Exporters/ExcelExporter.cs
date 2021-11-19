namespace MatchMaker.Reporting.Exporters;

using System;
using System.IO;

using Ardalis.GuardClauses;

using ClosedXML.Excel;

using Humanizer;

using MatchMaker.Reporting.Models;

/// <summary>
/// Defines the <see cref="ExcelExporter" />
/// </summary>
public class ExcelExporter : BaseExporter
{
    /// <summary>
    /// Defines the quizzer church column width
    /// </summary>
    private const double QuizzerChurchColumnWidth = 20.0;

    /// <summary>
    /// Defines the quizzer name column width
    /// </summary>
    private const double QuizzerNameColumnWidth = 20.0;

    /// <summary>
    /// Defines the quizzer team column width
    /// </summary>
    private const double QuizzerTeamColumnWidth = 20.0;

    /// <summary>
    /// Defines the team name column width
    /// </summary>
    private const double TeamNameColumnWidth = 20.0;

    /// <summary>
    /// Exports the <see cref="Summary"/> instance
    /// </summary>
    /// <param name="summary">The summary/></param>
    /// <param name="folder">The folder/></param>
    public override void Export(Summary summary, string folder)
    {
        Guard.Against.Null(summary, nameof(summary));
        Guard.Against.NullOrWhiteSpace(folder, nameof(folder));

        using var workbook = new XLWorkbook();
        ExportTeamResults(workbook, summary);
        ExportQuizzerResults(workbook, summary);
        SaveFile(workbook, summary, folder);
    }

    /// <summary>
    /// Exports the quizzer results.
    /// </summary>
    /// <param name="workbook">The workbook/></param>
    /// <param name="summary">The summary/></param>
    private static void ExportQuizzerResults(XLWorkbook workbook, Summary summary)
    {
        var worksheet = workbook.AddWorksheet("Quizzer Results");

        FillQuizzerHeaderRow(worksheet.Row(1));
        FillQuizzerResults(summary, worksheet, 2);
    }

    /// <summary>
    /// Exports the team results
    /// </summary>
    /// <param name="workbook">The workbook/></param>
    /// <param name="summary">The summary/></param>
    private static void ExportTeamResults(XLWorkbook workbook, Summary summary)
    {
        var worksheet = workbook.AddWorksheet("Team Results");

        FillTeamHeaderRow(worksheet.Row(1));
        FillTeamResults(summary, worksheet, 2);
    }

    /// <summary>
    /// Fills the quizzer header row
    /// </summary>
    /// <param name="row">The row/></param>
    private static void FillQuizzerHeaderRow(IXLRow row)
    {
        row.Cell(QuizzerColumns.Place).SetValue(nameof(QuizzerColumns.Place).Titleize());
        row.Cell(QuizzerColumns.ID).SetValue(nameof(QuizzerColumns.ID).Titleize());
        row.Cell(QuizzerColumns.Name).SetValue(nameof(QuizzerColumns.Name).Titleize());
        row.Cell(QuizzerColumns.Team).SetValue(nameof(QuizzerColumns.Team).Titleize());
        row.Cell(QuizzerColumns.Church).SetValue(nameof(QuizzerColumns.Church).Titleize());
        row.Cell(QuizzerColumns.IsRookie).SetValue(nameof(QuizzerColumns.IsRookie).Titleize());
        row.Cell(QuizzerColumns.TotalRounds).SetValue(nameof(QuizzerColumns.TotalRounds).Titleize());
        row.Cell(QuizzerColumns.TotalScore).SetValue(nameof(QuizzerColumns.TotalScore).Titleize());
        row.Cell(QuizzerColumns.TotalErrors).SetValue(nameof(QuizzerColumns.TotalErrors).Titleize());
        row.Cell(QuizzerColumns.AverageScore).SetValue(nameof(QuizzerColumns.AverageScore).Titleize());
        row.Cell(QuizzerColumns.AverageErrors).SetValue(nameof(QuizzerColumns.AverageErrors).Titleize());

        row.Cell(QuizzerColumns.Name).WorksheetColumn().Width = QuizzerNameColumnWidth;
        row.Cell(QuizzerColumns.Team).WorksheetColumn().Width = QuizzerTeamColumnWidth;
        row.Cell(QuizzerColumns.Church).WorksheetColumn().Width = QuizzerChurchColumnWidth;
        row.Cell(QuizzerColumns.ID).WorksheetColumn().Hide();
    }

    /// <summary>
    /// Fills the quizzer results
    /// </summary>
    /// <param name="summary">The summary/></param>
    /// <param name="worksheet">The worksheet/></param>
    /// <param name="row">The row/></param>
    /// <returns>The <see cref="int"/> indicating the number of rows</returns>
    private static int FillQuizzerResults(Summary summary, IXLWorksheet worksheet, int row)
    {
        var quizzers = GetQuizzerInfo(summary);

        foreach (var quizzer in quizzers)
        {
            FillQuizzerRow(worksheet.Row(row++), quizzer);
        }

        return row;
    }

    /// <summary>
    /// Fills the quizzer row
    /// </summary>
    /// <param name="row">The row</param>
    /// <param name="quizzer">The quizzer</param>
    private static void FillQuizzerRow(IXLRow row, QuizzerInfo quizzer)
    {
        row.Cell(QuizzerColumns.Place).SetValue(quizzer.Place);
        row.Cell(QuizzerColumns.ID).SetValue(quizzer.Id);
        row.Cell(QuizzerColumns.Name).SetValue(FormattableString.Invariant($"{quizzer.FirstName} {quizzer.LastName}"));
        row.Cell(QuizzerColumns.Team).SetValue(quizzer.Team?.Name ?? string.Empty);
        row.Cell(QuizzerColumns.Church).SetValue(quizzer.Church?.Name ?? string.Empty);
        row.Cell(QuizzerColumns.IsRookie).SetValue(quizzer.IsRookie ? "R" : string.Empty);

        var cellRounds = row.Cell(QuizzerColumns.TotalRounds);
        var cellScore = row.Cell(QuizzerColumns.TotalScore);
        var cellErrors = row.Cell(QuizzerColumns.TotalErrors);
        var cellAverageScore = row.Cell(QuizzerColumns.AverageScore);
        var cellAverageErrors = row.Cell(QuizzerColumns.AverageErrors);

        cellRounds.SetValue(quizzer.TotalRounds);
        cellScore.SetValue(quizzer.TotalScore);
        cellErrors.SetValue(quizzer.TotalErrors);

        cellAverageScore.SetFormulaA1(FormattableString.Invariant($"={cellScore.Address} / {cellRounds.Address}"));
        cellAverageScore.Style.NumberFormat.NumberFormatId = 2;
        cellAverageErrors.SetFormulaA1(FormattableString.Invariant($"={cellErrors.Address} / {cellRounds.Address}"));
        cellAverageErrors.Style.NumberFormat.NumberFormatId = 2;
    }

    /// <summary>
    /// Fills the team header row
    /// </summary>
    /// <param name="row">The row</param>
    private static void FillTeamHeaderRow(IXLRow row)
    {
        row.Cell(TeamColumns.Place).SetValue(nameof(TeamColumns.Place).Titleize());
        row.Cell(TeamColumns.ID).SetValue(nameof(TeamColumns.ID).Titleize());
        row.Cell(TeamColumns.Abbreviation).SetValue(nameof(TeamColumns.Abbreviation).Titleize());
        row.Cell(TeamColumns.Name).SetValue(nameof(TeamColumns.Name).Titleize());
        row.Cell(TeamColumns.Wins).SetValue(nameof(TeamColumns.Wins).Titleize());
        row.Cell(TeamColumns.Losses).SetValue(nameof(TeamColumns.Losses).Titleize());
        row.Cell(TeamColumns.TotalScore).SetValue(nameof(TeamColumns.TotalScore).Titleize());
        row.Cell(TeamColumns.TotalErrors).SetValue(nameof(TeamColumns.TotalErrors).Titleize());
        row.Cell(TeamColumns.Percentage).SetValue(nameof(TeamColumns.Percentage).Titleize());
        row.Cell(TeamColumns.AverageScore).SetValue(nameof(TeamColumns.AverageScore).Titleize());
        row.Cell(TeamColumns.AverageErrors).SetValue(nameof(TeamColumns.AverageErrors).Titleize());

        row.Cell(TeamColumns.Name).WorksheetColumn().Width = TeamNameColumnWidth;
        row.Cell(TeamColumns.ID).WorksheetColumn().Hide();
    }

    /// <summary>
    /// Fills the team results
    /// </summary>
    /// <param name="summary">The summary</param>
    /// <param name="worksheet">The worksheet</param>
    /// <param name="row">The row</param>
    /// <returns>The <see cref="int"/></returns>
    private static int FillTeamResults(Summary summary, IXLWorksheet worksheet, int row)
    {
        var teams = GetTeamInfo(summary);

        foreach (var team in teams)
        {
            FillTeamRow(worksheet.Row(row++), team);
        }

        return row;
    }

    /// <summary>
    /// Fills the team row
    /// </summary>
    /// <param name="row">The row</param>
    /// <param name="team">The team</param>
    /// <param name="summary">The summary</param>
    private static void FillTeamRow(IXLRow row, TeamInfo team)
    {
        row.Cell(TeamColumns.Place).SetValue(team.Place);
        row.Cell(TeamColumns.ID).SetValue(team.Id);
        row.Cell(TeamColumns.Abbreviation).SetValue(team.Abbreviation);
        row.Cell(TeamColumns.Name).SetValue(team.Name);

        var cellWins = row.Cell(TeamColumns.Wins);
        var cellLosses = row.Cell(TeamColumns.Losses);
        var cellScore = row.Cell(TeamColumns.TotalScore);
        var cellErrors = row.Cell(TeamColumns.TotalErrors);
        var cellPercentage = row.Cell(TeamColumns.Percentage);
        var cellAverageScore = row.Cell(TeamColumns.AverageScore);
        var cellAverageErrors = row.Cell(TeamColumns.AverageErrors);

        cellWins.SetValue(team.Wins);
        cellLosses.SetValue(team.Losses);
        cellScore.SetValue(team.TotalScore);
        cellErrors.SetValue(team.TotalErrors);
        cellPercentage.SetFormulaA1(FormattableString.Invariant($"={cellWins.Address} / ({cellWins.Address} + {cellLosses.Address})"));
        cellPercentage.Style.NumberFormat.NumberFormatId = 10;
        cellAverageScore.SetFormulaA1(FormattableString.Invariant($"={cellScore.Address} / ({cellWins.Address} + {cellLosses.Address})"));
        cellAverageScore.Style.NumberFormat.NumberFormatId = 2;
        cellAverageErrors.SetFormulaA1(FormattableString.Invariant($"={cellErrors.Address} / ({cellWins.Address} + {cellLosses.Address})"));
        cellAverageErrors.Style.NumberFormat.NumberFormatId = 2;
    }

    /// <summary>
    /// Saves the file
    /// </summary>
    /// <param name="workbook">The workbook</param>
    /// <param name="summary">The summary</param>
    /// <param name="folder">The folder</param>
    private static void SaveFile(XLWorkbook workbook, Summary summary, string folder)
    {
        var fileName = Path.Combine(folder, FormattableString.Invariant($"{summary.Name}.xlsx"));
        File.Delete(fileName);
        workbook.SaveAs(fileName);
    }

    /// <summary>
    /// Defines the <see cref="QuizzerColumns" />
    /// </summary>
    private static class QuizzerColumns
    {
        /// <summary>
        /// Defines the AverageErrors
        /// </summary>
        public const int AverageErrors = 11;

        /// <summary>
        /// Defines the AverageScore
        /// </summary>
        public const int AverageScore = 10;

        /// <summary>
        /// Defines the Church
        /// </summary>
        public const int Church = 5;

        /// <summary>
        /// Defines the ID
        /// </summary>
        public const int ID = 2;

        /// <summary>
        /// Defines the IsRookie
        /// </summary>
        public const int IsRookie = 6;

        /// <summary>
        /// Defines the Name
        /// </summary>
        public const int Name = 3;

        /// <summary>
        /// Defines the Place
        /// </summary>
        public const int Place = 1;

        /// <summary>
        /// Defines the Team
        /// </summary>
        public const int Team = 4;

        /// <summary>
        /// Defines the TotalErrors
        /// </summary>
        public const int TotalErrors = 9;

        /// <summary>
        /// Defines the TotalRounds
        /// </summary>
        public const int TotalRounds = 7;

        /// <summary>
        /// Defines the TotalScore
        /// </summary>
        public const int TotalScore = 8;
    }

    /// <summary>
    /// Defines the <see cref="TeamColumns" />
    /// </summary>
    private static class TeamColumns
    {
        /// <summary>
        /// Defines the Abbreviation
        /// </summary>
        public const int Abbreviation = 3;

        /// <summary>
        /// Defines the AverageErrors
        /// </summary>
        public const int AverageErrors = 11;

        /// <summary>
        /// Defines the AverageScore
        /// </summary>
        public const int AverageScore = 10;

        /// <summary>
        /// Defines the ID
        /// </summary>
        public const int ID = 2;

        /// <summary>
        /// Defines the Losses
        /// </summary>
        public const int Losses = 6;

        /// <summary>
        /// Defines the Name
        /// </summary>
        public const int Name = 4;

        /// <summary>
        /// Defines the Percentage
        /// </summary>
        public const int Percentage = 9;

        /// <summary>
        /// Defines the Place
        /// </summary>
        public const int Place = 1;

        /// <summary>
        /// Defines the TotalErrors
        /// </summary>
        public const int TotalErrors = 8;

        /// <summary>
        /// Defines the TotalScore
        /// </summary>
        public const int TotalScore = 7;

        /// <summary>
        /// Defines the Wins
        /// </summary>
        public const int Wins = 5;
    }
}
