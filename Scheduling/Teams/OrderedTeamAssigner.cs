namespace MatchMaker.Scheduling.Teams;

using System;
using System.Globalization;

using Humanizer;

using MatchMaker.Models;

/// <summary>
/// Assigns quizzers to teams in the order they appear in the schedule.
/// </summary>
/// <seealso cref="ITeamAssigner" />
public class OrderedTeamAssigner : ITeamAssigner
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
        var quizzers = CreateQuizzers(schedule.Quizzers, teams);

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
    /// <param name="quizzers">The quizzers.</param>
    /// <param name="teams">The teams.</param>
    /// <returns>The quizzer map</returns>
    private static Dictionary<int, Quizzer> CreateQuizzers(IDictionary<int, Quizzer> quizzers, Dictionary<int, Team> teams)
    {
        var result = new Dictionary<int, Quizzer>();

        var quizzerId = 0;

        foreach (var quizzer in quizzers.OrderBy(x => x.Key).Select(x => x.Value))
        {
            quizzerId++;
            var (quotient, remainder) = Math.DivRem(quizzerId - 1, teams.Count);
            var teamId = (quotient % 2 == 0) ? remainder + 1 : teams.Count - remainder;

            result.Add(
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

        return result;
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
        if (numberOfTeams > 0)
        {
            return numberOfTeams;
        }

        return numberOfRooms * 2;
    }
}
