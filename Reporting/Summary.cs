using System.Collections.Generic;

namespace MatchMaker.Reporting
{
    public class Summary
    {
        public string Name
        {
            get { return this.Result?.Name ?? string.Empty; }
        }

        public IDictionary<int, QuizzerSummary> QuizzerSummaries { get; set; }
        public Result Result { get; set; }
        public IDictionary<int, TeamSummary> TeamSummaries { get; set; }

        public static Summary FromResult(Result result, TeamRankingPolicy[] policies)
        {
            return new Summary
            {
                Result = result,
                TeamSummaries = TeamSummary.FromResult(result, policies),
                QuizzerSummaries = QuizzerSummary.FromResult(result)
            };
        }
    }
}