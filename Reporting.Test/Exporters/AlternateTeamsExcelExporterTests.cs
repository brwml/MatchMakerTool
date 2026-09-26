namespace Reporting.Test.Exporters;

using System;
using System.Collections.Generic;
using System.IO;

using ClosedXML.Excel;

using MatchMaker.Models;
using MatchMaker.Reporting.Exporters;

using Xunit;

public class AlternateTeamsExcelExporterTests
{
    [Fact]
    public void Export_WithNullSchedule_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => AlternateTeamsExcelExporter.Export(null!, Path.GetTempPath()));
    }

    [Fact]
    public void Export_WithNullFolder_ThrowsArgumentNullException()
    {
        var schedule = CreateSchedule();
        Assert.Throws<ArgumentNullException>(() => AlternateTeamsExcelExporter.Export(schedule, null!));
    }

    [Fact]
    public void Export_CreatesWorkbookNamedAfterSchedule()
    {
        var schedule = CreateSchedule();
        var tempDir = CreateTempDirectory();

        try
        {
            AlternateTeamsExcelExporter.Export(schedule, tempDir);

            var path = Path.Combine(tempDir, $"{schedule.Name}.xlsx");
            Assert.True(File.Exists(path));
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void Export_WritesTeamNamesAsColumnHeaders()
    {
        var schedule = CreateSchedule();
        var tempDir = CreateTempDirectory();

        try
        {
            AlternateTeamsExcelExporter.Export(schedule, tempDir);

            using var workbook = new XLWorkbook(Path.Combine(tempDir, $"{schedule.Name}.xlsx"));
            var worksheet = workbook.Worksheet("Teams");

            Assert.Equal("Team A", worksheet.Cell(1, 1).GetString());
            Assert.Equal("Team B", worksheet.Cell(1, 2).GetString());
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void Export_ListsTeamMembersInAlphabeticalOrder()
    {
        var schedule = CreateSchedule();
        var tempDir = CreateTempDirectory();

        try
        {
            AlternateTeamsExcelExporter.Export(schedule, tempDir);

            using var workbook = new XLWorkbook(Path.Combine(tempDir, $"{schedule.Name}.xlsx"));
            var worksheet = workbook.Worksheet("Teams");

            // Team A quizzers: Charlie Adams (teamId 1), Alice Brown (teamId 1) -> alphabetical by last name: Adams, Brown
            Assert.Equal("Charlie Adams", worksheet.Cell(2, 1).GetString());
            Assert.Equal("Alice Brown", worksheet.Cell(3, 1).GetString());
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void Export_OrdersColumnsByTeamId()
    {
        var churches = new Dictionary<int, Church> { { 1, new Church(1, "Church 1") } };
        var teams = new Dictionary<int, Team>
        {
            { 1, new Team(1, "Team B", "B", 0) },
            { 2, new Team(2, "Team A", "A", 0) }
        };
        var quizzers = new Dictionary<int, Quizzer>();
        var schedule = new Schedule("Column Order Test", churches, quizzers, teams, new Dictionary<int, Round>());
        var tempDir = CreateTempDirectory();

        try
        {
            AlternateTeamsExcelExporter.Export(schedule, tempDir);

            using var workbook = new XLWorkbook(Path.Combine(tempDir, $"{schedule.Name}.xlsx"));
            var worksheet = workbook.Worksheet("Teams");

            // Columns are ordered by team identifier, not by name, so team 1 ("Team B") comes
            // before team 2 ("Team A").
            Assert.Equal("Team B", worksheet.Cell(1, 1).GetString());
            Assert.Equal("Team A", worksheet.Cell(1, 2).GetString());
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void Export_OrdersColumnsByTeamId_NotByNameStringComparison()
    {
        // "Team AA" (id 27) would sort before "Team Z" (id 26) as a string, but the export must
        // order columns by team identifier so multi-letter team names beyond "Team Z" still
        // appear after every single-letter team.
        var churches = new Dictionary<int, Church> { { 1, new Church(1, "Church 1") } };
        var teams = new Dictionary<int, Team>
        {
            { 27, new Team(27, "Team AA", "AA", 0) },
            { 26, new Team(26, "Team Z", "Z", 0) }
        };
        var quizzers = new Dictionary<int, Quizzer>();
        var schedule = new Schedule("Large Column Order Test", churches, quizzers, teams, new Dictionary<int, Round>());
        var tempDir = CreateTempDirectory();

        try
        {
            AlternateTeamsExcelExporter.Export(schedule, tempDir);

            using var workbook = new XLWorkbook(Path.Combine(tempDir, $"{schedule.Name}.xlsx"));
            var worksheet = workbook.Worksheet("Teams");

            Assert.Equal("Team Z", worksheet.Cell(1, 1).GetString());
            Assert.Equal("Team AA", worksheet.Cell(1, 2).GetString());
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void Export_OverwritesExistingFile()
    {
        var schedule = CreateSchedule();
        var tempDir = CreateTempDirectory();

        try
        {
            AlternateTeamsExcelExporter.Export(schedule, tempDir);
            AlternateTeamsExcelExporter.Export(schedule, tempDir);

            var path = Path.Combine(tempDir, $"{schedule.Name}.xlsx");
            Assert.True(File.Exists(path));
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    private static Schedule CreateSchedule()
    {
        var churches = new Dictionary<int, Church> { { 1, new Church(1, "Church 1") } };

        var teams = new Dictionary<int, Team>
        {
            { 1, new Team(1, "Team A", "A", 0) },
            { 2, new Team(2, "Team B", "B", 0) }
        };

        var quizzers = new Dictionary<int, Quizzer>
        {
            { 1, new Quizzer(1, "Alice", "Brown", Gender.Female, DateTime.Now.Year, 1, 1) },
            { 2, new Quizzer(2, "Charlie", "Adams", Gender.Male, DateTime.Now.Year, 1, 1) },
            { 3, new Quizzer(3, "Bob", "Zeta", Gender.Male, DateTime.Now.Year, 2, 1) }
        };

        return new Schedule("Alternate Teams Test", churches, quizzers, teams, new Dictionary<int, Round>());
    }

    private static string CreateTempDirectory()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        return tempDir;
    }

    private static void CleanupDirectory(string directory)
    {
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
        }
    }
}
