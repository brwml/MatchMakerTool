namespace MatchMaker.Reporting.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;

using Ardalis.GuardClauses;

/// <summary>
/// Defines the <see cref="Round" />
/// </summary>
[DataContract]
public class Round
{
    /// <summary>
    /// Gets or sets the Id
    /// </summary>
    [DataMember]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Matches
    /// </summary>
    [DataMember]
    public IDictionary<int, MatchSchedule> Matches { get; set; }

    /// <summary>
    /// Gets or sets the start time
    /// </summary>
    [DataMember]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Creates a <see cref="Round"/> from an XML element
    /// </summary>
    /// <param name="xml">The <see cref="XElement"/></param>
    /// <returns>The <see cref="Round"/></returns>
    public static Round FromXml(XElement xml)
    {
        Guard.Against.Null(xml, nameof(xml));

        return new Round
        {
            Id = xml.GetAttribute<int>("id"),
            StartTime = ConvertDateTime(xml.Attribute("date").Value + " " + xml.Attribute("time").Value),
            Matches = xml.Elements("match").Select(x => MatchSchedule.FromXml(x)).ToDictionary(k => k.Id, v => v)
        };
    }

    /// <summary>
    /// Converts the date-time string to a <see cref="DateTime"/>
    /// </summary>
    /// <param name="dateTime">The date-time <see cref="string"/></param>
    /// <returns>The <see cref="DateTime"/></returns>
    private static DateTime ConvertDateTime(string dateTime)
    {
        return DateTime.TryParse(dateTime, out var result) ? result : DateTime.Today;
    }
}
