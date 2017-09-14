using System;
using System.Collections.Generic;
using System.Linq;

namespace MatchMaker.Reporting
{
    public abstract class QuizzerRankingPolicy
    {
        public void Rank(IEnumerable<QuizzerSummary> summaries)
        {
            var groups = summaries.GroupBy(s => s.Place);

            foreach (var group in groups)
            {
                this.RankGroup(group, group.Min(g => g.Place));
            }
        }

        public void SetRelativePlaces(List<QuizzerSummary> summaries, int initial, Func<QuizzerSummary, QuizzerSummary, bool> areEqual)
        {
            for (var i = 1; i < summaries.Count; i++)
            {
                if (areEqual(summaries[i], summaries[i - 1]))
                {
                    summaries[i].Place = summaries[i - 1].Place;
                }
                else
                {
                    summaries[i].Place = initial + i;
                }
            }
        }

        protected abstract void RankGroup(IEnumerable<QuizzerSummary> summaries, int initial);
    }
}