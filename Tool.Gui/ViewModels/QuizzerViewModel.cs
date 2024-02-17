namespace MatchMaker.Tool.Gui.ViewModels;

using System;

using MatchMaker.Models;

/// <summary>
/// The quizzer view model
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="QuizzerViewModel"/> class.
/// </remarks>
/// <param name="firstName">The first name of the quizzer</param>
/// <param name="lastName">The last name of the quizzer</param>
/// <param name="isMale">Indicates whether the quizzer is male</param>
/// <param name="isFemale">Indicates whether the quizzer is female</param>
/// <param name="firstYear">The first year of the quizzer</param>
/// <param name="church">The name of the church</param>
internal class QuizzerViewModel(string firstName, string lastName, bool isMale, bool isFemale, int firstYear, string church)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuizzerViewModel"/> class.
    /// </summary>
    /// <param name="quizzer">The quizzer</param>
    /// <param name="schedule">The schedule</param>
    public QuizzerViewModel(Quizzer quizzer, Schedule schedule)
        : this(quizzer.FirstName,
               quizzer.LastName,
               quizzer.Gender == Gender.Male,
               quizzer.Gender == Gender.Female,
               quizzer.RookieYear,
               schedule.Churches[quizzer.ChurchId].Name)
    {
    }

    /// <summary>
    /// Gets the first name of the quizzer
    /// </summary>
    public string FirstName { get; } = firstName;

    /// <summary>
    /// Gets the last name of the quizzer
    /// </summary>
    public string LastName { get; } = lastName;

    /// <summary>
    /// Gets a value indicating whether the quizzer is a male
    /// </summary>
    public bool IsMale { get; } = isMale;

    /// <summary>
    /// Gets a value indicating whether the quizzer is a female
    /// </summary>
    public bool IsFemale { get; } = isFemale;

    /// <summary>
    /// Gets the first year of the quizzer
    /// </summary>
    public int FirstYear { get; } = firstYear;

    /// <summary>
    /// Gets the name of the church
    /// </summary>
    public string Church { get; } = church;

    /// <summary>
    /// Converts the quizzer view model to a string.
    /// </summary>
    /// <returns>The string representation of the quizzer view model</returns>
    public override string ToString()
    {
        return FormattableString.Invariant($"{this.LastName}, {this.FirstName}");
    }
}
