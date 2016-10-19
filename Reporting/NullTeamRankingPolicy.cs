using System.Collections.Generic;

namespace MatchMaker.Reporting
{
    public class NullTeamRankingPolicy : TeamRankingPolicy
    {
        protected override void RankGroup(IEnumerable<TeamSummary> summaries, int initial)
        {
        }
    }
}