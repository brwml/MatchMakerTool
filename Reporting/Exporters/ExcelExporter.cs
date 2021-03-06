﻿namespace MatchMaker.Reporting.Exporters
{
    using System;
    using System.IO;
    using System.Linq;

    using ClosedXML.Excel;

    using MatchMaker.Reporting.Models;
    using MatchMaker.Utilities;

    /// <summary>
    /// Defines the <see cref="ExcelExporter" />
    /// </summary>
    public class ExcelExporter : IExporter
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
        /// Defines the team name column width
        /// </summary>
        private const double TeamNameColumnWidth = 20.0;

        /// <summary>
        /// Exports the <see cref="Summary"/> instance
        /// </summary>
        /// <param name="summary">The summary/></param>
        /// <param name="folder">The folder/></param>
        public void Export(Summary summary, string folder)
        {
            Arg.NotNull(summary, nameof(summary));

            using (var workbook = new XLWorkbook())
            {
                ExportTeamResults(workbook, summary);
                ExportQuizzerResults(workbook, summary);
                SaveFile(workbook, summary, folder);
            }
        }

        /// <summary>
        /// Exports the quizzer results.
        /// </summary>
        /// <param name="workbook">The workbook/></param>
        /// <param name="summary">The summary/></param>
        protected static void ExportQuizzerResults(XLWorkbook workbook, Summary summary)
        {
            Arg.NotNull(workbook, nameof(workbook));
            Arg.NotNull(summary, nameof(summary));

            var worksheet = workbook.AddWorksheet("Quizzer Results");

            var row = 1;

            FillQuizzerHeaderRow(worksheet.Row(row++));
            FillQuizzerResults(summary, worksheet, row);
        }

        /// <summary>
        /// Exports the team results
        /// </summary>
        /// <param name="workbook">The workbook/></param>
        /// <param name="summary">The summary/></param>
        private static void ExportTeamResults(XLWorkbook workbook, Summary summary)
        {
            var worksheet = workbook.AddWorksheet("Team Results");

            var row = 1;

            FillTeamHeaderRow(worksheet.Row(row++));
            FillTeamResults(summary, worksheet, row);
        }

        /// <summary>
        /// Fills the quizzer header row
        /// </summary>
        /// <param name="row">The row/></param>
        private static void FillQuizzerHeaderRow(IXLRow row)
        {
            row.Cell(QuizzerColumns.Place).SetValue(nameof(QuizzerColumns.Place));
            row.Cell(QuizzerColumns.ID).SetValue(nameof(QuizzerColumns.ID));
            row.Cell(QuizzerColumns.Name).SetValue(nameof(QuizzerColumns.Name));
            row.Cell(QuizzerColumns.Church).SetValue(nameof(QuizzerColumns.Church));
            row.Cell(QuizzerColumns.IsRookie).SetValue(nameof(QuizzerColumns.IsRookie));
            row.Cell(QuizzerColumns.TotalRounds).SetValue(nameof(QuizzerColumns.TotalRounds));
            row.Cell(QuizzerColumns.TotalScore).SetValue(nameof(QuizzerColumns.TotalScore));
            row.Cell(QuizzerColumns.TotalErrors).SetValue(nameof(QuizzerColumns.TotalErrors));
            row.Cell(QuizzerColumns.AverageScore).SetValue(nameof(QuizzerColumns.AverageScore));
            row.Cell(QuizzerColumns.AverageErrors).SetValue(nameof(QuizzerColumns.AverageErrors));

            row.Cell(QuizzerColumns.Name).WorksheetColumn().Width = QuizzerNameColumnWidth;
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
            var schedule = summary.Result.Schedule;
            var rookieYear = schedule.Rounds.Min(x => x.Value.StartTime).Subtract(TimeSpan.FromDays(180)).Year;

            foreach (var quizzerSummary in summary.QuizzerSummaries.OrderBy(s => s.Value.Place).Select(s => s.Value))
            {
                var quizzer = schedule.Quizzers[quizzerSummary.QuizzerId];
                var church = schedule.Churches.Values.FirstOrDefault(c => c.Id == quizzer.ChurchId);
                var team = schedule.Teams.Values.FirstOrDefault(t => t.Id == quizzer.TeamId);

                FillQuizzerRow(worksheet.Row(row++), quizzer, team, church, rookieYear, quizzerSummary);
            }

            return row;
        }

        /// <summary>
        /// Fills the quizzer row
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="quizzer">The quizzer</param>
        /// <param name="team">The team</param>
        /// <param name="church">The church</param>
        /// <param name="rookieYear">The rookie year</param>
        /// <param name="summary">The summary</param>
        private static void FillQuizzerRow(IXLRow row, Quizzer quizzer, Team team, Church church, int rookieYear, QuizzerSummary summary)
        {
            row.Cell(QuizzerColumns.Place).SetValue(summary.Place);
            row.Cell(QuizzerColumns.ID).SetValue(quizzer.Id);
            row.Cell(QuizzerColumns.Name).SetValue(FormattableString.Invariant($"{quizzer.FirstName} {quizzer.LastName}"));
            row.Cell(QuizzerColumns.Team).SetValue(team?.Name ?? string.Empty);
            row.Cell(QuizzerColumns.Church).SetValue(church?.Name ?? string.Empty);
            row.Cell(QuizzerColumns.IsRookie).SetValue(rookieYear == quizzer.RookieYear ? "R" : string.Empty);

            var cellRounds = row.Cell(QuizzerColumns.TotalRounds);
            var cellScore = row.Cell(QuizzerColumns.TotalScore);
            var cellErrors = row.Cell(QuizzerColumns.TotalErrors);
            var cellAverageScore = row.Cell(QuizzerColumns.AverageScore);
            var cellAverageErrors = row.Cell(QuizzerColumns.AverageErrors);

            cellRounds.SetValue(summary.TotalRounds);
            cellScore.SetValue(summary.TotalScore);
            cellErrors.SetValue(summary.TotalErrors);

            cellAverageScore.SetFormulaA1(FormattableString.Invariant($"={cellScore.Address} / {cellRounds.Address}"));
            cellAverageScore.Style.NumberFormat.NumberFormatId = 2;
            cellAverageErrors.SetFormulaA1(FormattableString.Invariant($"{cellErrors.Address} / {cellRounds.Address}"));
            cellAverageErrors.Style.NumberFormat.NumberFormatId = 2;
        }

        /// <summary>
        /// Fills the team header row
        /// </summary>
        /// <param name="row">The row</param>
        private static void FillTeamHeaderRow(IXLRow row)
        {
            row.Cell(TeamColumns.Place).SetValue(nameof(TeamColumns.Place));
            row.Cell(TeamColumns.ID).SetValue(nameof(TeamColumns.ID));
            row.Cell(TeamColumns.Abbreviation).SetValue(nameof(TeamColumns.Abbreviation));
            row.Cell(TeamColumns.Name).SetValue(nameof(TeamColumns.Name));
            row.Cell(TeamColumns.Wins).SetValue(nameof(TeamColumns.Wins));
            row.Cell(TeamColumns.Losses).SetValue(nameof(TeamColumns.Losses));
            row.Cell(TeamColumns.TotalScore).SetValue(nameof(TeamColumns.TotalScore));
            row.Cell(TeamColumns.TotalErrors).SetValue(nameof(TeamColumns.TotalErrors));
            row.Cell(TeamColumns.Percentage).SetValue(nameof(TeamColumns.Percentage));
            row.Cell(TeamColumns.AverageScore).SetValue(nameof(TeamColumns.AverageScore));
            row.Cell(TeamColumns.AverageErrors).SetValue(nameof(TeamColumns.AverageErrors));

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
            foreach (var team in summary.TeamSummaries.OrderBy(kvp => kvp.Value.Place).Select(kvp => kvp.Value))
            {
                FillTeamRow(worksheet.Row(row++), summary.Result.Schedule.Teams[team.TeamId], team);
            }

            return row;
        }

        /// <summary>
        /// Fills the team row
        /// </summary>
        /// <param name="row">The row</param>
        /// <param name="team">The team</param>
        /// <param name="summary">The summary</param>
        private static void FillTeamRow(IXLRow row, Team team, TeamSummary summary)
        {
            row.Cell(TeamColumns.Place).SetValue(summary.Place);
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

            cellWins.SetValue(summary.Wins);
            cellLosses.SetValue(summary.Losses);
            cellScore.SetValue(summary.TotalScore);
            cellErrors.SetValue(summary.TotalErrors);
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
}
