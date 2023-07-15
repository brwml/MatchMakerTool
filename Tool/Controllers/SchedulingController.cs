namespace MatchMaker.Tool.Controllers;

using System.Xml.Linq;

using Ardalis.GuardClauses;

using MatchMaker.Models;
using MatchMaker.Scheduling.Tournaments;

/// <summary>
/// Defines the <see cref="SchedulingController" />
/// </summary>
internal class SchedulingController : IProcessController<ScheduleOptions>
{
    /// <summary>
    /// Processes the scheduling options
    /// </summary>
    /// <param name="options">The <see cref="ScheduleOptions"/></param>
    /// <returns>The <see cref="bool"/></returns>
    public bool Process(ScheduleOptions options)
    {
        Guard.Against.Null(options);

        if (options.ScheduleType == ScheduleType.RoundRobin)
        {
            RoundRobinTournament
                .Create(LoadInputSchedule(options.InputSchedulePath), options.Rooms)
                .WithName(GetScheduleName(options.OutputSchedulePath))
                .ToXml()
                .Save(options.OutputSchedulePath);
        }

        return true;
    }

    /// <summary>
    /// Loads the input schedule.
    /// </summary>
    /// <param name="schedulePath">The schedule path.</param>
    /// <returns>The schedule instance</returns>
    private static Schedule LoadInputSchedule(string schedulePath)
    {
        return Schedule.FromXml(LoadXml(schedulePath), GetScheduleName(schedulePath));
    }

    /// <summary>
    /// Loads the XML document.
    /// </summary>
    /// <param name="schedulePath">The schedule path.</param>
    /// <returns>The XML document</returns>
    private static XDocument LoadXml(string schedulePath)
    {
        return XDocument.Load(schedulePath);
    }

    /// <summary>
    /// Gets the name of the schedule.
    /// </summary>
    /// <param name="schedulePath">The schedule path.</param>
    /// <returns></returns>
    private static string GetScheduleName(string schedulePath)
    {
        return Path.GetFileNameWithoutExtension(schedulePath);
    }
}
