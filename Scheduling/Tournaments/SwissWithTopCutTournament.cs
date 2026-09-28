namespace MatchMaker.Scheduling.Tournaments;

using System;
using System.Collections.Generic;
using System.Linq;

using MatchMaker.Models;

/// <summary>
/// Creates and advances Swiss-system tournament schedules whose top-placing teams, after a fixed
/// number of Swiss rounds, advance into a single-elimination "top cut" bracket to determine the
/// final standings.
/// </summary>
/// <remarks>
/// Like <see cref="SwissTournament"/>, neither the number of Swiss rounds nor the size of the top
/// cut is stored on the <see cref="Schedule"/>; both are recomputed as needed from the number of
/// teams. The Swiss stage uses <see cref="SwissTournament.GetSwissRoundCount"/>, and the top cut
/// size is the largest power of two that is at most half the field (see
/// <see cref="GetTopCutSize"/>), so the elimination bracket never needs a bye.
/// </remarks>
public static class SwissWithTopCutTournament
{
    /// <summary>
    /// Creates the initial round of the Swiss stage, identical to <see cref="SwissTournament.Create"/>
    /// except that the resulting schedule's <see cref="Schedule.Type"/> is
    /// <see cref="TournamentType.SwissWithTopCut"/>.
    /// </summary>
    /// <param name="schedule">The schedule containing the full list of teams and quizzers.</param>
    /// <param name="seededTeamIds">
    /// The identifiers of the teams entered in the tournament, ordered from the strongest seed to
    /// the weakest.
    /// </param>
    /// <param name="startDate">
    /// The optional date assigned to the first round. When <see langword="null"/>, the round
    /// defaults to the current date.
    /// </param>
    /// <param name="startTime">
    /// The optional start time assigned to the first round. When <see langword="null"/>, the round
    /// defaults to the current time.
    /// </param>
    /// <returns>The <see cref="Schedule"/> instance containing the seeded teams, their quizzers, and the first round of matches.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="schedule"/> or <paramref name="seededTeamIds"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="seededTeamIds"/> contains fewer than two teams.</exception>
    public static Schedule Create(
        Schedule schedule,
        IReadOnlyList<int> seededTeamIds,
        DateOnly? startDate = null,
        TimeOnly? startTime = null)
    {
        var swissSchedule = SwissTournament.Create(schedule, seededTeamIds, startDate, startTime);
        swissSchedule.Type = TournamentType.SwissWithTopCut;

        return swissSchedule;
    }

    /// <summary>
    /// Determines whether the tournament is complete, that is, whether the top-cut elimination
    /// bracket has been created and has been resolved down to a single champion.
    /// </summary>
    /// <param name="schedule">The tournament schedule.</param>
    /// <param name="result">The <see cref="Result"/> containing recorded match outcomes.</param>
    /// <returns><see langword="true"/> when the tournament has a decided champion; otherwise <see langword="false"/>.</returns>
    public static bool IsComplete(Schedule schedule, Result result)
    {
        ArgumentNullException.ThrowIfNull(schedule);
        ArgumentNullException.ThrowIfNull(result);

        if (schedule.Rounds.Count == 0)
        {
            return false;
        }

        var swissRounds = SwissTournament.GetSwissRoundCount(schedule.Teams.Count);

        if (schedule.Rounds.Count <= swissRounds)
        {
            // Still in the Swiss stage, or the Swiss stage just finished but the cut round has
            // not been created yet.
            return false;
        }

        var currentRound = GetLatestRound(schedule);

        return EliminationTournament.IsRoundComplete(currentRound, result) && GetRoundWinners(currentRound, result).Count <= 1;
    }

    /// <summary>
    /// Creates and appends the next round of the tournament: another Swiss round while the Swiss
    /// stage is in progress, the top-cut bracket's first round once the Swiss stage completes, or
    /// the next elimination round once the top cut is in progress.
    /// </summary>
    /// <param name="schedule">The tournament schedule. The new round, if any, is added directly to <see cref="Schedule.Rounds"/>.</param>
    /// <param name="result">The <see cref="Result"/> containing recorded match outcomes.</param>
    /// <returns>
    /// The newly created <see cref="Round"/>, or <see langword="null"/> when the latest round has
    /// not yet been fully resolved, or when the tournament is already complete.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="schedule"/> or <paramref name="result"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="schedule"/> does not contain any rounds to advance from.</exception>
    public static Round? AdvanceRound(Schedule schedule, Result result)
    {
        ArgumentNullException.ThrowIfNull(schedule);
        ArgumentNullException.ThrowIfNull(result);

        if (schedule.Rounds.Count == 0)
        {
            throw new InvalidOperationException("The schedule does not contain any rounds to advance from.");
        }

        var currentRound = GetLatestRound(schedule);

        if (!EliminationTournament.IsRoundComplete(currentRound, result))
        {
            return null;
        }

        var swissRounds = SwissTournament.GetSwissRoundCount(schedule.Teams.Count);

        if (schedule.Rounds.Count < swissRounds)
        {
            return SwissTournament.AdvanceRound(schedule, result);
        }

        if (schedule.Rounds.Count == swissRounds)
        {
            return CreateCutRound(schedule, result, swissRounds);
        }

        return AdvanceCutRound(schedule, result);
    }

