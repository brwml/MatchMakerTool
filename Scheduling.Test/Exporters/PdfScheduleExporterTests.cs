namespace Scheduling.Test.Exporters;

using System;
using System.IO;

using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

using MatchMaker.Scheduling.Exporters;

using Xunit;

public class PdfScheduleExporterTests
{
    [Fact]
    public void Export_CreatesPdfFile()
    {
        var exporter = new PdfScheduleExporter();
        var schedule = ScheduleExporterTestFixtures.CreateTestSchedule("Test Tournament");
        var tempDir = ScheduleExporterTestFixtures.CreateTempDirectory();

        try
        {
            exporter.Export(schedule, tempDir);

            var path = Path.Combine(tempDir, $"{schedule.Name}.pdf");
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
        var exporter = new PdfScheduleExporter();
        var schedule = ScheduleExporterTestFixtures.CreateTestSchedule("Test Tournament");
        var tempDir = ScheduleExporterTestFixtures.CreateTempDirectory();

        try
        {
            exporter.Export(schedule, tempDir);

            var text = ExtractText(Path.Combine(tempDir, $"{schedule.Name}.pdf"));

            Assert.Contains(schedule.Name, text, StringComparison.Ordinal);
            Assert.Contains("Room 1", text, StringComparison.Ordinal);
            Assert.Contains("T1 - Team 1", text, StringComparison.Ordinal);
        }
        finally
        {
            ScheduleExporterTestFixtures.CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void Export_WithNoRounds_OmitsDate()
    {
        var exporter = new PdfScheduleExporter();
        var schedule = ScheduleExporterTestFixtures.CreateEmptyTestSchedule("Empty Tournament");
        var tempDir = ScheduleExporterTestFixtures.CreateTempDirectory();

        try
        {
            exporter.Export(schedule, tempDir);

            var path = Path.Combine(tempDir, $"{schedule.Name}.pdf");
            Assert.True(File.Exists(path));

            var text = ExtractText(path);
            Assert.Contains(schedule.Name, text, StringComparison.Ordinal);
            Assert.DoesNotMatch(@"\d{4}", text);
        }
        finally
        {
            ScheduleExporterTestFixtures.CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void Export_WithRounds_IncludesDate()
    {
        var exporter = new PdfScheduleExporter();
        var schedule = ScheduleExporterTestFixtures.CreateTestSchedule("Test Tournament", startDate: new DateOnly(2026, 1, 15));
        var tempDir = ScheduleExporterTestFixtures.CreateTempDirectory();

        try
        {
            exporter.Export(schedule, tempDir);

            var text = ExtractText(Path.Combine(tempDir, $"{schedule.Name}.pdf"));

            Assert.Contains("January 15, 2026", text, StringComparison.Ordinal);
        }
        finally
        {
            ScheduleExporterTestFixtures.CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void Export_WithOddNumberOfTeams_HandlesUnpairedTeam()
    {
        var exporter = new PdfScheduleExporter();
        var schedule = ScheduleExporterTestFixtures.CreateTestSchedule("Odd Tournament", numberOfTeams: 3);
        var tempDir = ScheduleExporterTestFixtures.CreateTempDirectory();

        try
        {
            exporter.Export(schedule, tempDir);

            var text = ExtractText(Path.Combine(tempDir, $"{schedule.Name}.pdf"));

            Assert.Contains("T1 - Team 1", text, StringComparison.Ordinal);
            Assert.Contains("T2 - Team 2", text, StringComparison.Ordinal);
            Assert.Contains("T3 - Team 3", text, StringComparison.Ordinal);
        }
        finally
        {
            ScheduleExporterTestFixtures.CleanupDirectory(tempDir);
        }
    }

    private static string ExtractText(string path)
    {
        using var reader = new PdfReader(path);
        using var document = new PdfDocument(reader);

        var text = string.Empty;

        for (var i = 1; i <= document.GetNumberOfPages(); i++)
        {
            text += PdfTextExtractor.GetTextFromPage(document.GetPage(i));
        }

        return text;
    }
}
