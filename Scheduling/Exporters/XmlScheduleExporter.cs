namespace MatchMaker.Scheduling.Exporters;

using MatchMaker.Models;

/// <summary>
/// Exports the schedule in XML format.
/// </summary>
/// <seealso cref="IScheduleExporter" />
public class XmlScheduleExporter : IScheduleExporter
{
    /// <summary>
    /// Exports the specified schedule in XML format.
    /// </summary>
    /// <param name="schedule">The schedule to export.</param>
    /// <param name="folder">The target folder.</param>
    public void Export(Schedule schedule, string folder)
    {
        var fileName = Path.Combine(folder, $"{schedule.Name}.xml");
        schedule.ToXml().Save(fileName);
    }
}
