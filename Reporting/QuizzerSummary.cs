using System;
using System.Collections.Generic;
using System.Linq;

namespace MatchMaker.Reporting
{
    public class QuizzerSummary
    {
        public QuizzerSummary()
        {
            this.Place = 1;
            this.TotalRounds = 1;
        }

        public decimal AverageErrors
        {
            get { return Convert.ToDecimal(this.TotalErrors) / Convert.ToDecimal(this.TotalRounds); }
        }

        public decimal AverageScore
        {
            get { return Convert.ToDecimal(this.TotalScore) / Convert.ToDecimal(this.TotalRounds); }
        }

        public int Place { get; set; }
        public int QuizzerId { get; set; }
        public int TotalErrors { get; set; }
        public int TotalRounds { get; set; }
        public int TotalScore { get; set; }

        public static IDictionary<int, QuizzerSummary> FromResult(Result result)
        {
            var summaries = GetAllQuizzerSummaries(result)
                .GroupBy(s => s.QuizzerId)
                .Select(s => AggregateQuizzerSummary(s))
                .ToDictionary(s => s.QuizzerId, s => s);

            new ScoreQuizzerRankingPolicy().Rank(summaries.Values);
            new ErrorQuizzerRankingPolicy().Rank(summaries.Values);

            return summaries;
        }

        private static QuizzerSummary AggregateQuizzerSummary(IEnumerable<QuizzerSummary> summaries)
        {
            return summaries.Aggregate(QuizzerSummaryAccumulator);
        }

        private static QuizzerSummary FromQuizzerResult(QuizzerResult result)
        {
            return new QuizzerSummary
            {
                QuizzerId = result.QuizzerId,
                TotalScore = result.Score,
                TotalErrors = result.Errors
            };
        }

        private static IEnumerable<QuizzerResult> GetAllQuizzerResults(Result result)
        {
            return result.Matches.SelectMany(m => m.Value.QuizzerResults ?? new QuizzerResult[0]);
        }

        private static IEnumerable<QuizzerSummary> GetAllQuizzerSummaries(Result result)
        {
            return GetAllQuizzerResults(result).Select(FromQuizzerResult);
        }

        private static QuizzerSummary QuizzerSummaryAccumulator(QuizzerSummary s1, QuizzerSummary s2)
        {
            return new QuizzerSummary
            {
                QuizzerId = s1.QuizzerId,
                TotalRounds = s1.TotalRounds + s2.TotalRounds,
                TotalScore = s1.TotalScore + s2.TotalScore,
                TotalErrors = s1.TotalErrors + s2.TotalErrors
            };
        }
    }
}