namespace MatchMaker.Scheduling.Teams;

using System;
using System.Collections.Generic;
using System.Linq;

using MatchMaker.Models;

/// <summary>
/// Creates evenly matched teams from a ranked list of quizzers, minimizing the number of quizzers
/// from the same church that are assigned to the same team.
/// </summary>
public static class BalancedTeamAssigner
{
    /// <summary>
    /// Creates a new set of teams named "Team A", "Team B", and so on, distributing the given
    /// quizzers across the teams in serpentine (snake draft) order so that each team receives a
    /// similar overall skill level, then applies a best-effort pass to reduce the number of
    /// quizzers from the same church placed on the same team.
    /// </summary>
    /// <param name="schedule">The schedule containing the quizzers to distribute.</param>
    /// <param name="rankedQuizzerIds">
    /// The identifiers of the quizzers to distribute, ordered from the best-placing quizzer to the
    /// worst-placing quizzer.
    /// </param>
    /// <param name="numberOfTeams">The number of teams to create. Must be at least 1.</param>
    /// <returns>The <see cref="Schedule"/> instance containing the new teams and reassigned quizzers.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="schedule"/> or <paramref name="rankedQuizzerIds"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="numberOfTeams"/> is less than 1.</exception>
    public static Schedule Create(Schedule schedule, IReadOnlyList<int> rankedQuizzerIds, int numberOfTeams)
    {
        ArgumentNullException.ThrowIfNull(schedule);
        ArgumentNullException.ThrowIfNull(rankedQuizzerIds);

        if (numberOfTeams < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(numberOfTeams), numberOfTeams, "The number of teams must be at least 1.");
        }

        var teams = CreateTeams(numberOfTeams);
        var groups = CreateSnakeDraftGroups(rankedQuizzerIds, numberOfTeams);
        BalanceChurches(groups, schedule.Quizzers, rankedQuizzerIds);

        var quizzers = CreateQuizzers(groups, schedule.Quizzers);

