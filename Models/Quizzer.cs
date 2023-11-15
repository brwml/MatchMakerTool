namespace MatchMaker.Models;

using System.Diagnostics;
using System.Xml.Linq;

using Ardalis.GuardClauses;

/// <summary>
/// Defines the <see cref="Quizzer" />
/// </summary>
/// <remarks>
/// Initializes an instance of the <see cref="Quizzer"/> class.
/// </remarks>
/// <param name="id">The identifier</param>
/// <param name="firstName">The first name</param>
/// <param name="lastName">The last name</param>
/// <param name="gender">The gender</param>
/// <param name="rookieYear">The rookie year</param>
/// <param name="teamId">The team identifier</param>
/// <param name="churchId">The church identifier</param>
[DebuggerDisplay("Quizzer {FirstName} {LastName} ({Id}, Church {ChurchId}, Team {TeamId})")]
public class Quizzer(int id, string firstName, string lastName, Gender gender, int rookieYear, int teamId, int churchId)
{
    /// <summary>
    /// Gets or sets the church identifier
    /// </summary>
    public int ChurchId { get; } = churchId;

    /// <summary>
    /// Gets or sets the first name
    /// </summary>
    public string FirstName { get; } = firstName;

    /// <summary>
    /// Gets or sets the Gender
    /// </summary>
    public Gender Gender { get; } = gender;

    /// <summary>
    /// Gets or sets the Id
    /// </summary>
    public int Id { get; } = id;

    /// <summary>
    /// Gets or sets the last name
    /// </summary>
    public string LastName { get; } = lastName;

    /// <summary>
    /// Gets or sets the rookie year
    /// </summary>
    public int RookieYear { get; } = rookieYear;

    /// <summary>
    /// Gets or sets the team identifier
    /// </summary>
    public int TeamId { get; } = teamId;

    /// <summary>
    /// Creates a new <see cref="Quizzer"/> instance from an XML element.
    /// </summary>
    /// <param name="xml">The xml<see cref="XElement"/> instance</param>
    /// <returns>The <see cref="Quizzer"/> instance</returns>
    public static Quizzer FromXml(XElement xml)
    {
        Guard.Against.Null(xml);

        var id = xml.GetAttribute<int>("id");
        var teamId = xml.GetElement<int>("teamID");
        var churchId = xml.GetElement<int>("churchID");
        var firstName = xml.GetElement<string>("firstname").Trim();
        var lastName = xml.GetElement<string>("lastname").Trim();
        var gender = xml.GetElement<string>("gender").Equals("M", StringComparison.OrdinalIgnoreCase) ? Gender.Male : Gender.Female;
        var rookieYear = xml.GetElement<int>("rookieYear");

        return new Quizzer(id, firstName, lastName, gender, rookieYear, teamId, churchId);
    }

    /// <summary>
    /// Converts the <see cref="Quizzer"/> instance to XML.
    /// </summary>
    /// <returns>The <see cref="XElement"/> instance</returns>
    public XElement ToXml()
    {
        var gender = this.Gender switch
        {
            Gender.Male => "M",
            Gender.Female => "F",
            _ => string.Empty
        };

        return new XElement(
            "quizzer",
            new XAttribute("id", this.Id),
            new XElement("teamID", this.TeamId),
            new XElement("churchID", this.ChurchId),
            new XElement("firstname", this.FirstName),
            new XElement("lastname", this.LastName),
            new XElement("gender", gender),
            new XElement("rookieYear", this.RookieYear));
    }
}
