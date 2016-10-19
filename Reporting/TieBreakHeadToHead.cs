using System.Collections.Generic;
using System.Linq;

namespace MatchMaker.Reporting
{
    public class TieBreakHeadToHead : TieBreak
    {
        public TieBreakHeadToHead(IEnumerable<MatchResult> results, IDictionary<int, Team> teams)
        {
            this.Reason = TieBreakReason.HeadToHead;
            this.Results = results;
            this.Teams = teams;
        }

        public IDictionary<int, Team> Teams { get; set; }
        private IEnumerable<MatchResult> Results { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()} ({string.Join(", ", this.Results.Select(x => $"{this.GetWinner(x)}->{this.GetLoser(x)}"))})";
        }

        private string GetLoser(MatchResult result)
        {
            return this.Teams[result.TeamResults.First(x => x.Place == 2).TeamId].Abbreviation;
        }

        private string GetWinner(MatchResult result)
        {
            return this.Teams[result.TeamResults.First(x => x.Place == 1).TeamId].Abbreviation;
        }
    }
}