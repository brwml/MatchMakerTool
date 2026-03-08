namespace MatchMaker.Scheduling.Tournaments;

using System;
using System.Collections.Generic;
using System.Linq;

using MatchMaker.Models;

/// <summary>
/// Creates round-robin tournament schedules.
/// </summary>
public static class RoundRobinTournament
{
    /// <summary>
    /// Creates a round-robin tournament schedule.
    /// </summary>
    /// <param name="schedule">The schedule containing the list of teams.</param>
    /// <param name="availableRooms">The number of available rooms. Must be at least 1.</param>
    /// <param name="startDate">
    /// The optional date assigned to every round. When <see langword="null"/>, rounds default to the current date.
    /// </param>
    /// <param name="startTime">
    /// The optional start time of the first round. When combined with <paramref name="roundDuration"/>, each
    /// subsequent round is assigned a start time offset by one duration increment.
    /// </param>
    /// <param name="roundDuration">
    /// The optional duration of each round, used to compute per-round start times when
    /// <paramref name="startTime"/> is also provided.
    /// </param>
    /// <returns>The <see cref="Schedule"/> instance with the rounds populated.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="schedule"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="availableRooms"/> is less than 1.</exception>
    public static Schedule Create(
        Schedule schedule,
        int availableRooms,
        DateOnly? startDate = null,
        TimeOnly? startTime = null,
        TimeSpan? roundDuration = null)
    {
        ArgumentNullException.ThrowIfNull(schedule);

        if (availableRooms < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(availableRooms), availableRooms, "The number of available rooms must be at least 1.");
        }

        schedule.Rounds = CreateRounds(schedule.Teams.Values, availableRooms, startDate, startTime, roundDuration)
            .ToDictionary(x => x.Id, x => x);

        return schedule;
    }

    /// <summary>
    /// Creates the rounds from the given teams and available rooms.
    /// </summary>
    private static IEnumerable<Round> CreateRounds(
        IEnumerable<Team> teams,
        int availableRooms,
        DateOnly? startDate,
        TimeOnly? startTime,
        TimeSpan? roundDuration)
    {
        var teamList = teams.ToList();
        var effectiveRooms = Math.Min(teamList.Count / 2, availableRooms);

        if (effectiveRooms < 1)
        {
            yield break;
        }

        // Capture DateTime.Now once so all rounds share the same fallback timestamp.
        var now = DateTime.Now;
        var baseDate = startDate ?? DateOnly.FromDateTime(now);
        var baseTime = startTime ?? TimeOnly.FromDateTime(now);

        var currentRoom = 0;
        var roundIndex = 0;
        var currentRound = CreateRound(roundIndex, baseDate, baseTime, roundDuration);

        foreach (var match in CreateMatchSchedules(teamList))
        {
            match.Room = (currentRoom++ % effectiveRooms) + 1;
            currentRound.Matches.Add(match.Id, match);

            if (currentRound.Matches.Count == effectiveRooms)
            {
                yield return currentRound;
                currentRound = CreateRound(++roundIndex, baseDate, baseTime, roundDuration);
            }
        }

        if (currentRound.Matches.Count > 0)
        {
            yield return currentRound;
        }
    }

    /// <summary>
    /// Creates a new round with the given 0-based index, computing its start time from the base
    /// time plus one duration increment per round. When the computed time crosses midnight the
    /// date is advanced by the appropriate number of days.
    /// </summary>
    /// <param name="roundIndex">The 0-based round index; the round <see cref="Round.Id"/> will be <c>roundIndex + 1</c>.</param>
    /// <param name="date">The base date for the round.</param>
    /// <param name="baseTime">The start time of the first round.</param>
    /// <param name="roundDuration">The optional per-round duration used to offset each round's start time.</param>
    private static Round CreateRound(int roundIndex, DateOnly date, TimeOnly baseTime, TimeSpan? roundDuration)
    {
        TimeOnly time;
        DateOnly adjustedDate;

        if (roundDuration.HasValue)
        {
            time = baseTime.Add(new TimeSpan(roundDuration.Value.Ticks * roundIndex), out var wrappedDays);
            adjustedDate = date.AddDays(wrappedDays);
        }
        else
        {
            time = baseTime;
            adjustedDate = date;
        }

        return new Round(roundIndex + 1, new Dictionary<int, MatchSchedule>(), adjustedDate, time);
    }

    /// <summary>
    /// Creates the match schedules using the circle-rotation algorithm.
    /// </summary>
    /// <param name="teamList">The materialized, ordered list of teams.</param>
    /// <returns>The <see cref="IEnumerable{MatchSchedule}"/> in ideal scheduling order.</returns>
    /// <remarks>
    /// All teams are placed in a list. For an even number of teams the first team is held fixed and
    /// the remaining teams rotate one position each iteration; for an odd number of teams all teams
    /// rotate. For each iteration the list is folded in half to create matches: the first team plays
    /// the last, the second plays the second-to-last, and so on. For an odd team count the team at
    /// the middle position of the fold receives a bye that iteration. Rotation is performed via a
    /// virtual offset rather than mutating the list, eliminating O(n) shifts per iteration.
    /// </remarks>
    private static IEnumerable<MatchSchedule> CreateMatchSchedules(List<Team> teamList)
    {
        var teamsCount = teamList.Count;
        var matchCount = teamsCount / 2;
        var isEven = teamsCount % 2 == 0;
        var rotatingCount = isEven ? teamsCount - 1 : teamsCount;
        var multiplier = CreateMatchIdMultiplier(matchCount);

        for (var i = 0; i < rotatingCount; i++)
        {
            for (var j = 0; j < matchCount; j++)
            {
                int team1Id, team2Id;

                if (isEven)
                {
                    // teamList[0] is fixed; teamList[1..N-1] rotate via the virtual offset i.
                    // Virtual position k (k >= 1) maps to teamList[1 + (k - 1 + i) % (N - 1)].
                    team1Id = j == 0
                        ? teamList[0].Id
                        : teamList[1 + (j - 1 + i) % rotatingCount].Id;

                    team2Id = j == 0
                        ? teamList[1 + (teamsCount - 2 + i) % rotatingCount].Id
                        : teamList[1 + (teamsCount - j - 2 + i) % rotatingCount].Id;
                }
                else
                {
                    // All teams rotate via the virtual offset i.
                    // The team at the middle position (index matchCount) has a bye each iteration.
                    team1Id = teamList[(j + i) % teamsCount].Id;
                    team2Id = teamList[(teamsCount - j - 1 + i) % teamsCount].Id;
                }

                var id = CreateMatchId(i + 1, j + 1, multiplier);
                yield return new MatchSchedule(id, j + 1, new[] { team1Id, team2Id });
            }
        }
    }

    /// <summary>
    /// Creates the match identifier from the rotation and position indices.
    /// </summary>
    /// <param name="groupId">The rotation (group) index, 1-based.</param>
    /// <param name="instanceId">The position (instance) within the rotation, 1-based.</param>
    /// <param name="multiplier">The spacing multiplier derived from the group size.</param>
    /// <returns>The unique match identifier.</returns>
    private static int CreateMatchId(int groupId, int instanceId, int multiplier)
    {
        return (groupId * multiplier) + instanceId;
    }

    /// <summary>
    /// Creates the multiplier used to space group identifiers so that instance identifiers
    /// within a group do not collide across groups.
    /// </summary>
    /// <param name="groupSize">The number of matches per rotation.</param>
    /// <returns>The smallest power of 10 greater than <paramref name="groupSize"/>.</returns>
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
