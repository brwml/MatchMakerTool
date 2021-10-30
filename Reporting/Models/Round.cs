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
    /// Gets or sets the date of the round.
    /// </summary>
    [DataMember]
    public DateOnly Date { get; set; }

    /// <summary>
    /// Gets or sets the time of the round.
    /// </summary>
    [DataMember]
    public TimeOnly Time { get; set; }

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
            Date = ConvertDate(xml.GetAttribute<string>("date")),
            Time = ConvertTime(xml.GetAttribute<string>("time")),
            Matches = xml.Elements("match").Select(x => MatchSchedule.FromXml(x)).ToDictionary(k => k.Id, v => v)
        };
    }

    /// <summary>
    /// Converts the given date string to a date object.
    /// </summary>
    /// <param name="date">The date string</param>
    /// <returns>The <see cref="DateOnly"/> instance. If the date cannot be parsed then the current date is returned.</returns>
    private static DateOnly ConvertDate(string date)
    {
        return DateOnly.TryParse(date, out var dateOnly) ? dateOnly : DateOnly.FromDateTime(DateTime.Now);
    }

    /// <summary>
    /// Converts the given time string to a time object.
    /// </summary>
    /// <param name="time">The time string</param>
    /// <returns>The <see cref="TimeOnly"/> instance. If the time cannot be parsed then the current time is returned.</returns>
    private static TimeOnly ConvertTime(string time)
    {
        return TimeOnly.TryParse(time, out var timeOnly) ? timeOnly : TimeOnly.FromDateTime(DateTime.Now);
    }
}
