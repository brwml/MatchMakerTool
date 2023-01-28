namespace MatchMaker.Tool.Gui.ViewModels;

using System;

using Ardalis.GuardClauses;

using MatchMaker.Reporting.Models;

/// <summary>
/// The quizzer view model
/// </summary>
internal class QuizzerViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuizzerViewModel"/> class.
    /// </summary>
    /// <param name="firstName">The first name of the quizzer</param>
    /// <param name="lastName">The last name of the quizzer</param>
    /// <param name="isMale">Indicates whether the quizzer is male</param>
    /// <param name="isFemale">Indicates whether the quizzer is female</param>
    /// <param name="firstYear">The first year of the quizzer</param>
    /// <param name="church">The name of the church</param>
    public QuizzerViewModel(string firstName, string lastName, bool isMale, bool isFemale, int firstYear, string church)
    {
        Guard.Against.NullOrEmpty(firstName);
        Guard.Against.NullOrEmpty(lastName);
        Guard.Against.NullOrEmpty(church);
        Guard.Against.NegativeOrZero(firstYear);
        Guard.Against.AgainstExpression(
            (isGender) => isGender,
            isMale ^ isFemale,
            FormattableString.Invariant($"Only one gender flag is to be set (male is {isMale}, female is {isFemale})."));

        this.FirstName = firstName;
        this.LastName = lastName;
        this.IsMale = isMale;
        this.IsFemale = isFemale;
        this.FirstYear = firstYear;
        this.Church = church;
    }

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
    public string FirstName
    {
        get;
    }

    /// <summary>
    /// Gets the last name of the quizzer
    /// </summary>
    public string LastName
    {
        get;
    }

    /// <summary>
    /// Gets a value indicating whether the quizzer is a male
    /// </summary>
    public bool IsMale
    {
        get;
    }

    /// <summary>
    /// Gets a value indicating whether the quizzer is a female
    /// </summary>
    public bool IsFemale
    {
        get;
    }

    /// <summary>
    /// Gets the first year of the quizzer
    /// </summary>
    public int FirstYear
    {
        get;
    }

    /// <summary>
    /// Gets the name of the church
    /// </summary>
    public string Church
    {
        get;
    }

    /// <summary>
    /// Converts the quizzer view model to a string.
    /// </summary>
    /// <returns>The string representation of the quizzer view model</returns>
    public override string ToString()
    {
        return FormattableString.Invariant($"{this.LastName}, {this.FirstName}");
    }
}
