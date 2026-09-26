namespace Reporting.Test.Exporters;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Bogus;

using MatchMaker.Models;
using MatchMaker.Reporting.Exporters;
using MatchMaker.Reporting.Models;
using MatchMaker.Reporting.Policies;

using Xunit;

public class MarkdownExporterTests
{
    [Fact]
    public void MarkdownExporter_Export_CreatesMarkdownFolder()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament");
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var markdownFolder = Path.Combine(tempDir, "Markdown", summary.Name);
            Assert.True(Directory.Exists(markdownFolder), $"Markdown folder not created at {markdownFolder}");
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_CreatesIndexFile()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament");
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var indexPath = Path.Combine(tempDir, "Markdown", summary.Name, "index.md");
            Assert.True(File.Exists(indexPath), "Index file not created");
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_IndexFileContainsTournamentName()
    {
        var exporter = new MarkdownExporter();
        var tournamentName = "Test Tournament";
        var summary = CreateTestSummary(tournamentName);
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var indexPath = Path.Combine(tempDir, "Markdown", summary.Name, "index.md");
            var content = File.ReadAllText(indexPath);

            Assert.Contains(tournamentName, content);
            Assert.Contains("# " + tournamentName, content);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_IndexFileContainsSummaryLinks()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament");
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var indexPath = Path.Combine(tempDir, "Markdown", summary.Name, "index.md");
            var content = File.ReadAllText(indexPath);

            Assert.Contains("teams.md", content);
            Assert.Contains("quizzers.md", content);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_CreatesTeamSummaryFile()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament");
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var teamSummaryPath = Path.Combine(tempDir, "Markdown", summary.Name, "teams.md");
            Assert.True(File.Exists(teamSummaryPath), "Team summary file not created");
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_TeamSummaryFileContainsTableHeaders()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament");
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var teamSummaryPath = Path.Combine(tempDir, "Markdown", summary.Name, "teams.md");
            var content = File.ReadAllText(teamSummaryPath);

            Assert.Contains("Place", content);
            Assert.Contains("Team", content);
            Assert.Contains("Wins", content);
            Assert.Contains("Losses", content);
            Assert.Contains("Win %", content);
            Assert.Contains("Avg Score", content);
            Assert.Contains("Avg Errors", content);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_TeamSummaryFileContainsMarkdownTable()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament");
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var teamSummaryPath = Path.Combine(tempDir, "Markdown", summary.Name, "teams.md");
            var content = File.ReadAllText(teamSummaryPath);

            Assert.Contains("|", content);
            Assert.Contains("---", content);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_CreatesTeamDetailFolder()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament");
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var teamFolder = Path.Combine(tempDir, "Markdown", summary.Name, "teams");
            Assert.True(Directory.Exists(teamFolder), "Team details folder not created");
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_CreatesTeamDetailFiles()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament");
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var teamFolder = Path.Combine(tempDir, "Markdown", summary.Name, "teams");
            var teamFiles = Directory.GetFiles(teamFolder, "*.md");

            Assert.NotEmpty(teamFiles);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_TeamDetailFileContainsTeamStatistics()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament");
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var teamFolder = Path.Combine(tempDir, "Markdown", summary.Name, "teams");
            var teamFiles = Directory.GetFiles(teamFolder, "*.md");
            var content = File.ReadAllText(teamFiles[0]);

            Assert.Contains("## Team Statistics", content);
            Assert.Contains("Place:", content);
            Assert.Contains("Wins:", content);
            Assert.Contains("Losses:", content);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_CreatesQuizzerSummaryFile()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament");
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var quizzerSummaryPath = Path.Combine(tempDir, "Markdown", summary.Name, "quizzers.md");
            Assert.True(File.Exists(quizzerSummaryPath), "Quizzer summary file not created");
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_QuizzerSummaryFileContainsTableHeaders()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament");
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var quizzerSummaryPath = Path.Combine(tempDir, "Markdown", summary.Name, "quizzers.md");
            var content = File.ReadAllText(quizzerSummaryPath);

            Assert.Contains("Place", content);
            Assert.Contains("Name", content);
            Assert.Contains("Team", content);
            Assert.Contains("Church", content);
            Assert.Contains("Average Score", content);
            Assert.Contains("Average Errors", content);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_QuizzerSummaryFileContainsMarkdownTable()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament");
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var quizzerSummaryPath = Path.Combine(tempDir, "Markdown", summary.Name, "quizzers.md");
            var content = File.ReadAllText(quizzerSummaryPath);

            Assert.Contains("|", content);
            Assert.Contains("---", content);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_CreatesQuizzerDetailFolder()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament");
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var quizzerFolder = Path.Combine(tempDir, "Markdown", summary.Name, "quizzers");
            Assert.True(Directory.Exists(quizzerFolder), "Quizzer details folder not created");
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_CreatesQuizzerDetailFiles()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament");
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var quizzerFolder = Path.Combine(tempDir, "Markdown", summary.Name, "quizzers");
            var quizzerFiles = Directory.GetFiles(quizzerFolder, "*.md");

            Assert.NotEmpty(quizzerFiles);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_QuizzerDetailFileContainsQuizzerStatistics()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament");
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var quizzerFolder = Path.Combine(tempDir, "Markdown", summary.Name, "quizzers");
            var quizzerFiles = Directory.GetFiles(quizzerFolder, "*.md");
            var content = File.ReadAllText(quizzerFiles[0]);

            Assert.Contains("## Quizzer Statistics", content);
            Assert.Contains("Place:", content);
            Assert.Contains("Team:", content);
            Assert.Contains("Church:", content);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_DecimalFormattingIsCorrect()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament");
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var teamSummaryPath = Path.Combine(tempDir, "Markdown", summary.Name, "teams.md");
            var content = File.ReadAllText(teamSummaryPath);

            var lines = content.Split([Environment.NewLine], StringSplitOptions.None);
            var dataRow = System.Linq.Enumerable.FirstOrDefault(lines, l => l.StartsWith('|') && !l.Contains("Place") && !l.Contains("---"));

            Assert.NotNull(dataRow);
            var parts = dataRow.Split('|');
            Assert.True(parts.Length > 5, "Row should have multiple columns");
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_FilesUseMarkdownExtension()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament");
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var markdownFolder = Path.Combine(tempDir, "Markdown", summary.Name);
            var allFiles = Directory.GetFiles(markdownFolder, "*.*", SearchOption.AllDirectories);

            foreach (var file in allFiles)
            {
                Assert.EndsWith(".md", file, StringComparison.OrdinalIgnoreCase);
            }
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_ReplacesExistingMarkdownFolder()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament");
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var markdownFolder = Path.Combine(tempDir, "Markdown", summary.Name);
            var fileCountBefore = Directory.GetFiles(markdownFolder, "*.*", SearchOption.AllDirectories).Length;

            exporter.Export(summary, tempDir);

            var fileCountAfter = Directory.GetFiles(markdownFolder, "*.*", SearchOption.AllDirectories).Length;

            Assert.Equal(fileCountBefore, fileCountAfter);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_GeneratesValidMarkdownSyntax()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament");
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var indexPath = Path.Combine(tempDir, "Markdown", summary.Name, "index.md");
            var content = File.ReadAllText(indexPath);

            Assert.Contains("#", content);
            Assert.Contains("-", content);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    private static Summary CreateTestSummary(string name)
    {
        var faker = new Faker();

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
            { 1, new Quizzer(1, faker.Name.FirstName(Bogus.DataSets.Name.Gender.Male), faker.Name.LastName(), Gender.Male, DateTime.Now.Year, 1, 1) },
            { 2, new Quizzer(2, faker.Name.FirstName(Bogus.DataSets.Name.Gender.Female), faker.Name.LastName(), Gender.Female, DateTime.Now.Year, 2, 2) }
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
        var policies = new TeamRankingPolicy[] { new NullTeamRankingPolicy() };
        var summary = Summary.FromResult(result, policies);

        return summary;
    }

    /// <summary>
    /// Builds a summary with the given number of teams/quizzers and the given number of rounds,
    /// so that markdown tables and match-history sections produce multiple rows.
    /// </summary>
    private static Summary CreateTestSummary(string name, int numberOfTeams, int numberOfRounds, int numberOfEliminationTeams = 0)
    {
        var faker = new Faker();

        var churches = Enumerable.Range(1, numberOfTeams).ToDictionary(x => x, x => new Church(x, $"Church {x}"));
        var teams = Enumerable.Range(1, numberOfTeams).ToDictionary(x => x, x => new Team(x, $"Team {x}", $"T{x}", 0));
        var quizzers = Enumerable.Range(1, numberOfTeams).ToDictionary(
            x => x,
            x => new Quizzer(x, faker.Name.FirstName(), faker.Name.LastName(), Gender.Male, DateTime.Now.Year, x, x));

        var rounds = Enumerable.Range(1, numberOfRounds).ToDictionary(
            x => x,
            x => new Round(x, new Dictionary<int, MatchSchedule>(), DateOnly.FromDateTime(DateTime.Now), TimeOnly.FromDateTime(DateTime.Now)));

        var schedule = new Schedule(name, churches, quizzers, teams, rounds);

        var teamResults = Enumerable.Range(1, numberOfTeams).Select(x => new TeamResult(x, 100 - x, x - 1, 1)).ToList();
        var quizzerResults = Enumerable.Range(1, numberOfTeams).Select(x => new QuizzerResult(x, 100 - x, x - 1)).ToList();
        var matches = Enumerable.Range(1, numberOfRounds).ToDictionary(
            x => x,
            x => new MatchResult(x, x, x, teamResults, quizzerResults));

        var result = new Result(schedule, matches);
        var policies = new TeamRankingPolicy[] { new NullTeamRankingPolicy() };
        var summary = Summary.FromResult(result, policies);
        summary.NumberOfEliminationTeams = numberOfEliminationTeams;

        return summary;
    }

    [Fact]
    public void MarkdownExporter_Export_TeamSummaryFile_WritesEachTeamOnItsOwnLine()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament", numberOfTeams: 4, numberOfRounds: 1);
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var teamSummaryPath = Path.Combine(tempDir, "Markdown", summary.Name, "teams.md");
            var content = File.ReadAllText(teamSummaryPath);
            var lines = content.Split([Environment.NewLine], StringSplitOptions.None);
            var dataRows = lines.Where(l => l.StartsWith('|') && !l.Contains("Place") && !l.Contains("---")).ToArray();

            Assert.Equal(4, dataRows.Length);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_QuizzerSummaryFile_WritesEachQuizzerOnItsOwnLine()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament", numberOfTeams: 4, numberOfRounds: 1);
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var quizzerSummaryPath = Path.Combine(tempDir, "Markdown", summary.Name, "quizzers.md");
            var content = File.ReadAllText(quizzerSummaryPath);
            var lines = content.Split([Environment.NewLine], StringSplitOptions.None);
            var dataRows = lines.Where(l => l.StartsWith('|') && !l.Contains("Place") && !l.Contains("---")).ToArray();

            Assert.Equal(4, dataRows.Length);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_TeamDetailFile_WritesEachMatchOnItsOwnLine()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament", numberOfTeams: 2, numberOfRounds: 3);
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var teamDetailPath = Path.Combine(tempDir, "Markdown", summary.Name, "teams", "1.md");
            var content = File.ReadAllText(teamDetailPath);
            var matchHistorySection = content.Split("## Team Quizzers")[0];
            var lines = matchHistorySection.Split([Environment.NewLine], StringSplitOptions.None);
            var dataRows = lines.Where(l => l.StartsWith('|') && !l.Contains("Round") && !l.Contains("---")).ToArray();

            Assert.Equal(3, dataRows.Length);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_QuizzerDetailFile_WritesEachMatchOnItsOwnLine()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament", numberOfTeams: 2, numberOfRounds: 3);
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var quizzerDetailPath = Path.Combine(tempDir, "Markdown", summary.Name, "quizzers", "1.md");
            var content = File.ReadAllText(quizzerDetailPath);
            var lines = content.Split([Environment.NewLine], StringSplitOptions.None);
            var dataRows = lines.Where(l => l.StartsWith('|') && !l.Contains("Round") && !l.Contains("---")).ToArray();

            Assert.Equal(3, dataRows.Length);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_TeamSummaryFile_MarksEliminationTeamsWithAsterisk()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament", numberOfTeams: 4, numberOfRounds: 1, numberOfEliminationTeams: 2);
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var teamSummaryPath = Path.Combine(tempDir, "Markdown", summary.Name, "teams.md");
            var content = File.ReadAllText(teamSummaryPath);
            var eliminationTeamIds = summary.EliminationTeamIds;

            foreach (var teamId in Enumerable.Range(1, 4))
            {
                var expectedName = eliminationTeamIds.Contains(teamId) ? $"[*Team {teamId}]" : $"[Team {teamId}]";
                Assert.Contains(expectedName, content);
            }
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_TeamSummaryFile_IncludesLegendWhenEliminationTeamsPresent()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament", numberOfTeams: 4, numberOfRounds: 1, numberOfEliminationTeams: 2);
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var teamSummaryPath = Path.Combine(tempDir, "Markdown", summary.Name, "teams.md");
            var content = File.ReadAllText(teamSummaryPath);

            Assert.Contains("Indicates a team that will participate in the elimination tournament.", content);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void MarkdownExporter_Export_TeamSummaryFile_OmitsLegendWhenNoEliminationTeams()
    {
        var exporter = new MarkdownExporter();
        var summary = CreateTestSummary("Test Tournament", numberOfTeams: 4, numberOfRounds: 1, numberOfEliminationTeams: 0);
        var tempDir = CreateTempDirectory();

        try
        {
            exporter.Export(summary, tempDir);

            var teamSummaryPath = Path.Combine(tempDir, "Markdown", summary.Name, "teams.md");
            var content = File.ReadAllText(teamSummaryPath);

            Assert.DoesNotContain("Indicates a team that will participate in the elimination tournament.", content);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
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
