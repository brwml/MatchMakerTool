namespace MatchMaker.Scheduling.Tournaments;

using System;
using System.Collections.Generic;
using System.Linq;

using MatchMaker.Models;

/// <summary>
/// Implements the shared round-by-round mechanics for elimination tournaments in which a team is
/// eliminated only after accumulating a fixed number of losses (its "lives"), such as
/// <see cref="DoubleEliminationTournament"/> (two lives) and <see cref="TripleEliminationTournament"/>
/// (three lives). Single-elimination (one life) predates this generalization and remains
/// independently implemented by <see cref="EliminationTournament"/>.
/// </summary>
/// <remarks>
/// Unlike a traditional double/triple-elimination bracket, which tracks separate winners' and
/// losers' brackets so that a team that has already lost never faces a team that has not, this
/// implementation keeps a single evolving pool of not-yet-eliminated teams and re-pairs it each
/// round using the same strongest-versus-weakest fold used for single elimination, ordering teams
/// first by fewest accumulated losses and then by their finishing position in the prior round.
/// This satisfies the documented behavior of <see cref="TournamentType.DoubleElimination"/> and
/// <see cref="TournamentType.TripleElimination"/> (elimination after a fixed number of losses)
/// without requiring a dedicated bracket data structure.
/// </remarks>
internal static class MultiLifeEliminationTournament
{
    /// <summary>
    /// Creates the initial round of a multi-life elimination tournament. The first round pairing
    /// is identical to single elimination's, since every team begins with zero losses.
    /// </summary>
    /// <param name="schedule">The schedule containing the full list of teams and quizzers.</param>
    /// <param name="seededTeamIds">The seeded team identifiers, strongest seed first.</param>
    /// <param name="type">The tournament type recorded on the returned schedule.</param>
    /// <param name="startDate">The optional date assigned to the first round.</param>
    /// <param name="startTime">The optional start time assigned to the first round.</param>
    /// <returns>The <see cref="Schedule"/> instance containing only the seeded teams, their quizzers, and the first round of matches.</returns>
    public static Schedule Create(
        Schedule schedule,
        IReadOnlyList<int> seededTeamIds,
        TournamentType type,
        DateOnly? startDate,
        TimeOnly? startTime)
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

        var now = DateTime.Now;
        var date = startDate ?? DateOnly.FromDateTime(now);
        var time = startTime ?? TimeOnly.FromDateTime(now);
        var round = CreatePairingRound(1, seededTeamIds, date, time);

