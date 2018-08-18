using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MatchMaker.Reporting
{
    [DataContract]
    public class Summary
    {
        [IgnoreDataMember]
        public string Name => this.Result?.Name ?? string.Empty;

        [DataMember]
        public IDictionary<int, QuizzerSummary> QuizzerSummaries
        {
            get; set;
        }

        [DataMember]
        public Result Result
        {
            get; set;
        }

        [DataMember]
        public IDictionary<int, TeamSummary> TeamSummaries
        {
            get; set;
        }

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