namespace MatchMaker.Tool;

using Ardalis.GuardClauses;

/// <summary>
/// Defines the <see cref="Scheduling" />
/// </summary>
internal static class Scheduling
{
    /// <summary>
    /// Processes the scheduling options
    /// </summary>
    /// <param name="options">The <see cref="ScheduleOptions"/></param>
    /// <returns>The <see cref="bool"/></returns>
    public static bool Process(ScheduleOptions options)
    {
        Guard.Against.Null(options);

        return true;
    }
}
