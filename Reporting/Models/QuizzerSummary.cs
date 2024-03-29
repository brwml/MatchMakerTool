﻿namespace MatchMaker.Reporting.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using MatchMaker.Models;
using MatchMaker.Reporting.Policies;

/// <summary>
/// Defines the <see cref="QuizzerSummary" />
/// </summary>
/// <remarks>
/// TODO: Make the properties read-only.
/// </remarks>
[DebuggerDisplay("Quizzer Summary (Quizzer {QuizzerId}, Rounds {TotalRounds}, Score {TotalScore}, Errors {TotalErrors}, Place {Place})")]
public class QuizzerSummary
{
    /// <summary>
    /// Gets the average errors
    /// </summary>
    public decimal AverageErrors => Convert.ToDecimal(this.TotalErrors) / Convert.ToDecimal(this.TotalRounds);

    /// <summary>
    /// Gets the average score
    /// </summary>
    public decimal AverageScore => Convert.ToDecimal(this.TotalScore) / Convert.ToDecimal(this.TotalRounds);

    /// <summary>
    /// Gets or sets the Place
    /// </summary>
    public int Place { get; set; } = 1;

    /// <summary>
    /// Gets or sets the quizzer identifier
    /// </summary>
    public int QuizzerId
    {
        get; set;
    }

    /// <summary>
    /// Gets or sets the total errors
    /// </summary>
    public int TotalErrors
    {
        get; set;
    }

    /// <summary>
    /// Gets or sets the total rounds
    /// </summary>
    public int TotalRounds { get; set; } = 1;

    /// <summary>
    /// Gets or sets the total score
    /// </summary>
    public int TotalScore
    {
        get; set;
    }

    /// <summary>
    /// Converts a <see cref="Result"/> instance to a corresponding collection of placed <see cref="QuizzerSummary"/> instances.
    /// </summary>
    /// <param name="result">The <see cref="Result"/> instance</param>
    /// <returns>The <see cref="IDictionary{int, QuizzerSummary}"/> instance</returns>
    public static IDictionary<int, QuizzerSummary> FromResult(Result result)
    {
        var summaries = GetAllQuizzerSummaries(result)
            .GroupBy(s => s.QuizzerId)
            .Select(AggregateQuizzerSummary)
            .ToDictionary(s => s.QuizzerId, s => s);

        new ScoreQuizzerRankingPolicy().Rank(summaries.Values);
        new ErrorQuizzerRankingPolicy().Rank(summaries.Values);

        return summaries;
    }

    /// <summary>
    /// Aggregates a collection of <see cref="QuizzerSummary"/> instances into a single instance.
    /// </summary>
    /// <param name="summaries">The <see cref="QuizzerSummary"/> instances</param>
    /// <returns>The single <see cref="QuizzerSummary"/> instance</returns>
    private static QuizzerSummary AggregateQuizzerSummary(IEnumerable<QuizzerSummary> summaries)
    {
        return summaries.Aggregate(QuizzerSummaryAccumulator);
    }

    /// <summary>
    /// Creates a <see cref="QuizzerSummary"/> from a <see cref="QuizzerResult"/>
    /// </summary>
    /// <param name="result">The <see cref="QuizzerResult"/> instance</param>
    /// <returns>The <see cref="QuizzerSummary"/> instance</returns>
    private static QuizzerSummary FromQuizzerResult(QuizzerResult result)
    {
        return new QuizzerSummary
        {
            QuizzerId = result.QuizzerId,
            TotalScore = result.Score,
            TotalErrors = result.Errors
        };
    }

    /// <summary>
    /// Gets all quizzer results.
    /// </summary>
    /// <param name="result">The <see cref="Result"/> instance</param>
    /// <returns>The <see cref="QuizzerResult"/> instances</returns>
    private static IEnumerable<QuizzerResult> GetAllQuizzerResults(Result result)
    {
        return result.Matches.SelectMany(m => m.Value.QuizzerResults);
    }

    /// <summary>
    /// Gets all quizzer summaries
    /// </summary>
    /// <param name="result">The <see cref="Result"/> instance</param>
    /// <returns>The <see cref="QuizzerSummary"/> instances</returns>
    private static IEnumerable<QuizzerSummary> GetAllQuizzerSummaries(Result result)
    {
        return GetAllQuizzerResults(result).Select(FromQuizzerResult);
    }

    /// <summary>
    /// Combines two successive <see cref="QuizzerSummary"/> instances
    /// </summary>
    /// <param name="s1">The aggregate <see cref="QuizzerSummary"/> instance</param>
    /// <param name="s2">The new <see cref="QuizzerSummary"/> instance</param>
    /// <returns>The new aggregate <see cref="QuizzerSummary"/> instance</returns>
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
