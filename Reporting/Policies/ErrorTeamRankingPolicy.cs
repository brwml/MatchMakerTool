namespace MatchMaker.Reporting.Policies;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using MatchMaker.Models;
using MatchMaker.Reporting.Models;

/// <summary>
/// Defines the <see cref="ErrorTeamRankingPolicy" />
/// </summary>
public class ErrorTeamRankingPolicy : TeamRankingPolicy
{
    /// <summary>
    /// Ranks the collection of <see cref="Team"/> summary instances
    /// </summary>
    /// <param name="summaries">The summaries</param>
    /// <param name="initial">The initial place</param>
    protected override void RankGroup(IEnumerable<TeamSummary> summaries, int initial)
    {
        Trace.WriteLine("Ranking teams by average errors");
        var list = summaries.OrderBy(s => s.AverageErrors).ToList();
        SetRelativePlaces(list, initial, (s1, s2) => s1.AverageErrors == s2.AverageErrors, new TieBreak(TieBreakReason.AverageErrors));
    }
}
