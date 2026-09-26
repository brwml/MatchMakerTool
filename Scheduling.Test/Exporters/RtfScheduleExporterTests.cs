namespace Scheduling.Test.Exporters;

using System;
using System.IO;

using MatchMaker.Scheduling.Exporters;

using Xunit;

public class RtfScheduleExporterTests
{
    [Fact]
    public void Export_CreatesRtfFile()
    {
        var exporter = new RtfScheduleExporter();
        var schedule = ScheduleExporterTestFixtures.CreateTestSchedule("Test Tournament");
        var tempDir = ScheduleExporterTestFixtures.CreateTempDirectory();

        try
        {
            exporter.Export(schedule, tempDir);

            var path = Path.Combine(tempDir, $"{schedule.Name}.rtf");
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
        var exporter = new RtfScheduleExporter();
        var schedule = ScheduleExporterTestFixtures.CreateTestSchedule("Test Tournament");
        var tempDir = ScheduleExporterTestFixtures.CreateTempDirectory();

        try
        {
            exporter.Export(schedule, tempDir);

            var content = File.ReadAllText(Path.Combine(tempDir, $"{schedule.Name}.rtf"));

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
        var exporter = new RtfScheduleExporter();
        var schedule = ScheduleExporterTestFixtures.CreateEmptyTestSchedule("Empty Tournament");
        var tempDir = ScheduleExporterTestFixtures.CreateTempDirectory();

        try
        {
            exporter.Export(schedule, tempDir);

            var content = File.ReadAllText(Path.Combine(tempDir, $"{schedule.Name}.rtf"));

            Assert.DoesNotContain(@"\cf1", content, StringComparison.Ordinal);
        }
        finally
        {
            ScheduleExporterTestFixtures.CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void Export_WithRounds_IncludesDate()
    {
        var exporter = new RtfScheduleExporter();
        var schedule = ScheduleExporterTestFixtures.CreateTestSchedule("Test Tournament", startDate: new DateOnly(2026, 1, 15));
        var tempDir = ScheduleExporterTestFixtures.CreateTempDirectory();

        try
        {
            exporter.Export(schedule, tempDir);

            var content = File.ReadAllText(Path.Combine(tempDir, $"{schedule.Name}.rtf"));

            Assert.Contains(@"\cf1", content, StringComparison.Ordinal);
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
        var exporter = new RtfScheduleExporter();
        var schedule = ScheduleExporterTestFixtures.CreateTestSchedule("Odd Tournament", numberOfTeams: 3);
        var tempDir = ScheduleExporterTestFixtures.CreateTempDirectory();

        try
        {
            exporter.Export(schedule, tempDir);

            var content = File.ReadAllText(Path.Combine(tempDir, $"{schedule.Name}.rtf"));

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
