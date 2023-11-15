namespace MatchMaker.Reporting.Exporters;

using System;

using MatchMaker.Models;
using MatchMaker.Reporting.Models;

/// <summary>
/// The team information
/// </summary>
/// <remarks>
/// Initializes an instance of the <see cref="TeamInfo"/> class
/// </remarks>
/// <param name="team">The team</param>
/// <param name="summary">The team summary</param>
public class TeamInfo(Team team, TeamSummary summary)
{
    /// <summary>
    /// Gets or sets the Abbreviation
    /// </summary>
    public string Abbreviation { get; } = team.Abbreviation;

    /// <summary>
    /// Gets or sets the Id
    /// </summary>
    public int Id { get; } = team.Id;

    /// <summary>
    /// Gets or sets the Name
    /// </summary>
    public string Name { get; } = team.Name;

    /// <summary>
    /// Gets or sets the Losses
    /// </summary>
    public int Losses { get; } = summary.Losses;

    /// <summary>
    /// Gets or sets the Place
    /// </summary>
    public int Place { get; } = summary.Place;

    /// <summary>
    /// Gets or sets a value indicating whether to show the place.
    /// </summary>
    public bool ShowPlace { get; set; } = true;

    /// <summary>
    /// Gets or sets the tie breaker
    /// </summary>
    public TieBreak TieBreak { get; } = summary.TieBreak;

    /// <summary>
    /// Gets or sets the total errors
    /// </summary>
    public int TotalErrors { get; } = summary.TotalErrors;

    /// <summary>
    /// Gets the total rounds
    /// </summary>
    public int TotalRounds { get; } = summary.TotalRounds;

    /// <summary>
    /// Gets or sets the total score
    /// </summary>
    public int TotalScore { get; } = summary.TotalScore;

    /// <summary>
    /// Gets the win percentage
    /// </summary>
    public decimal WinPercentage => Convert.ToDecimal(this.Wins) / Convert.ToDecimal(this.TotalRounds);

    /// <summary>
    /// Gets or sets the Wins
    /// </summary>
    public int Wins { get; } = summary.Wins;

    /// <summary>
    /// Gets the average errors
    /// </summary>
    public decimal AverageErrors => Convert.ToDecimal(this.TotalErrors) / Convert.ToDecimal(this.TotalRounds);

    /// <summary>
    /// Gets the average score
    /// </summary>
    public decimal AverageScore => Convert.ToDecimal(this.TotalScore) / Convert.ToDecimal(this.TotalRounds);
}
