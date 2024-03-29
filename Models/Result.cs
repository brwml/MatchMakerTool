﻿namespace MatchMaker.Models;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

/// <summary>
/// Defines the <see cref="Result" />
/// </summary>
/// <remarks>
/// Initializes an instance of the <see cref="Result"/> class.
/// </remarks>
/// <param name="schedule">The schedule</param>
/// <param name="matches">The matches</param>
public class Result(Schedule schedule, IDictionary<int, MatchResult> matches)
{
    /// <summary>
    /// Gets an empty result instance.
    /// </summary>
    public static Result Null { get; } = new Result(Schedule.Null, new Dictionary<int, MatchResult>());

    /// <summary>
    /// Gets or sets the Matches
    /// </summary>
    public IDictionary<int, MatchResult> Matches { get; } = matches;

    /// <summary>
    /// Gets or sets the Schedule
    /// </summary>
    public Schedule Schedule { get; } = schedule;

    /// <summary>
    /// Gets the name of the schedule.
    /// </summary>
    public string Name => this.Schedule.Name;

    /// <summary>
    /// Creates a <see cref="Result"/> from XML documents and corresponding <see cref="Schedule"/>.
    /// </summary>
    /// <param name="documents">The <see cref="XDocument"/> instances</param>
    /// <param name="schedule">The <see cref="Schedule"/></param>
    /// <returns>The <see cref="Result"/></returns>
    public static Result FromXml(IEnumerable<XDocument> documents, Schedule schedule)
    {
        var matches = documents.SelectMany(LoadMatches).ToDictionary(m => m.ScheduleId, m => m);

        return new Result(schedule, matches);
    }

    /// <summary>
    /// Converts the <see cref="Result"/> instance to XML.
    /// </summary>
    /// <returns>The <see cref="XElement"/> instance</returns>
    public XDocument ToXml()
    {
        return new XDocument(
            new XmlDocumentDeclaration(),
            new XElement(
                "members",
                new XElement(
                    "results",
                    this.Matches.Select(x => x.Value).OrderBy(x => x.ScheduleId).Select(x => x.ToXml()))));
    }

    /// <summary>
    /// Loads the matches from the <see cref="XDocument"/>
    /// </summary>
    /// <param name="document">The <see cref="XDocument"/></param>
    /// <returns>The <see cref="IEnumerable{MatchResult}"/></returns>
    private static IEnumerable<MatchResult> LoadMatches(XDocument document)
    {
        return document.XPathSelectElements("/members/results/match").Select(MatchResult.FromXml);
    }
}
