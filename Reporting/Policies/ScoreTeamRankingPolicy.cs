namespace MatchMaker.Reporting.Policies
{
    using MatchMaker.Reporting.Models;

    using System.Collections.Generic;
    using System.Linq;

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
            var list = summaries.OrderByDescending(s => s.AverageScore).ToList();
            SetRelativePlaces(list, initial, (s1, s2) => s1.AverageScore == s2.AverageScore, new TieBreak { Reason = TieBreakReason.AverageScore });
        }
    }
}
