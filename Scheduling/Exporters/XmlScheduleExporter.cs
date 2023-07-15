namespace MatchMaker.Scheduling.Exporters;

using MatchMaker.Models;

public class XmlScheduleExporter : IScheduleExporter
{
    public void Export(Schedule schedule, string folder)
    {
        var fileName = Path.Combine(folder, $"{schedule.Name}.xml");
        schedule.ToXml().Save(fileName);
    }
}
