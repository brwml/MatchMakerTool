namespace MatchMaker.Reporting.Models;

using System.Runtime.Serialization;
using System.Xml.Linq;

using Ardalis.GuardClauses;

/// <summary>
/// Defines the <see cref="Quizzer" />
/// </summary>
[DataContract]
public class Quizzer
{
    /// <summary>
    /// Initializes an instance of the <see cref="Quizzer"/> class.
    /// </summary>
    /// <param name="id">The identifier</param>
    /// <param name="firstName">The first name</param>
    /// <param name="lastName">The last name</param>
    /// <param name="gender">The gender</param>
    /// <param name="rookieYear">The rookie year</param>
    /// <param name="teamId">The team identifier</param>
    /// <param name="churchId">The church identifier</param>
    public Quizzer(int id, string firstName, string lastName, Gender gender, int rookieYear, int teamId, int churchId)
    {
        this.Id = id;
        this.FirstName = firstName;
        this.LastName = lastName;
        this.Gender = gender;
        this.RookieYear = rookieYear;
        this.TeamId = teamId;
        this.ChurchId = churchId;
    }

    /// <summary>
    /// Gets or sets the church identifier
    /// </summary>
    [DataMember]
    public int ChurchId { get; set; }

    /// <summary>
    /// Gets or sets the first name
    /// </summary>
    [DataMember]
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the Gender
    /// </summary>
    [DataMember]
    public Gender Gender { get; set; }

    /// <summary>
    /// Gets or sets the Id
    /// </summary>
    [DataMember]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the last name
    /// </summary>
    [DataMember]
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the rookie year
    /// </summary>
    [DataMember]
    public int RookieYear { get; set; }

    /// <summary>
    /// Gets or sets the team identifier
    /// </summary>
    [DataMember]
    public int TeamId { get; set; }

    /// <summary>
    /// Creates a new <see cref="Quizzer"/> instance from an XML element.
    /// </summary>
    /// <param name="xml">The xml<see cref="XElement"/> instance</param>
    /// <returns>The <see cref="Quizzer"/> instance</returns>
    public static Quizzer FromXml(XElement xml)
    {
        Guard.Against.Null(xml, nameof(xml));

        var id = xml.GetAttribute<int>("id");
        var teamId = xml.GetElement<int>("teamID");
        var churchId = xml.GetElement<int>("churchID");
        var firstName = xml.Element("firstname")?.Value.Trim() ?? string.Empty;
        var lastName = xml.Element("lastname")?.Value.Trim() ?? string.Empty;
        var gender = xml.Element("gender")?.Value == "M" ? Gender.Male : Gender.Female;
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
