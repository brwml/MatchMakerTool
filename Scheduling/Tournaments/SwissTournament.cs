namespace MatchMaker.Scheduling.Tournaments;

using System;
using System.Collections.Generic;
using System.Linq;

using MatchMaker.Models;

/// <summary>
/// Creates and advances Swiss-system tournament schedules, in which pairings for each round are
/// recomputed from the current standings rather than fixed in advance.
/// </summary>
/// <remarks>
/// The number of rounds is not stored on the <see cref="Schedule"/>; instead it is recomputed as
/// needed from the number of teams (see <see cref="GetSwissRoundCount"/>), using the standard
/// recommendation of enough rounds to guarantee a single unbeaten leader (<c>ceil(log2(n))</c>).
/// Byes and rematches are avoided using a simple standings-based greedy pairing; unlike a
/// tournament-management system, this does not track historical bye counts, so with an odd number
/// of teams the weakest remaining team tends to receive repeat byes.
/// </remarks>
public static class SwissTournament
{
    /// <summary>
    /// Creates the initial round of a Swiss-system tournament, pairing the stronger (top) half of
    /// the seeded teams against the weaker (bottom) half, in seed order (for example, with eight
    /// teams: 1 vs 5, 2 vs 6, 3 vs 7, and 4 vs 8).
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
    /// <remarks>
    /// When an odd number of teams is given, the weakest seed receives a bye in the first round.
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
            throw new ArgumentOutOfRangeException(nameof(seededTeamIds), seededTeamIds.Count, "At least two teams are required to create a Swiss-system tournament.");
        }

        var teams = seededTeamIds.ToDictionary(id => id, id => schedule.Teams[id]);
        var quizzers = schedule.Quizzers
            .Where(x => teams.ContainsKey(x.Value.TeamId))
            .ToDictionary(x => x.Key, x => x.Value);

        var now = DateTime.Now;
        var date = startDate ?? DateOnly.FromDateTime(now);
        var time = startTime ?? TimeOnly.FromDateTime(now);
        var round = CreateFirstRound(seededTeamIds, date, time);

        return new Schedule(schedule.Name, schedule.Churches, quizzers, teams, new Dictionary<int, Round> { { round.Id, round } }, TournamentType.Swiss);
    }

    /// <summary>
    /// Determines whether the Swiss-system tournament has completed all of its rounds, that is,
    /// whether the recommended number of rounds (see <see cref="GetSwissRoundCount"/>) has been
    /// reached and the latest round has been fully resolved.
    /// </summary>
    /// <param name="schedule">The Swiss-system tournament schedule.</param>
    /// <param name="result">The <see cref="Result"/> containing recorded match outcomes.</param>
    /// <returns><see langword="true"/> when no further Swiss rounds are needed; otherwise <see langword="false"/>.</returns>
    public static bool IsComplete(Schedule schedule, Result result)
    {
        ArgumentNullException.ThrowIfNull(schedule);
        ArgumentNullException.ThrowIfNull(result);

        if (schedule.Rounds.Count == 0)
        {
            return false;
        }

        var targetRounds = GetSwissRoundCount(schedule.Teams.Count);
        var currentRound = GetLatestRound(schedule);

        return schedule.Rounds.Count >= targetRounds && EliminationTournament.IsRoundComplete(currentRound, result);
    }

    /// <summary>
    /// Creates and appends the next round of the Swiss-system tournament, pairing teams with the
    /// most similar records (most wins first) while avoiding rematches when possible.
    /// </summary>
    /// <param name="schedule">The Swiss-system tournament schedule. The new round, if any, is added directly to <see cref="Schedule.Rounds"/>.</param>
    /// <param name="result">The <see cref="Result"/> containing recorded match outcomes.</param>
    /// <returns>
    /// The newly created <see cref="Round"/>, or <see langword="null"/> when the latest round has
    /// not yet been fully resolved, or when the tournament has already completed all of its rounds.
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

        if (IsComplete(schedule, result))
        {
            return null;
        }

        var wins = GetWinCounts(schedule, result);
        var playedPairs = GetPlayedPairs(schedule);
        var standingsOrder = schedule.Teams.Keys.OrderByDescending(id => wins.GetValueOrDefault(id)).ThenBy(id => id).ToList();
        var pairs = PairByStandings(standingsOrder, playedPairs);

        var nextRoundId = currentRound.Id + 1;
        var matches = new Dictionary<int, MatchSchedule>();

        for (var i = 0; i < pairs.Count; i++)
        {
            var matchId = i + 1;
            matches.Add(matchId, new MatchSchedule(matchId, matchId, new[] { pairs[i].Team1, pairs[i].Team2 }));
        }

        var nextRound = new Round(nextRoundId, matches, currentRound.Date, currentRound.Time);
        schedule.Rounds.Add(nextRoundId, nextRound);

        return nextRound;
    }

    /// <summary>
    /// Gets the recommended number of Swiss rounds for the given number of teams: enough rounds
    /// that a single unbeaten leader is guaranteed, that is, <c>ceil(log2(teamCount))</c>.
    /// </summary>
    /// <param name="teamCount">The number of teams entered in the tournament.</param>
    /// <returns>The recommended number of Swiss rounds.</returns>
    internal static int GetSwissRoundCount(int teamCount)
    {
        return (int)Math.Ceiling(Math.Log2(Math.Max(teamCount, 2)));
    }

    /// <summary>
    /// Gets the number of match wins recorded for each team across all rounds played so far. A
    /// team that received a bye (played no match in a round) is not credited with a win for that
    /// round.
    /// </summary>
    internal static Dictionary<int, int> GetWinCounts(Schedule schedule, Result result)
    {
        var wins = schedule.Teams.Keys.ToDictionary(id => id, _ => 0);

        foreach (var matchResult in result.Matches.Values)
        {
            var winnerId = matchResult.TeamResults.FirstOrDefault(t => t.Place == 1)?.TeamId;

            if (winnerId.HasValue && wins.TryGetValue(winnerId.Value, out var winCount))
            {
                wins[winnerId.Value] = winCount + 1;
            }
        }

        return wins;
    }

    /// <summary>
    /// Gets the set of team pairs that have already played each other in any round scheduled so
    /// far, regardless of whether that round's result has been recorded yet.
    /// </summary>
    private static HashSet<(int Team1, int Team2)> GetPlayedPairs(Schedule schedule)
    {
        var pairs = new HashSet<(int, int)>();

        foreach (var round in schedule.Rounds.Values)
        {
            foreach (var match in round.Matches.Values.Where(m => m.Teams.Count == 2))
            {
                var ordered = match.Teams.OrderBy(id => id).ToList();
                pairs.Add((ordered[0], ordered[1]));
            }
        }

        return pairs;
    }

    /// <summary>
    /// Pairs teams from the given standings order (strongest first), greedily matching each team
    /// with the nearest team below it in the standings that it has not already played. When no
    /// unplayed opponent remains for a team, it is paired with the next available team (a
    /// rematch). When an odd number of teams is given, the weakest team is left unpaired (a bye).
    /// </summary>
    private static List<(int Team1, int Team2)> PairByStandings(List<int> standingsOrder, HashSet<(int Team1, int Team2)> playedPairs)
    {
        var remaining = new List<int>(standingsOrder);
        var pairs = new List<(int Team1, int Team2)>();

        while (remaining.Count > 1)
        {
            var team1 = remaining[0];
            remaining.RemoveAt(0);

            var opponentIndex = remaining.FindIndex(id => !HasPlayed(playedPairs, team1, id));

            if (opponentIndex < 0)
            {
                opponentIndex = 0;
            }

            var team2 = remaining[opponentIndex];
            remaining.RemoveAt(opponentIndex);

            pairs.Add((team1, team2));
        }

        return pairs;
    }

    /// <summary>
    /// Determines whether the given two teams have already played each other.
    /// </summary>
    private static bool HasPlayed(HashSet<(int Team1, int Team2)> playedPairs, int team1, int team2)
    {
        return team1 < team2 ? playedPairs.Contains((team1, team2)) : playedPairs.Contains((team2, team1));
    }

    /// <summary>
    /// Gets the most recently created round of the schedule.
    /// </summary>
    private static Round GetLatestRound(Schedule schedule)
    {
        return schedule.Rounds.Values.OrderByDescending(r => r.Id).First();
    }

    /// <summary>
    /// Creates the first round of the Swiss-system tournament, pairing the top half of the seeded
    /// teams against the bottom half, in seed order.
    /// </summary>
    private static Round CreateFirstRound(IReadOnlyList<int> seededTeamIds, DateOnly date, TimeOnly time)
    {
        // For an odd number of teams the weakest seed sits out this round.
        var pairingIds = seededTeamIds.Count % 2 == 0 ? seededTeamIds : seededTeamIds.Take(seededTeamIds.Count - 1).ToList();
        var half = pairingIds.Count / 2;
        var matches = new Dictionary<int, MatchSchedule>();

        for (var i = 0; i < half; i++)
        {
            var matchId = i + 1;
            matches.Add(matchId, new MatchSchedule(matchId, matchId, new[] { pairingIds[i], pairingIds[half + i] }));
        }

        return new Round(1, matches, date, time);
    }
}
