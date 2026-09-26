namespace Reporting.Test.Exporters;

using System;
using System.Collections.Generic;
using System.IO;

using MatchMaker.Models;
using MatchMaker.Reporting.Exporters;
using MatchMaker.Reporting.Models;
using MatchMaker.Reporting.Policies;

using Xunit;

public class HtmlSummaryExporterTests
{
    [Fact]
    public void Export_CreatesTeamsHtmlFile()
    {
        var exporter = new HtmlSummaryExporter();
        var summary = CreateTestSummary("Test Tournament", numberOfEliminationTeams: 0);
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var path = Path.Combine(tempDir, "Results", "teams.html");
            Assert.True(File.Exists(path));
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void Export_WithoutEliminationTeams_DoesNotIncludeLegendOrAsterisk()
    {
        var exporter = new HtmlSummaryExporter();
        var summary = CreateTestSummary("Test Tournament", numberOfEliminationTeams: 0);
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var content = File.ReadAllText(Path.Combine(tempDir, "Results", "teams.html"));

            Assert.DoesNotContain("elimination tournament", content, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(">*Team 1<", content, StringComparison.Ordinal);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void Export_WithEliminationTeams_MarksQualifyingTeamAndIncludesLegend()
    {
        var exporter = new HtmlSummaryExporter();
        var summary = CreateTestSummary("Test Tournament", numberOfEliminationTeams: 1);
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var content = File.ReadAllText(Path.Combine(tempDir, "Results", "teams.html"));

            Assert.Contains("elimination tournament", content, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(">*Team 1<", content, StringComparison.Ordinal);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    private static Summary CreateTestSummary(string name, int numberOfEliminationTeams)
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
        var summary = Summary.FromResult(result, policies);
        summary.NumberOfEliminationTeams = numberOfEliminationTeams;

        return summary;
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
