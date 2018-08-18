using System;
using System.Collections.Generic;
using System.Linq;

namespace MatchMaker.Reporting
{
    public abstract class TeamRankingPolicy
    {
        protected Result Result
        {
            get; set;
        }

        public void Rank(IEnumerable<TeamSummary> summaries, Result result)
        {
            this.Result = result;
            var groups = summaries.GroupBy(s => s.Place);

            foreach (var group in groups)
            {
                if (group.Count() > 1)
                {
                    this.RankGroup(group, group.Min(g => g.Place));
                }
            }
        }

        public void SetRelativePlaces(List<TeamSummary> summaries, int initial, Func<TeamSummary, TeamSummary, bool> areEqual, TieBreak tieBreak)
        {
            for (var i = 1; i < summaries.Count; i++)
            {
                if (areEqual(summaries[i], summaries[i - 1]))
                {
                    summaries[i].Place = summaries[i - 1].Place;
                }
                else
                {
                    summaries[i - 1].TieBreak = tieBreak;
                    summaries[i].TieBreak = tieBreak;
                    summaries[i].Place = initial + i;
                }
            }
        }

        protected abstract void RankGroup(IEnumerable<TeamSummary> summaries, int initial);
    }
}