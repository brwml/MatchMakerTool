namespace MatchMaker.Reporting.Models
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using MatchMaker.Reporting.Policies;

    /// <summary>
    /// Defines the <see cref="Summary" />
    /// </summary>
    [DataContract]
    public class Summary
    {
        /// <summary>
        /// Gets or sets the quizzer summaries
        /// </summary>
        [DataMember]
        public IDictionary<int, QuizzerSummary> QuizzerSummaries { get; set; }

        /// <summary>
        /// Gets or sets the Result
        /// </summary>
        [DataMember]
        public Result Result { get; set; }

        /// <summary>
        /// Gets or sets the team summaries
        /// </summary>
        [DataMember]
        public IDictionary<int, TeamSummary> TeamSummaries { get; set; }

        /// <summary>
        /// Gets the Name
        /// </summary>
        [IgnoreDataMember]
        public string Name => this.Result?.Name ?? string.Empty;

        /// <summary>
        /// Creates a <see cref="Summary"/> based on a <see cref="Result"/> and collection of ranking policies.
        /// </summary>
        /// <param name="result">The <see cref="Result"/></param>
        /// <param name="policies">The <see cref="TeamRankingPolicy"/> instances</param>
        /// <returns>The <see cref="Summary"/></returns>
        public static Summary FromResult(Result result, IEnumerable<TeamRankingPolicy> policies)
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
