namespace MatchMaker.Reporting.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines the <see cref="TieBreakHeadToHead" />
    /// </summary>
    [DataContract]
    public class TieBreakHeadToHead : TieBreak
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TieBreakHeadToHead"/> class.
        /// </summary>
        /// <param name="results">The <see cref="IEnumerable{MatchResult}"/></param>
        /// <param name="teams">The <see cref="IDictionary{int, Team}"/></param>
        public TieBreakHeadToHead(IEnumerable<MatchResult> results, IDictionary<int, Team> teams)
        {
            this.Reason = TieBreakReason.HeadToHead;
            this.Results = results;
            this.Teams = teams;
        }

        /// <summary>
        /// Gets the Teams
        /// </summary>
        [DataMember]
        public IDictionary<int, Team> Teams { get; }

        /// <summary>
        /// Gets or sets the Results
        /// </summary>
        [DataMember]
        private IEnumerable<MatchResult> Results { get; set; }

        /// <summary>
        /// Creates a <see cref="string"/> describing the head-to-head matches that produced the tie breaker.
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public override string ToString()
        {
            return $"{base.ToString()} ({string.Join(", ", this.Results.Select(x => $"{this.GetWinner(x)}->{this.GetLoser(x)}"))})";
        }

        /// <summary>
        /// Gets the loser
        /// </summary>
        /// <param name="result">The <see cref="MatchResult"/></param>
        /// <returns>The <see cref="string"/></returns>
        private string GetLoser(MatchResult result)
        {
            return this.Teams[result.TeamResults.First(x => x.Place == 2).TeamId].Abbreviation;
        }

        /// <summary>
        /// Gets the winner
        /// </summary>
        /// <param name="result">The <see cref="MatchResult"/></param>
        /// <returns>The <see cref="string"/></returns>
        private string GetWinner(MatchResult result)
        {
            return this.Teams[result.TeamResults.First(x => x.Place == 1).TeamId].Abbreviation;
        }
    }
}
