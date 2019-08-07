namespace MatchMaker.Reporting.Policies
{
    using MatchMaker.Reporting.Models;

    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="ScoreQuizzerRankingPolicy" />
    /// </summary>
    public class ScoreQuizzerRankingPolicy : QuizzerRankingPolicy
    {
        /// <summary>
        /// Ranks the quizzers by average score
        /// </summary>
        /// <param name="summaries">The <see cref="IEnumerable{QuizzerSummary}"/></param>
        /// <param name="initial">The initial placement</param>
        protected override void RankGroup(IEnumerable<QuizzerSummary> summaries, int initial)
        {
            var list = summaries.OrderByDescending(s => s.AverageScore).ToList();
            SetRelativePlaces(list, initial, (s1, s2) => s1.AverageScore == s2.AverageScore);
        }
    }
}
