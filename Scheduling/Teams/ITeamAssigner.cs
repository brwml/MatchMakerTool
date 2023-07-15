namespace MatchMaker.Scheduling.Teams;

using MatchMaker.Models;

/// <summary>
/// Defines the team assigner interface.
/// </summary>
public interface ITeamAssigner
{
    /// <summary>
    /// Assigns the teams in the schedule.
    /// </summary>
    /// <param name="schedule">The schedule.</param>
    /// <param name="numberOfRooms">The number of rooms.</param>
    /// <param name="numberOfTeams">The number of teams.</param>
    /// <returns>The <see cref="Schedule"/> instance</returns>
    Schedule Create(Schedule schedule, int numberOfRooms, int numberOfTeams);
}
