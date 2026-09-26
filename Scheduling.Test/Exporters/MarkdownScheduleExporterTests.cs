namespace Scheduling.Test.Exporters;

using System;
using System.IO;

using MatchMaker.Scheduling.Exporters;

using Xunit;

public class MarkdownScheduleExporterTests
{
    [Fact]
    public void Export_CreatesMarkdownFile()
    {
        var exporter = new MarkdownScheduleExporter();
        var schedule = ScheduleExporterTestFixtures.CreateTestSchedule("Test Tournament");
        var tempDir = ScheduleExporterTestFixtures.CreateTempDirectory();

        try
        {
            exporter.Export(schedule, tempDir);

            var path = Path.Combine(tempDir, $"{schedule.Name}.md");
            Assert.True(File.Exists(path));
        }
        finally
        {
            ScheduleExporterTestFixtures.CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void Export_IncludesTitleRoomsAndTeams()
    {
        var exporter = new MarkdownScheduleExporter();
        var schedule = ScheduleExporterTestFixtures.CreateTestSchedule("Test Tournament");
        var tempDir = ScheduleExporterTestFixtures.CreateTempDirectory();

        try
        {
            exporter.Export(schedule, tempDir);

            var content = File.ReadAllText(Path.Combine(tempDir, $"{schedule.Name}.md"));

            Assert.Contains(schedule.Name, content, StringComparison.Ordinal);
            Assert.Contains("Room 1", content, StringComparison.Ordinal);
            Assert.Contains("T1", content, StringComparison.Ordinal);
            Assert.Contains("Team 1", content, StringComparison.Ordinal);
        }
        finally
        {
            ScheduleExporterTestFixtures.CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void Export_WithNoRounds_OmitsDate()
    {
        var exporter = new MarkdownScheduleExporter();
        var schedule = ScheduleExporterTestFixtures.CreateEmptyTestSchedule("Empty Tournament");
        var tempDir = ScheduleExporterTestFixtures.CreateTempDirectory();

        try
        {
            exporter.Export(schedule, tempDir);

            var content = File.ReadAllText(Path.Combine(tempDir, $"{schedule.Name}.md"));

            Assert.DoesNotMatch(@"\d{4}", content);
        }
        finally
        {
            ScheduleExporterTestFixtures.CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void Export_WithRounds_IncludesDate()
    {
        var exporter = new MarkdownScheduleExporter();
        var schedule = ScheduleExporterTestFixtures.CreateTestSchedule("Test Tournament", startDate: new DateOnly(2026, 1, 15));
        var tempDir = ScheduleExporterTestFixtures.CreateTempDirectory();

        try
        {
            exporter.Export(schedule, tempDir);

            var content = File.ReadAllText(Path.Combine(tempDir, $"{schedule.Name}.md"));

            Assert.Contains("January 15, 2026", content, StringComparison.Ordinal);
        }
        finally
        {
            ScheduleExporterTestFixtures.CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void Export_WithOddNumberOfTeams_HandlesUnpairedTeam()
    {
        var exporter = new MarkdownScheduleExporter();
        var schedule = ScheduleExporterTestFixtures.CreateTestSchedule("Odd Tournament", numberOfTeams: 3);
        var tempDir = ScheduleExporterTestFixtures.CreateTempDirectory();

        try
        {
            exporter.Export(schedule, tempDir);

            var content = File.ReadAllText(Path.Combine(tempDir, $"{schedule.Name}.md"));

            Assert.Contains("T1 - Team 1", content, StringComparison.Ordinal);
            Assert.Contains("T2 - Team 2", content, StringComparison.Ordinal);
            Assert.Contains("T3 - Team 3", content, StringComparison.Ordinal);
        }
        finally
        {
            ScheduleExporterTestFixtures.CleanupDirectory(tempDir);
        }
    }
}
