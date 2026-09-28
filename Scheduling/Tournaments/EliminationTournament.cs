namespace MatchMaker.Scheduling.Tournaments;

using System;
using System.Collections.Generic;
using System.Linq;

using MatchMaker.Models;

/// <summary>
/// Creates single-elimination tournament schedules for a fixed set of seeded teams.
/// </summary>
public static class EliminationTournament
{
    /// <summary>
    /// Creates a single-elimination tournament schedule containing only the given seeded teams
    /// and their quizzers, with a first round pairing the strongest remaining seed against the
    /// weakest remaining seed (for example, seed 1 versus seed 4, and seed 2 versus seed 3).
    /// </summary>
    /// <param name="schedule">The schedule containing the full list of teams and quizzers.</param>
    /// <param name="seededTeamIds">
    /// The identifiers of the teams that qualify for the elimination tournament, ordered from the
    /// strongest seed to the weakest.
    /// </param>
    /// <param name="startDate">
    /// The optional date assigned to the first round. When <see langword="null"/>, the round
    /// defaults to the current date.
    /// </param>
    /// <param name="startTime">
    /// The optional start time assigned to the first round. When <see langword="null"/>, the round
    /// defaults to the current time.
    /// </param>
    /// <returns>The <see cref="Schedule"/> instance containing only the seeded teams, their quizzers, and the first round of matches.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="schedule"/> or <paramref name="seededTeamIds"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="seededTeamIds"/> contains fewer than two teams.</exception>
    /// <remarks>
    /// When an odd number of teams is given, the strongest seed receives a bye in the first round;
    /// the remaining teams are paired using the same strongest-versus-weakest pattern.
    /// </remarks>
    public static Schedule Create(
        Schedule schedule,
        IReadOnlyList<int> seededTeamIds,
        DateOnly? startDate = null,
        TimeOnly? startTime = null)
    {
        ArgumentNullException.ThrowIfNull(schedule);
        ArgumentNullException.ThrowIfNull(seededTeamIds);

        if (seededTeamIds.Count < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(seededTeamIds), seededTeamIds.Count, "At least two teams are required to create an elimination tournament.");
        }

        var teams = seededTeamIds.ToDictionary(id => id, id => schedule.Teams[id]);
        var quizzers = schedule.Quizzers
            .Where(x => teams.ContainsKey(x.Value.TeamId))
            .ToDictionary(x => x.Key, x => x.Value);

        var round = CreateFirstRound(seededTeamIds, startDate, startTime);

