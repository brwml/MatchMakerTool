using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace MatchMaker.Reporting
{
    [DataContract]
    public class TeamSummary
    {
        public TeamSummary()
        {
            this.TieBreak = TieBreak.None;
            this.Place = 1;
        }

        [IgnoreDataMember]
        public decimal AverageErrors => Convert.ToDecimal(this.TotalErrors) / Convert.ToDecimal(this.TotalRounds);

        [IgnoreDataMember]
        public decimal AverageScore => Convert.ToDecimal(this.TotalScore) / Convert.ToDecimal(this.TotalRounds);

        [DataMember]
        public int Losses
        {
            get; set;
        }

        [DataMember]
        public int Place
        {
            get; set;
        }

        [DataMember]
        public int TeamId
        {
            get; set;
        }

        [DataMember]
        public TieBreak TieBreak
        {
            get; set;
        }

        [DataMember]
        public int TotalErrors
        {
            get; set;
        }

        [IgnoreDataMember]
        public int TotalRounds => this.Wins + this.Losses;

        [DataMember]
        public int TotalScore
        {
            get; set;
        }

        [IgnoreDataMember]
        public decimal WinPercentage => Convert.ToDecimal(this.Wins) / Convert.ToDecimal(this.TotalRounds);

        [DataMember]
        public int Wins
        {
            get; set;
        }

        public static IDictionary<int, TeamSummary> FromResult(Result result, TeamRankingPolicy[] policies)
        {
            var summaries = GetAllTeamSummaries(result)
                .GroupBy(s => s.TeamId)
                .Select(t => AggregateTeamSummary(t))
                .ToDictionary(kvp => kvp.TeamId, kvp => kvp);

            foreach (var policy in policies)
            {
                policy.Rank(summaries.Values, result);
            }

            return summaries;
        }

        public static IEnumerable<TeamResult> GetAllTeamResults(Result result)
        {
            return result.Matches.SelectMany(m => m.Value.TeamResults);
        }

        private static TeamSummary AggregateTeamSummary(IEnumerable<TeamSummary> summaries)
        {
            return summaries.Aggregate(TeamSummaryAccumulator);
        }

        private static TeamSummary FromTeamResult(TeamResult result)
        {
            return new TeamSummary
            {
                TeamId = result.TeamId,
                Wins = result.Place == 1 ? 1 : 0,
                Losses = result.Place == 2 ? 1 : 0,
                TotalScore = result.Score,
                TotalErrors = result.Errors
            };
        }

        private static IEnumerable<TeamSummary> GetAllTeamSummaries(Result result)
        {
            return GetAllTeamResults(result).Select(x => FromTeamResult(x));
        }

        private static TeamSummary TeamSummaryAccumulator(TeamSummary team1, TeamSummary team2)
        {
            return new TeamSummary
            {
                TeamId = team1.TeamId,
                Wins = team1.Wins + team2.Wins,
                Losses = team1.Losses + team2.Losses,
                TotalScore = team1.TotalScore + team2.TotalScore,
                TotalErrors = team1.TotalErrors + team2.TotalErrors
            };
        }
    }
}