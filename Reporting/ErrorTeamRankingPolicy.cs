using System.Collections.Generic;
using System.Linq;

namespace MatchMaker.Reporting
{
    public class ErrorTeamRankingPolicy : TeamRankingPolicy
    {
        protected override void RankGroup(IEnumerable<TeamSummary> summaries, int initial)
        {
            var list = summaries.OrderBy(s => s.AverageErrors).ToList();
            this.SetRelativePlaces(list, initial, (s1, s2) => s1.AverageErrors == s2.AverageErrors, new TieBreak { Reason = TieBreakReason.AverageErrors });
        }
    }
}