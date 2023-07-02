namespace MatchMaker.Tool.Controllers;

using Ardalis.GuardClauses;

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

        return true;
    }
}
