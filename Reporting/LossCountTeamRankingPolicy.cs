using System.Collections.Generic;
using System.Linq;

namespace MatchMaker.Reporting
{
    public class LossCountTeamRankingPolicy : TeamRankingPolicy
    {
        protected override void RankGroup(IEnumerable<TeamSummary> summaries, int initial)
        {
            var list = summaries.ToList();
            list.Sort(Compare);

            this.SetRelativePlaces(list, initial, (s1, s2) => s1.Losses == s2.Losses, TieBreak.None);
        }

        private static int Compare(TeamSummary x, TeamSummary y)
        {
            if (x.Losses < y.Losses)
            {
                return -1;
            }
            else if (x.Losses > y.Losses)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}