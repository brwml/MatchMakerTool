﻿namespace MatchMaker.Reporting.Policies
{
    using System.Collections.Generic;
    using System.Linq;

    using MatchMaker.Reporting.Models;

    /// <summary>
    /// Defines the <see cref="WinPercentageTeamRankingPolicy" />
    /// </summary>
    public class WinPercentageTeamRankingPolicy : TeamRankingPolicy
    {
        /// <summary>
        /// Ranks a group of <see cref="TeamSummary"/> by winning percentage.
        /// </summary>
        /// <param name="summaries">The <see cref="IEnumerable{TeamSummary}"/></param>
        /// <param name="initial">The initial placement</param>
        protected override void RankGroup(IEnumerable<TeamSummary> summaries, int initial)
        {
            var list = summaries.ToList();
            list.Sort(Compare);

            SetRelativePlaces(list, initial, (s1, s2) => s1.WinPercentage == s2.WinPercentage, TieBreak.None);
        }

        /// <summary>
        /// Compares two <see cref="TeamSummary"/> instances
        /// </summary>
        /// <param name="x">The <see cref="TeamSummary"/></param>
        /// <param name="y">The <see cref="TeamSummary"/></param>
        /// <returns>The comparison result</returns>
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
