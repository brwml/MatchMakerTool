using System.Collections.Generic;
using System.Linq;

namespace MatchMaker.Reporting
{
    public class ScoreTeamRankingPolicy : TeamRankingPolicy
    {
        protected override void RankGroup(IEnumerable<TeamSummary> summaries, int initial)
        {
            var list = summaries.OrderByDescending(s => s.AverageScore).ToList();
            this.SetRelativePlaces(list, initial, (s1, s2) => s1.AverageScore == s2.AverageScore, new TieBreak { Reason = TieBreakReason.AverageScore });
        }
    }
}