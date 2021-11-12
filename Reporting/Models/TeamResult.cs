﻿namespace MatchMaker.Reporting.Models;

using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Linq;

using Ardalis.GuardClauses;

/// <summary>
/// Defines the <see cref="TeamResult" />
/// </summary>
[DataContract]
[DebuggerDisplay("Team Result (Team {TeamId}, Score {Score}, Errors {Errors}, Place {Place})")]
public class TeamResult
{
    /// <summary>
    /// Gets or sets the Errors
    /// </summary>
    [DataMember]
    public int Errors { get; set; }

    /// <summary>
    /// Gets or sets the Place
    /// </summary>
    [DataMember]
    public int Place { get; set; }

    /// <summary>
    /// Gets or sets the Score
    /// </summary>
    [DataMember]
    public int Score { get; set; }

    /// <summary>
    /// Gets or sets the team identifier
    /// </summary>
    [DataMember]
    public int TeamId { get; set; }

    /// <summary>
    /// Creates a <see cref="TeamResult"/> from an <see cref="XElement"/>
    /// </summary>
    /// <param name="xml">The <see cref="XElement"/></param>
    /// <returns>The <see cref="TeamResult"/></returns>
    public static TeamResult FromXml(XElement xml)
    {
        Guard.Against.Null(xml, nameof(xml));

        return new TeamResult
        {
            TeamId = xml.GetAttribute<int>("id"),
            Score = xml.GetAttribute<int>("score"),
            Errors = xml.GetAttribute<int>("errors"),
            Place = xml.GetAttribute<int>("place")
        };
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
