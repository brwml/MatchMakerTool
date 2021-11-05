namespace MatchMaker.Reporting.Models;

using System.Runtime.Serialization;
using System.Xml.Linq;

using Ardalis.GuardClauses;

/// <summary>
/// Defines the <see cref="Team" />
/// </summary>
[DataContract]
public class Team
{
    /// <summary>
    /// Initializes an instance of the <see cref="Team"/> class.
    /// </summary>
    /// <param name="id">The team identifier</param>
    /// <param name="name">The team name</param>
    /// <param name="abbreviation">The team abbreviation</param>
    /// <param name="division">The team division</param>
    public Team(int id, string name, string abbreviation, int division)
    {
        this.Id = id;
        this.Name = name;
        this.Abbreviation = abbreviation;
        this.Division = division;
    }

    /// <summary>
    /// Gets or sets the Abbreviation
    /// </summary>
    [DataMember]
    public string Abbreviation { get; set; }

    /// <summary>
    /// Gets or sets the Division
    /// </summary>
    [DataMember]
    public int Division { get; set; }

    /// <summary>
    /// Gets or sets the Id
    /// </summary>
    [DataMember]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Name
    /// </summary>
    [DataMember]
    public string Name { get; set; }

    /// <summary>
    /// Creates a <see cref="Team"/> from an <see cref="XElement"/>
    /// </summary>
    /// <param name="xml">The <see cref="XElement"/></param>
    /// <returns>The <see cref="Team"/></returns>
    public static Team FromXml(XElement xml)
    {
        Guard.Against.Null(xml, nameof(xml));

        var id = xml.GetAttribute<int>("id");
        var division = xml.GetAttribute<int>("div");
        var abbreviation = xml.Attribute("abbrev")?.Value ?? string.Empty;
        var name = xml.Value;

        return new Team(id, name, abbreviation, division);
    }
}
