namespace MatchMaker.Reporting.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Ardalis.GuardClauses;

using MatchMaker.Models;
using MatchMaker.Reporting.Policies;

/// <summary>
/// Defines the <see cref="TeamSummary" />
/// </summary>
[DebuggerDisplay("Team Summary (Team {TeamId}, {Wins}-{Losses}, Rounds {TotalRounds}, Score {TotalScore}, Errors {TotalErrors}, Place {Place})")]
public class TeamSummary
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
    /// Gets or sets the Losses
    /// </summary>
    public int Losses
    {
        get; set;
    }

    /// <summary>
    /// Gets or sets the Place
    /// </summary>
    public int Place { get; set; } = 1;

    /// <summary>
    /// Gets or sets the team identifier
    /// </summary>
    public int TeamId
    {
        get; set;
    }

    /// <summary>
    /// Gets or sets the tie breaker
    /// </summary>
    public TieBreak TieBreak { get; set; } = TieBreak.None;

    /// <summary>
    /// Gets or sets the total errors
    /// </summary>
    public int TotalErrors
    {
        get; set;
    }

    /// <summary>
    /// Gets the total rounds
    /// </summary>
    public int TotalRounds => this.Wins + this.Losses;

    /// <summary>
    /// Gets or sets the total score
    /// </summary>
    public int TotalScore
    {
        get; set;
    }

    /// <summary>
    /// Gets the win percentage
    /// </summary>
    public decimal WinPercentage => Convert.ToDecimal(this.Wins) / Convert.ToDecimal(this.TotalRounds);

    /// <summary>
    /// Gets or sets the Wins
    /// </summary>
    public int Wins
    {
        get; set;
    }

    /// <summary>
    /// Creates a collection of placed <see cref="TeamSummary"/> instances from a <see cref="Result"/>
    /// </summary>
    /// <param name="result">The result<see cref="Result"/></param>
    /// <param name="policies">The <see cref="TeamRankingPolicy"/> instances</param>
    /// <returns>The <see cref="IDictionary{int, TeamSummary}"/></returns>
    public static IDictionary<int, TeamSummary> FromResult(Result result, IEnumerable<TeamRankingPolicy> policies)
    {
        Guard.Against.Null(policies);
        Guard.Against.NullOrEmpty(policies);

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

    /// <summary>
    /// Gets all team results.
    /// </summary>
    /// <param name="result">The <see cref="Result"/></param>
    /// <returns>The <see cref="IEnumerable{TeamResult}"/></returns>
    private static IEnumerable<TeamResult> GetAllTeamResults(Result result)
    {
        Guard.Against.Null(result);

        return result.Matches.SelectMany(m => m.Value.TeamResults);
    }

    /// <summary>
    /// Aggregates a collection of <see cref="TeamSummary"/> instances into a single <see cref="TeamSummary"/> instance
    /// </summary>
    /// <param name="summaries">The <see cref="IEnumerable{TeamSummary}"/></param>
    /// <returns>The <see cref="TeamSummary"/></returns>
    private static TeamSummary AggregateTeamSummary(IEnumerable<TeamSummary> summaries)
    {
        return summaries.Aggregate(TeamSummaryAccumulator);
    }

    /// <summary>
    /// Creates a <see cref="TeamSummary"/> from a <see cref="TeamResult"/>
    /// </summary>
    /// <param name="result">The <see cref="TeamResult"/></param>
    /// <returns>The <see cref="TeamSummary"/></returns>
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

    /// <summary>
    /// Gets all team summaries
    /// </summary>
    /// <param name="result">The <see cref="Result"/></param>
    /// <returns>The <see cref="IEnumerable{TeamSummary}"/></returns>
    private static IEnumerable<TeamSummary> GetAllTeamSummaries(Result result)
    {
        return GetAllTeamResults(result).Select(x => FromTeamResult(x));
    }

    /// <summary>
    /// Accumulates two <see cref="TeamSummary"/> instances
    /// </summary>
    /// <param name="team1">The <see cref="TeamSummary"/></param>
    /// <param name="team2">The <see cref="TeamSummary"/></param>
    /// <returns>The <see cref="TeamSummary"/></returns>
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
