using System.Collections.Generic;
using System.Linq;

namespace MatchMaker.Reporting
{
    public class ErrorQuizzerRankingPolicy : QuizzerRankingPolicy
    {
        protected override void RankGroup(IEnumerable<QuizzerSummary> summaries, int initial)
        {
            var list = summaries.OrderBy(s => s.AverageErrors).ToList();
            this.SetRelativePlaces(list, initial, (s1, s2) => s1.AverageErrors == s2.AverageErrors);
        }
    }
}