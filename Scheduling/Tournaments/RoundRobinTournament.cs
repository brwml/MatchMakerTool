namespace MatchMaker.Scheduling.Tournaments;

using System;
using System.Collections.Generic;
using System.Linq;

using Ardalis.GuardClauses;

using MatchMaker.Models;

/// <summary>
/// Create round-robin tournament schedules.
/// </summary>
public static class RoundRobinTournament
{
    /// <summary>
    /// Create a round-robin tournament schedule.
    /// </summary>
    /// <param name="schedule">The schedule containing the list of teams.</param>
    /// <param name="availableRooms">The number of available rooms.</param>
    /// <returns>The <see cref="Schedule"/> instance with the rounds created.</returns>
    public static Schedule Create(Schedule schedule, int availableRooms)
    {
        Guard.Against.Null(schedule);

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
            match.Room = currentRoom++ % availableRooms + 1;

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
        return new Round(currentRound + 1);
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
        var multiplier = CreateMatchIdMultiplier(matchCount);

        for (var i = 0; i < iterations; i++)
        {
            for (var j = 0; j < matchCount; j++)
            {
                var team1 = teamList[j].Id;
                var team2 = teamList[teamsCount - j - 1].Id;

                var id = CreateMatchId(i + 1, j + 1, multiplier);
                var match = new List<int> { team1, team2 };

                yield return new MatchSchedule(id, j + 1, match);
            }

            var movingTeam = teamList[rotationIndex];
            teamList.RemoveAt(rotationIndex);
            teamList.Add(movingTeam);
        }
    }

    /// <summary>
    /// Create the match identifier given the identifier of the two teams.
    /// </summary>
    /// <param name="groupId">The group identifier</param>
    /// <param name="instanceId">The instance identifier</param>
    /// <param name="multiplier">The group multiplier</param>
    /// <returns>The match identifier</returns>
    private static int CreateMatchId(int groupId, int instanceId, int multiplier)
    {
        return groupId * multiplier + instanceId;
    }

    /// <summary>
    /// Creates the multiplier for the match identifier.
    /// </summary>
    /// <param name="groupSize">The group size</param>
    /// <returns>The multiplier</returns>
    private static int CreateMatchIdMultiplier(int groupSize)
    {
        var multiplier = 1;

        while (groupSize > 0)
        {
            multiplier *= 10;
            groupSize /= 10;
        }

        return multiplier;
    }
}