        return new Schedule(schedule.Name, schedule.Churches, quizzers, teams, new Dictionary<int, Round> { { round.Id, round } }, TournamentType.SingleElimination);
    }

    /// <summary>
    /// Determines whether every match in the given <paramref name="round"/> has a recorded result.
    /// </summary>
    /// <param name="round">The round to inspect.</param>
    /// <param name="result">The <see cref="Result"/> containing recorded match outcomes.</param>
    /// <returns><see langword="true"/> when every match in the round has a result; otherwise <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="round"/> or <paramref name="result"/> is <see langword="null"/>.</exception>
    public static bool IsRoundComplete(Round round, Result result)
    {
        ArgumentNullException.ThrowIfNull(round);
        ArgumentNullException.ThrowIfNull(result);

        return round.Matches.Values.All(match => result.Matches.ContainsKey(GetScheduleId(round, match)));
    }

    /// <summary>
    /// Gets the identifier of the team recorded as the sole winner (<see cref="TeamResult.Place"/>
    /// equal to <c>1</c>) of the given match result.
    /// </summary>
    /// <param name="matchResult">The recorded match result.</param>
    /// <returns>The winning team's identifier.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the match result does not contain exactly one team result recorded with
    /// <see cref="TeamResult.Place"/> equal to <c>1</c>, for example because of a tie or a
    /// malformed/incomplete result.
    /// </exception>
    internal static int GetWinnerId(MatchResult matchResult)
    {
        ArgumentNullException.ThrowIfNull(matchResult);

        var winners = matchResult.TeamResults.Where(t => t.Place == 1).ToList();

        return winners.Count == 1
            ? winners[0].TeamId
            : throw new InvalidOperationException(
                FormattableString.Invariant(
                    $"Match result {matchResult.ScheduleId} must have exactly one team recorded with place 1, but found {winners.Count}."));
    }

    /// <summary>
    /// Gets the identifier of the team recorded as the sole loser (any <see cref="TeamResult.Place"/>
    /// other than <c>1</c>) of a two-team match result.
    /// </summary>
    /// <param name="matchResult">The recorded match result.</param>
    /// <returns>The losing team's identifier.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the match result does not contain exactly two team results, or does not
    /// contain exactly one team result recorded with a place other than <c>1</c>.
    /// </exception>
    internal static int GetLoserId(MatchResult matchResult)
    {
        ArgumentNullException.ThrowIfNull(matchResult);

        var losers = matchResult.TeamResults.Where(t => t.Place != 1).ToList();

        return matchResult.TeamResults.Count == 2 && losers.Count == 1
            ? losers[0].TeamId
            : throw new InvalidOperationException(
                FormattableString.Invariant(
                    $"Match result {matchResult.ScheduleId} must have exactly two teams with a single non-winning team, but found {matchResult.TeamResults.Count} team(s) and {losers.Count} non-winner(s)."));
    }

    /// <summary>
    /// Determines whether the elimination tournament is complete, that is, whether the latest
    /// round has been fully resolved and only a single team (the champion) remains.
    /// </summary>
    /// <param name="schedule">The elimination tournament schedule.</param>
    /// <param name="result">The <see cref="Result"/> containing recorded match outcomes.</param>
    /// <returns>
    /// <see langword="true"/> when the tournament has a decided champion; <see langword="false"/>
    /// when the schedule has no rounds yet, or the latest round has not been fully resolved, or
    /// more than one team remains.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="schedule"/> or <paramref name="result"/> is <see langword="null"/>.</exception>
    public static bool IsComplete(Schedule schedule, Result result)
    {
        ArgumentNullException.ThrowIfNull(schedule);
        ArgumentNullException.ThrowIfNull(result);

        if (schedule.Rounds.Count == 0)
        {
            return false;
        }

        var currentRound = GetLatestRound(schedule);

        return IsRoundComplete(currentRound, result) && GetWinnersInOrder(schedule, currentRound, result).Count <= 1;
    }

    /// <summary>
    /// Gets the identifiers of the teams that won each match of the given round, in the same
    /// strongest-to-weakest order used to construct that round: the round's bye team (if any)
    /// first, followed by each match's winner in ascending match order.
    /// </summary>
    /// <param name="schedule">The elimination tournament schedule.</param>
    /// <param name="round">The completed round.</param>
    /// <param name="result">The <see cref="Result"/> containing recorded match outcomes.</param>
    /// <returns>The winning team identifiers, ordered from strongest to weakest.</returns>
    private static List<int> GetWinnersInOrder(Schedule schedule, Round round, Result result)
    {
        var winners = new List<int>();
        var byeTeamId = GetByeTeamId(schedule, round, result);

        if (byeTeamId.HasValue)
        {
            winners.Add(byeTeamId.Value);
        }

        foreach (var match in round.Matches.Values.OrderBy(m => m.Id))
        {
            var matchResult = result.Matches[GetScheduleId(round, match)];
            var winnerId = GetWinnerId(matchResult);
            winners.Add(winnerId);
        }

        return winners;
    }

    /// <summary>
    /// Gets the identifier of the team that has a bye in the given round, that is, the one team
    /// still in the tournament at the start of the round that does not appear in any of the
    /// round's matches. Unlike <see cref="ScheduleExtensions.GetByeTeamId"/> (which assumes every
    /// team in the schedule plays every round, as in round robin), this accounts for teams
    /// eliminated in earlier rounds by excluding anyone with a recorded loss in a prior round.
    /// </summary>
    /// <param name="schedule">The elimination tournament schedule.</param>
    /// <param name="round">The round to inspect.</param>
    /// <param name="result">The <see cref="Result"/> containing recorded match outcomes, including prior rounds.</param>
    /// <returns>The bye team identifier, or <see langword="null"/> when no single team has a bye.</returns>
    private static int? GetByeTeamId(Schedule schedule, Round round, Result result)
    {
        var eliminated = result.Matches.Values
            .Where(m => m.Round < round.Id)
            .SelectMany(m => m.TeamResults)
            .Where(t => t.Place != 1)
            .Select(t => t.TeamId);

        var stillIn = new HashSet<int>(schedule.Teams.Keys);
        stillIn.ExceptWith(eliminated);
        stillIn.ExceptWith(round.Matches.Values.SelectMany(m => m.Teams));

        return stillIn.Count == 1 ? stillIn.Single() : null;
    }

    /// <summary>
    /// Creates and appends the next round of the elimination tournament from the results of the
    /// latest round, pairing the winners (and the current round's bye team, if any) using the
    /// same strongest-versus-weakest pattern used to build the first round. This preserves the
    /// standard "protect the seed" bracket structure without needing to track original seed
    /// values: at every round, the surviving teams are already ordered from strongest to weakest
    /// by construction, so re-applying the identical fold produces the correct next-round pairings.
    /// </summary>
    /// <param name="schedule">The elimination tournament schedule. The new round, if any, is added directly to <see cref="Schedule.Rounds"/>.</param>
    /// <param name="result">The <see cref="Result"/> containing recorded match outcomes.</param>
    /// <returns>
    /// The newly created <see cref="Round"/>, or <see langword="null"/> when the latest round has
    /// not yet been fully resolved, or when the tournament is already complete (a single champion
    /// remains and no further round is needed).
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

        if (!IsRoundComplete(currentRound, result))
        {
            return null;
        }

        var winners = GetWinnersInOrder(schedule, currentRound, result);

        if (winners.Count < 2)
        {
            return null;
        }

        var nextRound = CreatePairingRound(currentRound.Id + 1, winners, currentRound.Date, currentRound.Time);
        schedule.Rounds.Add(nextRound.Id, nextRound);

        return nextRound;
    }

    /// <summary>
    /// Gets the most recently created round of the schedule.
    /// </summary>
    /// <param name="schedule">The elimination tournament schedule.</param>
    /// <returns>The <see cref="Round"/> with the highest <see cref="Round.Id"/>.</returns>
    private static Round GetLatestRound(Schedule schedule)
    {
        return schedule.Rounds.Values.OrderByDescending(r => r.Id).First();
    }

    /// <summary>
    /// Gets the identifier used to look up a match's recorded result within a <see cref="Result"/>.
    /// </summary>
    /// <param name="round">The round containing the match.</param>
    /// <param name="match">The match.</param>
    /// <returns>The schedule identifier, matching <see cref="MatchResult.ScheduleId"/>.</returns>
    private static int GetScheduleId(Round round, MatchSchedule match)
    {
        return MatchResult.GetScheduleId(round.Id, match.Room);
    }

    /// <summary>
    /// Creates the first round of the elimination tournament.
    /// </summary>
    /// <param name="seededTeamIds">The seeded team identifiers, strongest seed first.</param>
    /// <param name="startDate">The optional round date.</param>
    /// <param name="startTime">The optional round start time.</param>
    /// <returns>The first <see cref="Round"/> instance.</returns>
    private static Round CreateFirstRound(IReadOnlyList<int> seededTeamIds, DateOnly? startDate, TimeOnly? startTime)
    {
        var now = DateTime.Now;
        var date = startDate ?? DateOnly.FromDateTime(now);
        var time = startTime ?? TimeOnly.FromDateTime(now);

        return CreatePairingRound(1, seededTeamIds, date, time);
    }

    /// <summary>
    /// Creates a round pairing the given teams using the strongest-versus-weakest fold: the
    /// strongest remaining team plays the weakest, the second-strongest plays the
    /// second-weakest, and so on. When an odd number of teams is given, the first (strongest)
    /// team receives a bye and the remaining teams are paired using the same pattern.
    /// </summary>
    /// <param name="roundId">The identifier assigned to the new round.</param>
    /// <param name="teamIdsInOrder">The team identifiers, ordered from strongest to weakest.</param>
    /// <param name="date">The round date.</param>
    /// <param name="time">The round start time.</param>
    /// <returns>The <see cref="Round"/> instance.</returns>
    private static Round CreatePairingRound(int roundId, IReadOnlyList<int> teamIdsInOrder, DateOnly date, TimeOnly time)
    {
        // For an odd number of teams the strongest remaining team sits out this round; the
        // remaining teams are paired using the same strongest-versus-weakest pattern.
        var pairingIds = teamIdsInOrder.Count % 2 == 0 ? teamIdsInOrder : teamIdsInOrder.Skip(1).ToList();

        var matches = new Dictionary<int, MatchSchedule>();
        var matchCount = pairingIds.Count / 2;

        for (var i = 0; i < matchCount; i++)
        {
            var team1Id = pairingIds[i];
            var team2Id = pairingIds[pairingIds.Count - 1 - i];
            var matchId = i + 1;
            matches.Add(matchId, new MatchSchedule(matchId, matchId, new[] { team1Id, team2Id }));
        }

        return new Round(roundId, matches, date, time);
    }
}
