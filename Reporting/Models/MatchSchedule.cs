namespace MatchMaker.Reporting.Models;

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Linq;

using Ardalis.GuardClauses;

/// <summary>
/// Defines the <see cref="MatchSchedule" />
/// </summary>
[DataContract]
[DebuggerDisplay("Match {Id} (Room {Room})")]
public class MatchSchedule
{
    /// <summary>
    /// Initializes an instance of the <see cref="MatchSchedule"/> class.
    /// </summary>
    /// <param name="id">The identifier</param>
    /// <param name="room">The room</param>
    /// <param name="teams">The teams</param>
    public MatchSchedule(int id, int room, IList<int> teams)
    {
        this.Id = id;
        this.Room = room;
        this.Teams = teams;
    }

    /// <summary>
    /// Gets or sets the Id
    /// </summary>
    [DataMember]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the room identifier
    /// </summary>
    [DataMember]
    public int Room { get; set; }

    /// <summary>
    /// Gets or sets the Teams
    /// </summary>
    [DataMember]
    public IList<int> Teams { get; set; }

    /// <summary>
    /// Creates a <see cref="MatchSchedule"/> instance from an XML element.
    /// </summary>
    /// <param name="xml">The <see cref="XElement"/> instance</param>
    /// <returns>The <see cref="MatchSchedule"/> instance</returns>
    public static MatchSchedule FromXml(XElement xml)
    {
        Guard.Against.Null(xml, nameof(xml));

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
