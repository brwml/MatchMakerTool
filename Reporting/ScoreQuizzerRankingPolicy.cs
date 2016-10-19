using System.Collections.Generic;
using System.Linq;

namespace MatchMaker.Reporting
{
    public class ScoreQuizzerRankingPolicy : QuizzerRankingPolicy
    {
        protected override void RankGroup(IEnumerable<QuizzerSummary> summaries, int initial)
        {
            var list = summaries.OrderByDescending(s => s.AverageScore).ToList();
            this.SetRelativePlaces(list, initial, (s1, s2) => s1.AverageScore == s2.AverageScore);
        }
    }
}