﻿namespace MatchMaker.Reporting.Policies;

using System.Collections.Generic;
using System.Linq;

using MatchMaker.Models;
using MatchMaker.Reporting.Models;

/// <summary>
/// Defines the <see cref="LossCountTeamRankingPolicy" />
/// </summary>
public class LossCountTeamRankingPolicy : TeamRankingPolicy
{
    /// <summary>
    /// Ranks the collection of <see cref="TeamSummary"/> instance
    /// </summary>
    /// <param name="summaries">The <see cref="IEnumerable{TeamSummary}"/> instances</param>
    /// <param name="initial">The initial placement</param>
    protected override void RankGroup(IEnumerable<TeamSummary> summaries, int initial)
    {
        var list = summaries.ToList();
        list.Sort(Compare);

        SetRelativePlaces(list, initial, (s1, s2) => s1.Losses == s2.Losses, TieBreak.None);
    }

    /// <summary>
    /// Compares two <see cref="TeamSummary"/> instances.
    /// </summary>
    /// <param name="x">The left <see cref="TeamSummary"/> instance</param>
    /// <param name="y">The right <see cref="TeamSummary"/> instance</param>
    /// <returns>The result of the comparison.</returns>
    private static int Compare(TeamSummary x, TeamSummary y)
    {
        return x.Losses < y.Losses
            ? -1
            : x.Losses > y.Losses
                ? 1
                : 0;
    }
}
