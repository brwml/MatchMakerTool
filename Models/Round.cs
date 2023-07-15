namespace MatchMaker.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Ardalis.GuardClauses;

/// <summary>
/// Defines the <see cref="Round" />
/// </summary>
public class Round
{
    /// <summary>
    /// Initializes an instance of the <see cref="Round"/> class.
    /// </summary>
    /// <param name="id">The identifier</param>
    /// <param name="matches">The matches</param>
    /// <param name="date">The date</param>
    /// <param name="time">The time</param>
    public Round(int id, IDictionary<int, MatchSchedule> matches, DateOnly date, TimeOnly time)
    {
        this.Id = id;
        this.Matches = matches;
        this.Date = date;
        this.Time = time;
    }

    /// <summary>
    /// Initializes an instance of the <see cref="Round"/> class
    /// </summary>
    /// <param name="id">The identifier</param>
    public Round(int id)
        : this(id, new Dictionary<int, MatchSchedule>(), DateOnly.FromDateTime(DateTime.Now), TimeOnly.FromDateTime(DateTime.Now))
    {
    }

    /// <summary>
    /// Gets or sets the Id
    /// </summary>
    public int Id
    {
        get;
    }

    /// <summary>
    /// Gets or sets the Matches
    /// </summary>
    public IDictionary<int, MatchSchedule> Matches
    {
        get;
    }

    /// <summary>
    /// Gets or sets the date of the round.
    /// </summary>
    public DateOnly Date
    {
        get;
    }

    /// <summary>
    /// Gets or sets the time of the round.
    /// </summary>
    public TimeOnly Time
    {
        get;
    }

    /// <summary>
    /// Creates a <see cref="Round"/> from an XML element
    /// </summary>
    /// <param name="xml">The <see cref="XElement"/></param>
    /// <returns>The <see cref="Round"/></returns>
    public static Round FromXml(XElement xml)
    {
        Guard.Against.Null(xml);

        var id = xml.GetAttribute<int>("id");
        var date = ConvertDate(xml.GetAttribute<string>("date"));
        var time = ConvertTime(xml.GetAttribute<string>("time"));
        var matches = xml.Elements("match").Select(x => MatchSchedule.FromXml(x)).ToDictionary(k => k.Id, v => v);

        return new Round(id, matches, date, time);
    }

    /// <summary>
    /// Converts the <see cref="Round"/> instance to XML.
    /// </summary>
    /// <returns>The <see cref="XElement"/> instance</returns>
    public XElement ToXml()
    {
        return new XElement(
            "round",
            new XAttribute("id", this.Id),
            new XAttribute("date", this.Date),
            new XAttribute("time", this.Time),
            this.Matches.Select(x => x.Value.ToXml()));
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
