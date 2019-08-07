namespace MatchMaker.Reporting.Policies
{
    using MatchMaker.Reporting.Models;

    using System.Collections.Generic;

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
}
