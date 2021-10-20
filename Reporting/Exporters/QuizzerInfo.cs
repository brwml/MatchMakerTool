namespace MatchMaker.Reporting.Exporters;

using System;

using Ardalis.GuardClauses;

using MatchMaker.Reporting.Models;

/// <summary>
/// Contains all quizzer information.
/// </summary>
public class QuizzerInfo
{
    /// <summary>
    /// Initializes an instance of the <see cref="QuizzerInfo"/> class.
    /// </summary>
    /// <param name="quizzer">The quizzer</param>
    /// <param name="summary">The quizzer summary</param>
    /// <param name="church">The church</param>
    /// <param name="team">The team</param>
    public QuizzerInfo(Quizzer quizzer, QuizzerSummary summary, Church church, Team team)
    {
        Guard.Against.Null(quizzer, nameof(quizzer));
        Guard.Against.Null(summary, nameof(summary));
        Guard.Against.Null(church, nameof(church));
        Guard.Against.Null(team, nameof(team));

        this.Id = quizzer.Id;
        this.FirstName = quizzer.FirstName;
        this.LastName = quizzer.LastName;
        this.RookieYear = quizzer.RookieYear;
        this.Church = church;
        this.Team = team;
        this.Place = summary.Place;
        this.TotalErrors = summary.TotalErrors;
        this.TotalRounds = summary.TotalRounds;
        this.TotalScore = summary.TotalScore;
        this.ShowPlace = true;
    }

    /// <summary>
    /// Gets the quizzer identifier
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets the first name
    /// </summary>
    public string FirstName { get; }

    /// <summary>
    /// Gets the last name
    /// </summary>
    public string LastName { get; }

    /// <summary>
    /// Gets the full name
    /// </summary>
    public string FullName => FormattableString.Invariant($"{this.FirstName} {this.LastName}");

    /// <summary>
    /// Gets the rookie year
    /// </summary>
    public int RookieYear { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the quizzing is a rookie.
    /// </summary>
    public bool IsRookie { get; set; }

    /// <summary>
    /// Gets the church
    /// </summary>
    public Church Church { get; }

    /// <summary>
    /// Gets the team
    /// </summary>
    public Team Team { get; }

    /// <summary>
    /// Gets the place
    /// </summary>
    public int Place { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the place should be shown.
    /// </summary>
    public bool ShowPlace { get; set; }

    /// <summary>
    /// Gets the total errors.
    /// </summary>
    public int TotalErrors { get; }

    /// <summary>
    /// Gets the total rounds.
    /// </summary>
    public int TotalRounds { get; }

    /// <summary>
    /// Gets the total score.
    /// </summary>
    public int TotalScore { get; }

    /// <summary>
    /// Gets the average errors
    /// </summary>
    public decimal AverageErrors => Convert.ToDecimal(this.TotalErrors) / Convert.ToDecimal(this.TotalRounds);

    /// <summary>
    /// Gets the average score
    /// </summary>
    public decimal AverageScore => Convert.ToDecimal(this.TotalScore) / Convert.ToDecimal(this.TotalRounds);
}
