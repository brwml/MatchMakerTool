namespace MatchMaker.Scheduling.Exporters;

using MatchMaker.Models;

public interface IScheduleExporter
{
    void Export(Schedule schedule, string folder);
}
