namespace MatchMaker.Reporting.Models;

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;

using Ardalis.GuardClauses;

/// <summary>
/// Defines the <see cref="MatchSchedule" />
/// </summary>
[DataContract]
public class MatchSchedule
{
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

        return new MatchSchedule
        {
            Id = xml.GetAttribute<int>("id"),
            Room = xml.GetAttribute<int>("room"),
            Teams = new[]
            {
                    xml.GetAttribute<int>("team1"),
                    xml.GetAttribute<int>("team2")
                }
        };
    }
}
