namespace Scheduling.Test.Exporters;

using System.IO;
using System.Xml.Linq;

using MatchMaker.Models;
using MatchMaker.Scheduling.Exporters;

using Xunit;

public class XmlScheduleExporterTests
{
    [Fact]
    public void Export_CreatesXmlFile()
    {
        var exporter = new XmlScheduleExporter();
        var schedule = ScheduleExporterTestFixtures.CreateTestSchedule("Test Tournament");
        var tempDir = ScheduleExporterTestFixtures.CreateTempDirectory();

        try
        {
            exporter.Export(schedule, tempDir);

            var path = Path.Combine(tempDir, $"{schedule.Name}.xml");
            Assert.True(File.Exists(path));
        }
        finally
        {
            ScheduleExporterTestFixtures.CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public void Export_WritesRoundTrippableSchedule()
    {
        var exporter = new XmlScheduleExporter();
        var schedule = ScheduleExporterTestFixtures.CreateTestSchedule("Test Tournament");
        var tempDir = ScheduleExporterTestFixtures.CreateTempDirectory();

        try
        {
            exporter.Export(schedule, tempDir);

            var path = Path.Combine(tempDir, $"{schedule.Name}.xml");
            var document = XDocument.Load(path);
            var loaded = Schedule.FromXml(document, schedule.Name);

            Assert.Equal(schedule.Name, loaded.Name);
            Assert.Equal(schedule.Teams.Count, loaded.Teams.Count);
            Assert.Equal(schedule.Quizzers.Count, loaded.Quizzers.Count);
            Assert.Equal(schedule.Rounds.Count, loaded.Rounds.Count);
        }
        finally
        {
            ScheduleExporterTestFixtures.CleanupDirectory(tempDir);
        }
    }
}