        return new Schedule(schedule.Name, schedule.Churches, quizzers, teams, new Dictionary<int, Round> { { round.Id, round } }, type);
    }

    /// <summary>
    /// Determines whether the tournament is complete, that is, whether the latest round has been
    /// fully resolved and only a single team remains that has not yet accumulated <paramref name="lives"/> losses.
    /// </summary>
    /// <param name="schedule">The elimination tournament schedule.</param>
    /// <param name="result">The <see cref="Result"/> containing recorded match outcomes.</param>
    /// <param name="lives">The number of losses a team may accumulate before being eliminated.</param>
    /// <returns><see langword="true"/> when the tournament has a decided champion; otherwise <see langword="false"/>.</returns>
    public static bool IsComplete(Schedule schedule, Result result, int lives)
    {
        ArgumentNullException.ThrowIfNull(schedule);
        ArgumentNullException.ThrowIfNull(result);

        if (schedule.Rounds.Count == 0)
        {
            return false;
        }

        var currentRound = GetLatestRound(schedule);

        return EliminationTournament.IsRoundComplete(currentRound, result) && GetSurvivorsInOrder(schedule, currentRound, result, lives).Count <= 1;
    }

    /// <summary>
    /// Creates and appends the next round of the tournament from the results of the latest round,
    /// re-pairing every team that has not yet accumulated <paramref name="lives"/> losses using the
    /// strongest-versus-weakest fold, ordered by fewest losses first.
    /// </summary>
    /// <param name="schedule">The elimination tournament schedule. The new round, if any, is added directly to <see cref="Schedule.Rounds"/>.</param>
    /// <param name="result">The <see cref="Result"/> containing recorded match outcomes.</param>
    /// <param name="lives">The number of losses a team may accumulate before being eliminated.</param>
    /// <returns>
    /// The newly created <see cref="Round"/>, or <see langword="null"/> when the latest round has
    /// not yet been fully resolved, or when the tournament is already complete.
    /// </returns>
    public static Round? AdvanceRound(Schedule schedule, Result result, int lives)
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

        var survivors = GetSurvivorsInOrder(schedule, currentRound, result, lives);

        if (survivors.Count < 2)
        {
            return null;
        }

        var nextRound = CreatePairingRound(currentRound.Id + 1, survivors, currentRound.Date, currentRound.Time);
        schedule.Rounds.Add(nextRound.Id, nextRound);

        return nextRound;
    }

    /// <summary>
    /// Gets the surviving teams (those with fewer than <paramref name="lives"/> losses once the
    /// given round is accounted for), ordered from fewest losses to most, preserving the relative
    /// strongest-to-weakest ordering of the given round for teams with equal loss counts.
    /// </summary>
    private static List<int> GetSurvivorsInOrder(Schedule schedule, Round round, Result result, int lives)
    {
        var lossCountsThroughRound = GetLossCounts(result, round.Id);
        var byeTeamId = GetByeTeamId(schedule, round, result, lives);
        var ordered = new List<int>();

        if (byeTeamId.HasValue)
        {
            ordered.Add(byeTeamId.Value);
        }

        foreach (var match in round.Matches.Values.OrderBy(m => m.Id))
        {
            var matchResult = result.Matches[GetScheduleId(round, match)];
            var winnerId = matchResult.TeamResults.First(t => t.Place == 1).TeamId;
            var loserId = matchResult.TeamResults.First(t => t.Place != 1).TeamId;

            ordered.Add(winnerId);

            if (lossCountsThroughRound.GetValueOrDefault(loserId) < lives)
            {
                ordered.Add(loserId);
            }
        }

        // OrderBy is a stable sort, so teams with equal loss counts retain the relative order
        // established above (bye first, then each match's winner followed by its surviving loser).
        return [.. ordered.OrderBy(id => lossCountsThroughRound.GetValueOrDefault(id))];
    }

    /// <summary>
    /// Gets the identifier of the team that has a bye in the given round, that is, the one team
    /// not yet eliminated (fewer than <paramref name="lives"/> losses through the prior round) that
    /// does not appear in any of the round's matches.
    /// </summary>
    private static int? GetByeTeamId(Schedule schedule, Round round, Result result, int lives)
    {
        var lossCountsBeforeRound = GetLossCounts(result, round.Id - 1);
        var stillIn = new HashSet<int>(schedule.Teams.Keys.Where(id => lossCountsBeforeRound.GetValueOrDefault(id) < lives));
        stillIn.ExceptWith(round.Matches.Values.SelectMany(m => m.Teams));

        return stillIn.Count == 1 ? stillIn.Single() : null;
    }

    /// <summary>
    /// Gets the number of losses recorded for each team across all matches through the given round.
    /// </summary>
    private static Dictionary<int, int> GetLossCounts(Result result, int throughRoundId)
    {
        return result.Matches.Values
            .Where(m => m.Round <= throughRoundId)
            .SelectMany(m => m.TeamResults)
            .Where(t => t.Place != 1)
            .GroupBy(t => t.TeamId)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    /// <summary>
    /// Gets the most recently created round of the schedule.
    /// </summary>
    private static Round GetLatestRound(Schedule schedule)
    {
        return schedule.Rounds.Values.OrderByDescending(r => r.Id).First();
    }

    /// <summary>
    /// Gets the identifier used to look up a match's recorded result within a <see cref="Result"/>.
    /// </summary>
    private static int GetScheduleId(Round round, MatchSchedule match)
    {
        return (round.Id * 100) + match.Room;
    }

    /// <summary>
    /// Creates a round pairing the given teams using the strongest-versus-weakest fold, giving the
    /// strongest remaining team a bye when an odd number of teams is given.
    /// </summary>
    private static Round CreatePairingRound(int roundId, IReadOnlyList<int> teamIdsInOrder, DateOnly date, TimeOnly time)
    {
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
