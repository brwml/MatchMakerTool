using System.Collections.Generic;
using System.Linq;

namespace MatchMaker.Reporting
{
    public class WinPercentageTeamRankingPolicy : TeamRankingPolicy
    {
        protected override void RankGroup(IEnumerable<TeamSummary> summaries, int initial)
        {
            var list = summaries.ToList();
            list.Sort(Compare);

            this.SetRelativePlaces(list, initial, (s1, s2) => s1.WinPercentage == s2.WinPercentage, TieBreak.None);
        }

        private static int Compare(TeamSummary x, TeamSummary y)
        {
            if (x.WinPercentage > y.WinPercentage)
            {
                return -1;
            }
            else if (x.WinPercentage < y.WinPercentage)
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