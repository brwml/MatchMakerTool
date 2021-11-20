namespace MatchMaker.Reporting.Policies;

using System.Collections.Generic;
using System.Linq;

using Ardalis.GuardClauses;

using MatchMaker.Reporting.Models;

/// <summary>
/// Defines the <see cref="ScoreTeamRankingPolicy" />
/// </summary>
public class ScoreTeamRankingPolicy : TeamRankingPolicy
{
    /// <summary>
    /// Ranks the teams by average score
    /// </summary>
    /// <param name="summaries">The <see cref="IEnumerable{TeamSummary}"/></param>
    /// <param name="initial">The initial placement</param>
    protected override void RankGroup(IEnumerable<TeamSummary> summaries, int initial)
    {
        Guard.Against.NullOrEmpty(summaries, nameof(summaries));
        Guard.Against.NegativeOrZero(initial, nameof(initial));

        var list = summaries.OrderByDescending(s => s.AverageScore).ToList();
        SetRelativePlaces(list, initial, (s1, s2) => s1.AverageScore == s2.AverageScore, new TieBreak(TieBreakReason.AverageScore));
    }
}
