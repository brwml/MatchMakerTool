namespace MatchMaker.Scheduling;

using System;
using System.Collections.Generic;
using System.Linq;

using MatchMaker.Reporting.Models;

/// <summary>
/// Create round-robin tournament schedules.
/// </summary>
public class RoundRobinTournament
{
    /// <summary>
    /// Create a round-robin tournament schedule.
    /// </summary>
    /// <param name="schedule">The schedule containing the list of teams.</param>
    /// <param name="availableRooms">The number of available rooms.</param>
    /// <returns>The <see cref="Schedule"/> instance with the rounds created.</returns>
    public static Schedule Create(Schedule schedule, int availableRooms)
    {
        schedule.Rounds = CreateRounds(schedule.Teams.Select(x => x.Value), availableRooms).ToDictionary(x => x.Id, x => x);
        return schedule;
    }

    /// <summary>
    /// Creates the rounds from the given teams and available rooms.
    /// </summary>
    /// <param name="teams">The list of teams in the tournament.</param>
    /// <param name="availableRooms">The number of available rooms.</param>
    /// <returns>The <see cref="IEnumerable{Round}"/> instance</returns>
    private static IEnumerable<Round> CreateRounds(IEnumerable<Team> teams, int availableRooms)
    {
        var matches = CreateMatchSchedules(teams);
        var currentRoom = 0;
        var currentRound = CreateRound();

        availableRooms = Math.Min(teams.Count() / 2, availableRooms);

        foreach (var match in matches)
        { 
            match.Room = (currentRoom++ % availableRooms) + 1;

            currentRound.Matches.Add(match.Id, match);

            if (currentRound.Matches.Count == availableRooms)
            {
                yield return currentRound;

                currentRound = CreateRound(currentRound.Id);
            }
        }

        if (currentRound.Matches.Any())
        {
            yield return currentRound;
        }
    }

    /// <summary>
    /// Create a new round with the next round number.
    /// </summary>
    /// <param name="currentRound">The current round number</param>
    /// <returns>The <see cref="Round"/> instance</returns>
    private static Round CreateRound(int currentRound = 0)
    {
        return new Round
        {
            Id = currentRound + 1,
            Matches = new Dictionary<int, MatchSchedule>()
        };
    }

    /// <summary>
    /// Creates the match schedules.
    /// </summary>
    /// <param name="teams">The list of teams</param>
    /// <returns>The <see cref="IEnumerable{MatchSchedule}"/> instance in ideal order</returns>
    /// <remarks>
    /// The algorithm places all the teams an queue as a structure to rotate the teams in a circle. The rotations occur
    /// until each team has occupied each place in the queue. For each iteration of the queue, the queue is foldered to
    /// create the matches.
    /// </remarks>
    private static IEnumerable<MatchSchedule> CreateMatchSchedules(IEnumerable<Team> teams)
    {
        var teamList = teams.ToList();
        var teamsCount = teamList.Count;
        var matchCount = teamsCount / 2;
        var rotationIndex = (teamsCount + 1) % 2;
        var iterations = teamsCount - rotationIndex;

        for (var i = 0; i < iterations; i++)
        {
            for (var j = 0; j < matchCount; j++)
            {
                var team1 = teamList[j].Id;
                var team2 = teamList[teamsCount - j - 1].Id;

                yield return new MatchSchedule
                {
                    Id = CreateMatchId(team1, team2),
                    Teams = new List<int> { team1, team2 }
                };
            }

            var movingTeam = teamList[rotationIndex];
            teamList.RemoveAt(rotationIndex);
            teamList.Add(movingTeam);
        }
    }

    /// <summary>
    /// Create the match identifier given the identifier of the two teams.
    /// </summary>
    /// <param name="team1">The first team</param>
    /// <param name="team2">The second team</param>
    /// <returns>The match identifier</returns>
    private static int CreateMatchId(int team1, int team2)
    {
        const int BitsPerByte = 8;
        const int ShiftSize = sizeof(int) * BitsPerByte / 2;
        return (Math.Min(team1, team2) << ShiftSize) + Math.Max(team1, team2);
    }
}
