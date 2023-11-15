namespace MatchMaker.Reporting.Exporters;

using System;

using MatchMaker.Models;
using MatchMaker.Reporting.Models;

/// <summary>
/// Contains all quizzer information.
/// </summary>
/// <remarks>
/// Initializes an instance of the <see cref="QuizzerInfo"/> class.
/// </remarks>
/// <param name="quizzer">The quizzer</param>
/// <param name="summary">The quizzer summary</param>
/// <param name="church">The church</param>
/// <param name="team">The team</param>
public class QuizzerInfo(Quizzer quizzer, QuizzerSummary summary, Church church, Team team)
{
    /// <summary>
    /// Gets the quizzer identifier
    /// </summary>
    public int Id { get; } = quizzer.Id;

    /// <summary>
    /// Gets the first name
    /// </summary>
    public string FirstName { get; } = quizzer.FirstName;

    /// <summary>
    /// Gets the last name
    /// </summary>
    public string LastName { get; } = quizzer.LastName;

    /// <summary>
    /// Gets the full name
    /// </summary>
    public string FullName => FormattableString.Invariant($"{this.FirstName} {this.LastName}");

    /// <summary>
    /// Gets the rookie year
    /// </summary>
    public int RookieYear { get; } = quizzer.RookieYear;

    /// <summary>
    /// Gets or sets a value indicating whether the quizzing is a rookie.
    /// </summary>
    public bool IsRookie
    {
        get; set;
    }

    /// <summary>
    /// Gets the church
    /// </summary>
    public Church Church { get; } = church;

    /// <summary>
    /// Gets the team
    /// </summary>
    public Team Team { get; } = team;

    /// <summary>
    /// Gets the place
    /// </summary>
    public int Place { get; } = summary.Place;

    /// <summary>
    /// Gets or sets a value indicating whether the place should be shown.
    /// </summary>
    public bool ShowPlace { get; set; } = true;

    /// <summary>
    /// Gets the total errors.
    /// </summary>
    public int TotalErrors { get; } = summary.TotalErrors;

    /// <summary>
    /// Gets the total rounds.
    /// </summary>
    public int TotalRounds { get; } = summary.TotalRounds;

    /// <summary>
    /// Gets the total score.
    /// </summary>
    public int TotalScore { get; } = summary.TotalScore;

    /// <summary>
    /// Gets the average errors
    /// </summary>
    public decimal AverageErrors => Convert.ToDecimal(this.TotalErrors) / Convert.ToDecimal(this.TotalRounds);

    /// <summary>
    /// Gets the average score
    /// </summary>
    public decimal AverageScore => Convert.ToDecimal(this.TotalScore) / Convert.ToDecimal(this.TotalRounds);
}
