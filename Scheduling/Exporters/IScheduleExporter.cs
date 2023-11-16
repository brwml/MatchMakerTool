namespace MatchMaker.Scheduling.Exporters;

using MatchMaker.Models;

/// <summary>
/// Defines the <see cref="IScheduleExporter"/> interface.
/// </summary>
public interface IScheduleExporter
{
    /// <summary>
    /// Exports the specified schedule.
    /// </summary>
    /// <param name="schedule">The schedule to export.</param>
    /// <param name="folder">The target folder.</param>
    void Export(Schedule schedule, string folder);
}
