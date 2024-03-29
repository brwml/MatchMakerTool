﻿namespace MatchMaker.Models;

using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;

/// <summary>
/// Defines the <see cref="MatchSchedule" />
/// </summary>
/// <remarks>
/// Initializes an instance of the <see cref="MatchSchedule"/> class.
/// </remarks>
/// <param name="id">The identifier</param>
/// <param name="room">The room</param>
/// <param name="teams">The teams</param>
[DebuggerDisplay("Match {Id} (Room {Room})")]
public class MatchSchedule(int id, int room, IList<int> teams)
{
    /// <summary>
    /// Gets or sets the Id
    /// </summary>
    public int Id { get; } = id;

    /// <summary>
    /// Gets or sets the room identifier
    /// </summary>
    public int Room { get; set; } = room;

    /// <summary>
    /// Gets or sets the Teams
    /// </summary>
    public IList<int> Teams { get; } = teams;

    /// <summary>
    /// Creates a <see cref="MatchSchedule"/> instance from an XML element.
    /// </summary>
    /// <param name="xml">The <see cref="XElement"/> instance</param>
    /// <returns>The <see cref="MatchSchedule"/> instance</returns>
    public static MatchSchedule FromXml(XElement xml)
    {
        var id = xml.GetAttribute<int>("id");
        var room = xml.GetAttribute<int>("room");
        var teams = new[]
        {
            xml.GetAttribute<int>("team1"),
            xml.GetAttribute<int>("team2")
        };

        return new MatchSchedule(id, room, teams);
    }

    /// <summary>
    /// Converts the <see cref="MatchSchedule" /> instance to XML.
    /// </summary>
    /// <returns>The <see cref="XElement"/> instance</returns>
    public XElement ToXml()
    {
        return new XElement(
            "match",
            new XAttribute("id", this.Id),
            new XAttribute("room", this.Room),
            new XAttribute("team1", this.Teams[0]),
            new XAttribute("team2", this.Teams[1]));
    }
}
