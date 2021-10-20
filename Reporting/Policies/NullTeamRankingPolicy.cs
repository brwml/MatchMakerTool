namespace MatchMaker.Reporting.Policies;

using System.Collections.Generic;

using MatchMaker.Reporting.Models;

/// <summary>
/// Defines the <see cref="NullTeamRankingPolicy" />
/// </summary>
public class NullTeamRankingPolicy : TeamRankingPolicy
{
    /// <summary>
    /// Performs no ranking operation on the collection of <see cref="TeamSummary"/> instances.
    /// </summary>
    /// <param name="summaries">The <see cref="TeamSummary"/> instances</param>
    /// <param name="initial">The initial placement</param>
    protected override void RankGroup(IEnumerable<TeamSummary> summaries, int initial)
    {
    }
}
