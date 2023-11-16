namespace MatchMaker.Scheduling.Exporters;

using System;

using MatchMaker.Models;

/// <summary>
/// Exports the schedule in Rich Text Format (RTF).
/// </summary>
/// <seealso cref="IScheduleExporter" />
public class RtfScheduleExporter : IScheduleExporter
{
    /// <summary>
    /// Exports the specified schedule in Rich Text Format (RTF).
    /// </summary>
    /// <param name="schedule">The schedule to export.</param>
    /// <param name="folder">The target folder.</param>
    public void Export(Schedule schedule, string folder)
    {
        throw new NotImplementedException();
    }
}
