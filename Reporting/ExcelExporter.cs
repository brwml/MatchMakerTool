using System;
using System.IO;
using System.Linq;
using ClosedXML.Excel;

namespace MatchMaker.Reporting
{
    public class ExcelExporter : IExporter
    {
        private const double QuizzerChurchColumnWidth = 20.0;

        private const double QuizzerNameColumnWidth = 20.0;

        private const double TeamNameColumnWidth = 20.0;

        public void Export(Summary summary, string folder)
        {
            using (var workbook = new XLWorkbook())
            {
                ExportTeamResults(workbook, summary);
                ExportQuizzerResults(workbook, summary);
                SaveFile(workbook, summary, folder);
            }
        }

        private static void ExportQuizzerResults(XLWorkbook workbook, Summary summary)
        {
            var worksheet = workbook.AddWorksheet("Quizzer Results");

            var row = 1;

            FillQuizzerHeaderRow(worksheet.Row(row++));
            FillQuizzerResults(summary, worksheet, row);
        }

        private static void ExportTeamResults(XLWorkbook workbook, Summary summary)
        {
            var worksheet = workbook.AddWorksheet("Team Results");

            var row = 1;

            FillTeamHeaderRow(worksheet.Row(row++));
            FillTeamResults(summary, worksheet, row);
        }

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

        private static int FillQuizzerResults(Summary summary, IXLWorksheet worksheet, int row)
        {
            var schedule = summary.Result.Schedule;
            var rookieYear = schedule.Rounds.Min(x => x.Value.StartTime).Subtract(TimeSpan.FromDays(180)).Year;

            foreach (var quizzerSummary in summary.QuizzerSummaries.OrderBy(s => s.Value.Place).Select(s => s.Value))
            {
                var quizzer = schedule.Quizzers[quizzerSummary.QuizzerId];
                var church = schedule.Churches.Values.FirstOrDefault(c => c.Id == quizzer.ChurchId);

                FillQuizzerRow(worksheet.Row(row++), quizzer, church, rookieYear, quizzerSummary);
            }

            return row;
        }

        private static void FillQuizzerRow(IXLRow row, Quizzer quizzer, Church church, int rookieYear, QuizzerSummary summary)
        {
            row.Cell(QuizzerColumns.Place).SetValue(summary.Place);
            row.Cell(QuizzerColumns.ID).SetValue(quizzer.Id);
            row.Cell(QuizzerColumns.Name).SetValue($"{quizzer.FirstName} {quizzer.LastName}");
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

            cellAverageScore.SetFormulaA1($"={cellScore.Address} / {cellRounds.Address}");
            cellAverageScore.Style.NumberFormat.NumberFormatId = 2;
            cellAverageErrors.SetFormulaA1($"{cellErrors.Address} / {cellRounds.Address}");
            cellAverageErrors.Style.NumberFormat.NumberFormatId = 2;
        }

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

        private static int FillTeamResults(Summary summary, IXLWorksheet worksheet, int row)
        {
            foreach (var team in summary.TeamSummaries.OrderBy(kvp => kvp.Value.Place).Select(kvp => kvp.Value))
            {
                FillTeamRow(worksheet.Row(row++), summary.Result.Schedule.Teams[team.TeamId], team);
            }

            return row;
        }

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
            cellPercentage.SetFormulaA1($"={cellWins.Address} / ({cellWins.Address} + {cellLosses.Address})");
            cellPercentage.Style.NumberFormat.NumberFormatId = 10;
            cellAverageScore.SetFormulaA1($"={cellScore.Address} / ({cellWins.Address} + {cellLosses.Address})");
            cellAverageScore.Style.NumberFormat.NumberFormatId = 2;
            cellAverageErrors.SetFormulaA1($"={cellErrors.Address} / ({cellWins.Address} + {cellLosses.Address})");
            cellAverageErrors.Style.NumberFormat.NumberFormatId = 2;
        }

        private static void SaveFile(XLWorkbook workbook, Summary summary, string folder)
        {
            var fileName = Path.Combine(folder, $"{summary.Name}.xlsx");
            File.Delete(fileName);
            workbook.SaveAs(fileName);
        }

        private static class QuizzerColumns
        {
            public const int AverageErrors = 10;
            public const int AverageScore = 9;
            public const int Church = 4;
            public const int ID = 2;
            public const int IsRookie = 5;
            public const int Name = 3;
            public const int Place = 1;
            public const int TotalErrors = 8;
            public const int TotalRounds = 6;
            public const int TotalScore = 7;
        }

        private static class TeamColumns
        {
            public const int Abbreviation = 3;
            public const int AverageErrors = 11;
            public const int AverageScore = 10;
            public const int ID = 2;
            public const int Losses = 6;
            public const int Name = 4;
            public const int Percentage = 9;
            public const int Place = 1;
            public const int TotalErrors = 8;
            public const int TotalScore = 7;
            public const int Wins = 5;
        }
    }
}