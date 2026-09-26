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

public class SummaryExporterTests
{
    [Fact]
    public void Export_CreatesSummaryXlsxFile()
    {
        var summary = CreateTestSummary("Tournament 1");
        var tempDir = CreateTempDirectory();

        try
        {
            SummaryExporter.Export([summary], tempDir);

            Assert.True(File.Exists(Path.Combine(tempDir, "summary.xlsx")));
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void Export_WritesHeaderRowWithTournamentNames()
    {
        var summary1 = CreateTestSummary("Tournament 1");
        var summary2 = CreateTestSummary("Tournament 2");
        var tempDir = CreateTempDirectory();

        try
        {
            SummaryExporter.Export([summary1, summary2], tempDir);

            using var workbook = new XLWorkbook(Path.Combine(tempDir, "summary.xlsx"));
            var worksheet = workbook.Worksheet("Summary");

            Assert.Equal("ID", worksheet.Cell(1, 1).GetString());
            Assert.Equal("Name", worksheet.Cell(1, 2).GetString());
            Assert.Equal("Church", worksheet.Cell(1, 3).GetString());
            Assert.Equal("Tournament 1", worksheet.Cell(1, 4).GetString());
            Assert.Equal("Tournament 2", worksheet.Cell(1, 5).GetString());
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void Export_MergesQuizzerAveragesAcrossMultipleTournaments()
    {
        var summary1 = CreateTestSummary("Tournament 1");
        var summary2 = CreateTestSummary("Tournament 2");
        var tempDir = CreateTempDirectory();

        try
        {
            SummaryExporter.Export([summary1, summary2], tempDir);

            using var workbook = new XLWorkbook(Path.Combine(tempDir, "summary.xlsx"));
            var worksheet = workbook.Worksheet("Summary");

            // Two quizzers total, one row each, populated for both tournament columns.
            Assert.Equal("Adams, Alice", worksheet.Cell(2, 2).GetString());
            Assert.False(worksheet.Cell(2, 4).IsEmpty());
            Assert.False(worksheet.Cell(2, 5).IsEmpty());
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void Export_WhenFileAlreadyExists_OverwritesFile()
    {
        var summary = CreateTestSummary("Tournament 1");
        var tempDir = CreateTempDirectory();

        try
        {
            var path = Path.Combine(tempDir, "summary.xlsx");
            File.WriteAllText(path, "placeholder");

            SummaryExporter.Export([summary], tempDir);

            using var workbook = new XLWorkbook(path);
            Assert.NotNull(workbook.Worksheet("Summary"));
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
