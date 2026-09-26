namespace Reporting.Test.Exporters;

using System;
using System.Collections.Generic;
using System.IO;

using ClosedXML.Excel;

using MatchMaker.Models;
using MatchMaker.Reporting.Exporters;
using MatchMaker.Reporting.Models;
using MatchMaker.Reporting.Policies;

using Xunit;

public class ExcelSummaryExporterTests
{
    [Fact]
    public void Export_CreatesXlsxFile()
    {
        var exporter = new ExcelSummaryExporter();
        var summary = CreateTestSummary("Test Tournament");
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var path = Path.Combine(tempDir, $"{summary.Name}.xlsx");
            Assert.True(File.Exists(path));
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void Export_WritesTeamResultsWorksheet()
    {
        var exporter = new ExcelSummaryExporter();
        var summary = CreateTestSummary("Test Tournament");
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            using var workbook = new XLWorkbook(Path.Combine(tempDir, $"{summary.Name}.xlsx"));
            var worksheet = workbook.Worksheet("Team Results");

            Assert.Equal("Place", worksheet.Cell(1, 1).GetString());
            Assert.Equal("Team 1", worksheet.Cell(2, 4).GetString());
            Assert.Equal("Team 2", worksheet.Cell(3, 4).GetString());
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void Export_WritesQuizzerResultsWorksheet()
    {
        var exporter = new ExcelSummaryExporter();
        var summary = CreateTestSummary("Test Tournament");
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            using var workbook = new XLWorkbook(Path.Combine(tempDir, $"{summary.Name}.xlsx"));
            var worksheet = workbook.Worksheet("Quizzer Results");

            Assert.Equal("Place", worksheet.Cell(1, 1).GetString());
            Assert.Equal("Alice Adams", worksheet.Cell(2, 3).GetString());
            Assert.Equal("Bob Brown", worksheet.Cell(3, 3).GetString());
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void Export_WhenFileAlreadyExists_OverwritesFile()
    {
        var exporter = new ExcelSummaryExporter();
        var summary = CreateTestSummary("Test Tournament");
        var tempDir = CreateTempDirectory();

        try
        {
            var path = Path.Combine(tempDir, $"{summary.Name}.xlsx");
            File.WriteAllText(path, "placeholder");

            exporter.Export(summary, tempDir);

            using var workbook = new XLWorkbook(path);
            Assert.NotNull(workbook.Worksheet("Team Results"));
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    private static Summary CreateTestSummary(string name)
    {
        var churches = new Dictionary<int, Church>
        {
            { 1, new Church(1, "Church 1") },
            { 2, new Church(2, "Church 2") }
        };

        var teams = new Dictionary<int, Team>
        {
            { 1, new Team(1, "Team 1", "T1", 0) },
            { 2, new Team(2, "Team 2", "T2", 0) }
        };

        var quizzers = new Dictionary<int, Quizzer>
        {
            { 1, new Quizzer(1, "Alice", "Adams", Gender.Female, DateTime.Now.Year, 1, 1) },
            { 2, new Quizzer(2, "Bob", "Brown", Gender.Male, DateTime.Now.Year, 2, 2) }
        };

        var round = new Round(1, new Dictionary<int, MatchSchedule>(), DateOnly.FromDateTime(DateTime.Now), TimeOnly.FromDateTime(DateTime.Now));
        var rounds = new Dictionary<int, Round> { { 1, round } };

        var schedule = new Schedule(name, churches, quizzers, teams, rounds);

        var teamResults = new List<TeamResult>
        {
            new(1, 90, 0, 1),
            new(2, 80, 1, 2)
        };
        var quizzerResults = new List<QuizzerResult>
        {
            new(1, 90, 0),
            new(2, 80, 1)
        };
        var matchResult = new MatchResult(1, 1, 1, teamResults, quizzerResults);
        var matches = new Dictionary<int, MatchResult> { { 1, matchResult } };

        var result = new Result(schedule, matches);
        var policies = new TeamRankingPolicy[] { new WinPercentageTeamRankingPolicy() };

        return Summary.FromResult(result, policies);
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
