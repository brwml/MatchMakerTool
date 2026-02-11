namespace MatchMaker.Scheduling.Teams;

using System.Globalization;

using Humanizer;

using MatchMaker.Models;

/// <summary>
/// Assigns quizzers to teams randomly.
/// </summary>
/// <seealso cref="ITeamAssigner" />
public class RandomTeamAssigner : ITeamAssigner
{
    /// <summary>
    /// Assigns the teams in the schedule.
    /// </summary>
    /// <param name="schedule">The schedule.</param>
    /// <param name="numberOfRooms">The number of rooms.</param>
    /// <param name="numberOfTeams">The number of teams.</param>
    /// <returns>
    /// The <see cref="Schedule" /> instance
    /// </returns>
    public Schedule Create(Schedule schedule, int numberOfRooms, int numberOfTeams)
    {
        var teams = CreateTeams(GetNumberOfTeams(numberOfRooms, numberOfTeams));
        var quizzers = CreateQuizzers(schedule, teams);

        return new Schedule(
            schedule.Name,
            schedule.Churches,
            quizzers,
            teams,
            new Dictionary<int, Round>());
    }

    /// <summary>
    /// Creates the quizzers map.
    /// </summary>
    /// <param name="schedule">The schedule.</param>
    /// <param name="teams">The teams.</param>
    /// <returns>The quizzers map</returns>
    private static Dictionary<int, Quizzer> CreateQuizzers(Schedule schedule, Dictionary<int, Team> teams)
    {
        var teamId = 0;
        var quizzerId = 0;

        var quizzers = new Dictionary<int, Quizzer>();

        foreach (var quizzer in schedule.Quizzers.OrderBy(_ => Random.Shared.Next()).Select(x => x.Value))
        {
            teamId = ((teamId + 1) % teams.Count) + 1;
            quizzerId++;

            quizzers.Add(
                quizzerId,
                new Quizzer(
                    quizzerId,
                    quizzer.FirstName,
                    quizzer.LastName,
                    quizzer.Gender,
                    quizzer.RookieYear,
                    teamId,
                    quizzer.ChurchId));
        }

        return quizzers;
    }

    /// <summary>
    /// Creates the teams map.
    /// </summary>
    /// <param name="numberOfTeams">The number of teams.</param>
    /// <returns>The teams map</returns>
    private static Dictionary<int, Team> CreateTeams(int numberOfTeams)
    {
        var teamMap = new Dictionary<int, Team>();

        for (var i = 1; i <= numberOfTeams; i++)
        {
            teamMap.Add(i, new Team(i, $"Team {i.ToWords().Titleize()}", i.ToString(CultureInfo.InvariantCulture), 0));
        }

        return teamMap;
    }

    /// <summary>
    /// Gets the number of teams.
    /// </summary>
    /// <param name="numberOfRooms">The number of rooms.</param>
    /// <param name="numberOfTeams">The number of teams.</param>
    /// <returns>The number of teams</returns>
    private static int GetNumberOfTeams(int numberOfRooms, int numberOfTeams)
    {
        return Math.Max(numberOfTeams, numberOfRooms * 2);
    }
}
