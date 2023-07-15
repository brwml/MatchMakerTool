namespace MatchMaker.Models;

using System.Diagnostics;
using System.Xml.Linq;

using Ardalis.GuardClauses;

/// <summary>
/// Defines the <see cref="TeamResult" />
/// </summary>
[DebuggerDisplay("Team Result (Team {TeamId}, Score {Score}, Errors {Errors}, Place {Place})")]
public class TeamResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TeamResult"/> class.
    /// </summary>
    /// <param name="id">The team identifier</param>
    /// <param name="score">The team score</param>
    /// <param name="errors">The team score</param>
    /// <param name="place">The team place</param>
    public TeamResult(int id, int score, int errors, int place = 1)
    {
        this.TeamId = id;
        this.Score = score;
        this.Errors = errors;
        this.Place = place;
    }

    /// <summary>
    /// Gets or sets the Errors
    /// </summary>
    public int Errors
    {
        get;
    }

    /// <summary>
    /// Gets or sets the Place
    /// </summary>
    public int Place
    {
        get;
    }

    /// <summary>
    /// Gets or sets the Score
    /// </summary>
    public int Score
    {
        get;
    }

    /// <summary>
    /// Gets or sets the team identifier
    /// </summary>
    public int TeamId
    {
        get;
    }

    /// <summary>
    /// Creates a <see cref="TeamResult"/> from an <see cref="XElement"/>
    /// </summary>
    /// <param name="xml">The <see cref="XElement"/></param>
    /// <returns>The <see cref="TeamResult"/></returns>
    public static TeamResult FromXml(XElement xml)
    {
        Guard.Against.Null(xml);

        return new TeamResult(
            xml.GetAttribute<int>("id"),
            xml.GetAttribute<int>("score"),
            xml.GetAttribute<int>("errors"),
            xml.GetAttribute<int>("place"));
    }

    /// <summary>
    /// Converts the <see cref="TeamResult"/> instance to XML.
    /// </summary>
    /// <returns>The <see cref="XElement"/> instance</returns>
    public XElement ToXml()
    {
        return new XElement(
            "team",
            new XAttribute("id", this.TeamId),
            new XAttribute("score", this.Score),
            new XAttribute("errors", this.Errors),
            new XAttribute("place", this.Place));
    }
}
