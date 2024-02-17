namespace MatchMaker.Models;

using System.Diagnostics;
using System.Xml.Linq;

/// <summary>
/// Defines the <see cref="Team" />
/// </summary>
/// <remarks>
/// Initializes an instance of the <see cref="Team"/> class.
/// </remarks>
/// <param name="id">The team identifier</param>
/// <param name="name">The team name</param>
/// <param name="abbreviation">The team abbreviation</param>
/// <param name="division">The team division</param>
[DebuggerDisplay("Team {Name} ({Id})")]
public class Team(int id, string name, string abbreviation, int division)
{
    /// <summary>
    /// Gets or sets the Abbreviation
    /// </summary>
    public string Abbreviation { get; } = abbreviation;

    /// <summary>
    /// Gets or sets the Division
    /// </summary>
    public int Division { get; } = division;

    /// <summary>
    /// Gets or sets the Id
    /// </summary>
    public int Id { get; } = id;

    /// <summary>
    /// Gets or sets the Name
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Creates a <see cref="Team"/> from an <see cref="XElement"/>
    /// </summary>
    /// <param name="xml">The <see cref="XElement"/></param>
    /// <returns>The <see cref="Team"/></returns>
    public static Team FromXml(XElement xml)
    {
        var id = xml.GetAttribute<int>("id");
        var division = xml.GetAttribute<int>("div");
        var abbreviation = xml.GetAttribute<string>("abbrev");
        var name = xml.Value;

        return new Team(id, name, abbreviation, division);
    }

    /// <summary>
    /// Converts the <see cref="Team"/> to XML.
    /// </summary>
    /// <returns>The <see cref="XElement"/> instance</returns>
    public XElement ToXml()
    {
        return new XElement(
            "team",
            new XAttribute("id", this.Id),
            new XAttribute("div", this.Division),
            new XAttribute("abbrev", this.Abbreviation),
            new XText(this.Name));
    }
}
