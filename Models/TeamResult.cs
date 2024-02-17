namespace MatchMaker.Models;

using System.Diagnostics;
using System.Xml.Linq;

/// <summary>
/// Defines the <see cref="TeamResult" />
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TeamResult"/> class.
/// </remarks>
/// <param name="id">The team identifier</param>
/// <param name="score">The team score</param>
/// <param name="errors">The team score</param>
/// <param name="place">The team place</param>
[DebuggerDisplay("Team Result (Team {TeamId}, Score {Score}, Errors {Errors}, Place {Place})")]
public class TeamResult(int id, int score, int errors, int place = 1)
{
    /// <summary>
    /// Gets or sets the Errors
    /// </summary>
    public int Errors { get; } = errors;

    /// <summary>
    /// Gets or sets the Place
    /// </summary>
    public int Place { get; } = place;

    /// <summary>
    /// Gets or sets the Score
    /// </summary>
    public int Score { get; } = score;

    /// <summary>
    /// Gets or sets the team identifier
    /// </summary>
    public int TeamId { get; } = id;

    /// <summary>
    /// Creates a <see cref="TeamResult"/> from an <see cref="XElement"/>
    /// </summary>
    /// <param name="xml">The <see cref="XElement"/></param>
    /// <returns>The <see cref="TeamResult"/></returns>
    public static TeamResult FromXml(XElement xml)
    {
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