    /// <summary>
    /// Gets the number of teams that advance from the Swiss stage into the top-cut elimination
    /// bracket: the largest power of two that is at most half the field (so the bracket never
    /// needs a bye), with a minimum of two teams.
    /// </summary>
    /// <param name="teamCount">The number of teams entered in the tournament.</param>
    /// <returns>The number of teams that advance to the top cut.</returns>
    internal static int GetTopCutSize(int teamCount)
    {
        var size = 1;

        while (size * 2 <= teamCount)
        {
            size *= 2;
        }

        if (size >= teamCount && size > 2)
        {
            size /= 2;
        }

        return Math.Max(size, 2);
    }

    /// <summary>
    /// Creates the first round of the top-cut elimination bracket from the final Swiss standings,
    /// pairing the strongest remaining seed against the weakest remaining seed.
    /// </summary>
    private static Round CreateCutRound(Schedule schedule, Result result, int swissRounds)
    {
        var wins = SwissTournament.GetWinCounts(schedule, result);
        var cutSize = GetTopCutSize(schedule.Teams.Count);
        var seeded = schedule.Teams.Keys
            .OrderByDescending(id => wins.GetValueOrDefault(id))
            .ThenBy(id => id)
            .Take(cutSize)
            .ToList();

        var currentRound = GetLatestRound(schedule);
        var cutRound = CreatePairingRound(swissRounds + 1, seeded, currentRound.Date, currentRound.Time);
        schedule.Rounds.Add(cutRound.Id, cutRound);

        return cutRound;
    }

    /// <summary>
    /// Creates and appends the next round of the top-cut elimination bracket from the results of
    /// the latest round.
    /// </summary>
    private static Round? AdvanceCutRound(Schedule schedule, Result result)
    {
        var currentRound = GetLatestRound(schedule);
        var winners = GetRoundWinners(currentRound, result);

        if (winners.Count < 2)
        {
            return null;
        }

        var nextRound = CreatePairingRound(currentRound.Id + 1, winners, currentRound.Date, currentRound.Time);
        schedule.Rounds.Add(nextRound.Id, nextRound);

        return nextRound;
    }

    /// <summary>
    /// Gets the identifiers of the teams that won each match of the given top-cut round, in
    /// ascending match order.
    /// </summary>
    private static List<int> GetRoundWinners(Round round, Result result)
    {
        return round.Matches.Values
            .OrderBy(m => m.Id)
            .Select(m => result.Matches[GetScheduleId(round, m)].TeamResults.First(t => t.Place == 1).TeamId)
            .ToList();
    }

    /// <summary>
    /// Gets the identifier used to look up a match's recorded result within a <see cref="Result"/>.
    /// </summary>
    private static int GetScheduleId(Round round, MatchSchedule match)
    {
        return (round.Id * 100) + match.Room;
    }

    /// <summary>
    /// Gets the most recently created round of the schedule.
    /// </summary>
    private static Round GetLatestRound(Schedule schedule)
    {
        return schedule.Rounds.Values.OrderByDescending(r => r.Id).First();
    }

    /// <summary>
    /// Creates a round pairing the given teams using the strongest-versus-weakest fold. The
    /// top-cut bracket size is always a power of two, so this never needs to assign a bye.
    /// </summary>
    private static Round CreatePairingRound(int roundId, List<int> teamIdsInOrder, DateOnly date, TimeOnly time)
    {
        var matches = new Dictionary<int, MatchSchedule>();
        var matchCount = teamIdsInOrder.Count / 2;

        for (var i = 0; i < matchCount; i++)
        {
            var team1Id = teamIdsInOrder[i];
            var team2Id = teamIdsInOrder[teamIdsInOrder.Count - 1 - i];
            var matchId = i + 1;
            matches.Add(matchId, new MatchSchedule(matchId, matchId, new[] { team1Id, team2Id }));
        }

        return new Round(roundId, matches, date, time);
    }
}
