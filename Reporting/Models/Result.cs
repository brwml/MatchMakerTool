﻿namespace MatchMaker.Reporting.Models;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.XPath;

using Ardalis.GuardClauses;

/// <summary>
/// Defines the <see cref="Result" />
/// </summary>
[DataContract]
public class Result
{
    /// <summary>
    /// Initializes an instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="schedule">The schedule</param>
    /// <param name="matches">The matches</param>
    public Result(Schedule schedule, IDictionary<int, MatchResult> matches)
    {
        this.Schedule = schedule;
        this.Matches = matches;
    }

    /// <summary>
    /// Gets or sets the Matches
    /// </summary>
    [DataMember]
    public IDictionary<int, MatchResult> Matches { get; set; }

    /// <summary>
    /// Gets or sets the Schedule
    /// </summary>
    [DataMember]
    public Schedule Schedule { get; set; }

    /// <summary>
    /// Gets the Name
    /// </summary>
    [IgnoreDataMember]
    public string Name => this.Schedule?.Name ?? string.Empty;

    /// <summary>
    /// Converts the <see cref="Result"/> instance to XML.
    /// </summary>
    /// <returns>The <see cref="XElement"/> instance</returns>
    public XElement ToXml()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Creates a <see cref="Result"/> from XML documents and corresponding <see cref="Schedule"/>.
    /// </summary>
    /// <param name="documents">The <see cref="XDocument"/> instances</param>
    /// <param name="schedule">The <see cref="Schedule"/></param>
    /// <returns>The <see cref="Result"/></returns>
    public static Result FromXml(IEnumerable<XDocument> documents, Schedule schedule)
    {
        Guard.Against.NullOrEmpty(documents, nameof(documents));
        Guard.Against.Null(schedule, nameof(schedule));

        var matches = documents.SelectMany(x => LoadMatches(x)).ToDictionary(m => m.ScheduleId, m => m);

        return new Result(schedule, matches);
    }

    /// <summary>
    /// Loads the matches from the <see cref="XDocument"/>
    /// </summary>
    /// <param name="document">The <see cref="XDocument"/></param>
    /// <returns>The <see cref="IEnumerable{MatchResult}"/></returns>
    private static IEnumerable<MatchResult> LoadMatches(XDocument document)
    {
        return document.XPathSelectElements("/members/results/match").Select(x => MatchResult.FromXml(x));
    }
}