        return new Schedule(schedule.Name, schedule.Churches, quizzers, teams, new Dictionary<int, Round>());
    }

    /// <summary>
    /// Converts a 1-based index into an Excel-style column letter (1 = A, 26 = Z, 27 = AA, ...).
    /// </summary>
    /// <param name="index">The 1-based index.</param>
    /// <returns>The letter designation.</returns>
    private static string GetTeamLetters(int index)
    {
        var letters = string.Empty;

        while (index > 0)
        {
            index--;
            letters = (char)('A' + (index % 26)) + letters;
            index /= 26;
        }

        return letters;
    }

    /// <summary>
    /// Creates the teams map, named "Team A", "Team B", and so on.
    /// </summary>
    /// <param name="numberOfTeams">The number of teams to create.</param>
    /// <returns>The teams map.</returns>
    private static Dictionary<int, Team> CreateTeams(int numberOfTeams)
    {
        var teams = new Dictionary<int, Team>();

        for (var i = 1; i <= numberOfTeams; i++)
        {
            var letters = GetTeamLetters(i);
            teams.Add(i, new Team(i, $"Team {letters}", letters, 0));
        }

        return teams;
    }

    /// <summary>
    /// Distributes the ranked quizzers across the given number of teams using a serpentine
    /// (snake draft) pattern: the first pass assigns teams 0, 1, 2, ..., N-1, the second pass
    /// assigns teams N-1, N-2, ..., 0, and so on, so that overall skill is evenly distributed.
    /// </summary>
    /// <param name="rankedQuizzerIds">The ranked quizzer identifiers, best first.</param>
    /// <param name="numberOfTeams">The number of teams.</param>
    /// <returns>The list of quizzer identifier groups, one per team, indexed from 0.</returns>
    private static List<List<int>> CreateSnakeDraftGroups(IReadOnlyList<int> rankedQuizzerIds, int numberOfTeams)
    {
        var groups = Enumerable.Range(0, numberOfTeams).Select(_ => new List<int>()).ToList();

        for (var i = 0; i < rankedQuizzerIds.Count; i++)
        {
            var (round, position) = Math.DivRem(i, numberOfTeams);
            var teamIndex = round % 2 == 0 ? position : numberOfTeams - position - 1;
            groups[teamIndex].Add(rankedQuizzerIds[i]);
        }

        return groups;
    }

    /// <summary>
    /// Applies a best-effort greedy swap pass to reduce the number of quizzers from the same
    /// church assigned to the same team. Swap candidates are chosen to minimize disruption to the
    /// skill balance established by the snake draft: among all valid swaps, the one exchanging
    /// quizzers with the closest original ranks is preferred. When no swap can be found that
    /// resolves a conflict without introducing a new one elsewhere, the conflict is left in place.
    /// </summary>
    /// <param name="groups">The quizzer identifier groups, one per team.</param>
    /// <param name="quizzers">The full quizzer map, used to look up church identifiers.</param>
    /// <param name="rankedQuizzerIds">The original ranking order, used to minimize skill disruption when swapping.</param>
    private static void BalanceChurches(List<List<int>> groups, IDictionary<int, Quizzer> quizzers, IReadOnlyList<int> rankedQuizzerIds)
    {
        var rankIndex = new Dictionary<int, int>();

        for (var i = 0; i < rankedQuizzerIds.Count; i++)
        {
            rankIndex[rankedQuizzerIds[i]] = i;
        }

        var totalQuizzers = groups.Sum(x => x.Count);
        var maxIterations = totalQuizzers * Math.Max(1, groups.Count);
        var changed = true;
        var iterations = 0;

        while (changed && iterations++ < maxIterations)
        {
            changed = false;

            foreach (var group in groups)
            {
                if (TryResolveDuplicateChurch(groups, group, quizzers, rankIndex))
                {
                    changed = true;
                }
            }
        }
    }

    /// <summary>
    /// Attempts to resolve one duplicated church in the given group by swapping a quizzer from
    /// that church with a compatible quizzer from another group. Every duplicated church in the
    /// group is tried, in order, until a resolvable one is found, so that a single unresolvable
    /// conflict does not prevent other, independently resolvable conflicts from being fixed.
    /// </summary>
    /// <param name="groups">All quizzer groups.</param>
    /// <param name="group">The group to examine.</param>
    /// <param name="quizzers">The full quizzer map.</param>
    /// <param name="rankIndex">The original rank of each quizzer, used to minimize skill disruption.</param>
    /// <returns><see langword="true"/> when a swap was applied; otherwise <see langword="false"/>.</returns>
    private static bool TryResolveDuplicateChurch(
        List<List<int>> groups,
        List<int> group,
        IDictionary<int, Quizzer> quizzers,
        IDictionary<int, int> rankIndex)
    {
        foreach (var duplicateChurchId in FindDuplicateChurches(group, quizzers))
        {
            foreach (var quizzerToMove in group.Where(id => quizzers[id].ChurchId == duplicateChurchId).ToArray())
            {
                var swapTarget = FindSwapTarget(groups, group, quizzerToMove, quizzers, rankIndex);

                if (swapTarget is null)
                {
                    continue;
                }

                var (otherGroup, otherQuizzerId) = swapTarget.Value;

                group.Remove(quizzerToMove);
                otherGroup.Remove(otherQuizzerId);
                group.Add(otherQuizzerId);
                otherGroup.Add(quizzerToMove);

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Finds every church identifier that appears more than once in the given group, ordered by
    /// the number of duplicates, then by church identifier, for deterministic processing.
    /// </summary>
    /// <param name="group">The quizzer identifiers in the group.</param>
    /// <param name="quizzers">The full quizzer map.</param>
    /// <returns>The duplicated church identifiers.</returns>
    private static int[] FindDuplicateChurches(List<int> group, IDictionary<int, Quizzer> quizzers)
    {
        return group
            .GroupBy(id => quizzers[id].ChurchId)
            .Where(g => g.Count() > 1)
            .OrderByDescending(g => g.Count())
            .ThenBy(g => g.Key)
            .Select(g => g.Key)
            .ToArray();
    }

    /// <summary>
    /// Finds the other group and quizzer that can be swapped with the given quizzer without
    /// introducing a new same-church conflict in either group. When more than one valid swap
    /// exists, the one exchanging quizzers with the closest original rank is preferred, to
    /// minimize disruption to the skill balance established by the snake draft.
    /// </summary>
    /// <param name="groups">All quizzer groups.</param>
    /// <param name="sourceGroup">The group containing the quizzer to move.</param>
    /// <param name="quizzerToMove">The identifier of the quizzer to move out of <paramref name="sourceGroup"/>.</param>
    /// <param name="quizzers">The full quizzer map.</param>
    /// <param name="rankIndex">The original rank of each quizzer.</param>
    /// <returns>The target group and quizzer identifier to swap with, or <see langword="null"/> when no valid swap exists.</returns>
    private static (List<int> Group, int QuizzerId)? FindSwapTarget(
        List<List<int>> groups,
        List<int> sourceGroup,
        int quizzerToMove,
        IDictionary<int, Quizzer> quizzers,
        IDictionary<int, int> rankIndex)
    {
        var movingChurchId = quizzers[quizzerToMove].ChurchId;
        var remainingChurchIds = new HashSet<int>(sourceGroup.Where(id => id != quizzerToMove).Select(id => quizzers[id].ChurchId));
        var movingRank = rankIndex[quizzerToMove];

        (List<int> Group, int QuizzerId)? best = null;
        var bestDistance = int.MaxValue;

        foreach (var otherGroup in groups)
        {
            // Moving quizzerToMove into otherGroup must not create a new duplicate there.
            if (ReferenceEquals(otherGroup, sourceGroup) || otherGroup.Any(id => quizzers[id].ChurchId == movingChurchId))
            {
                continue;
            }

            foreach (var candidateId in otherGroup)
            {
                // Moving candidateId into sourceGroup must not create a new duplicate there.
                if (remainingChurchIds.Contains(quizzers[candidateId].ChurchId))
                {
                    continue;
                }

                var distance = Math.Abs(rankIndex[candidateId] - movingRank);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = (otherGroup, candidateId);
                }
            }
        }

        return best;
    }

    /// <summary>
    /// Builds the final quizzer map, reassigning each quizzer to the team identifier of the group
    /// they were placed in.
    /// </summary>
    /// <param name="groups">The quizzer identifier groups, one per team, indexed from 0.</param>
    /// <param name="quizzers">The full quizzer map.</param>
    /// <returns>The reassigned quizzer map.</returns>
    private static Dictionary<int, Quizzer> CreateQuizzers(List<List<int>> groups, IDictionary<int, Quizzer> quizzers)
    {
        var result = new Dictionary<int, Quizzer>();

        for (var teamIndex = 0; teamIndex < groups.Count; teamIndex++)
        {
            var teamId = teamIndex + 1;

            foreach (var quizzerId in groups[teamIndex])
            {
                var quizzer = quizzers[quizzerId];
                result.Add(
                    quizzerId,
                    new Quizzer(quizzer.Id, quizzer.FirstName, quizzer.LastName, quizzer.Gender, quizzer.RookieYear, teamId, quizzer.ChurchId));
            }
        }

        return result;
    }
}
