﻿namespace MatchMaker.Models;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

/// <summary>
/// Defines the <see cref="Schedule" />
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Schedule"/> class.
/// </remarks>
/// <param name="name">The schedule name</param>
/// <param name="churches">The churches</param>
/// <param name="quizzers">The quizzers</param>
/// <param name="teams">The teams</param>
/// <param name="rounds">The rounds</param>
[DebuggerDisplay("Schedule {Name}")]
public class Schedule(string name, IDictionary<int, Church> churches, IDictionary<int, Quizzer> quizzers, IDictionary<int, Team> teams, IDictionary<int, Round> rounds)
{
    /// <summary>
    /// Gets an empty schedule instance.
    /// </summary>
    public static Schedule Null { get; } = new Schedule(string.Empty, new Dictionary<int, Church>(), new Dictionary<int, Quizzer>(), new Dictionary<int, Team>(), new Dictionary<int, Round>());

    /// <summary>
    /// Gets or sets the Churches
    /// </summary>
    public IDictionary<int, Church> Churches { get; } = churches;

    /// <summary>
    /// Gets or sets the Name
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Gets or sets the Quizzers
    /// </summary>
    public IDictionary<int, Quizzer> Quizzers { get; } = quizzers;

    /// <summary>
    /// Gets or sets the Rounds
    /// </summary>
    /// <remarks>
    /// TODO: Make property read-only.
    /// </remarks>
    public IDictionary<int, Round> Rounds { get; set; } = rounds;

    /// <summary>
    /// Gets or sets the Teams
    /// </summary>
    public IDictionary<int, Team> Teams { get; } = teams;

    /// <summary>
    /// Creates a <see cref="Schedule"/> from an <see cref="XDocument"/> and a <paramref name="name"/>.
    /// </summary>
    /// <param name="document">The <see cref="XDocument"/></param>
    /// <param name="name">The tournament name</param>
    /// <returns>The <see cref="Schedule"/></returns>
    public static Schedule FromXml(XDocument document, string name)
    {
        return PopulateSchedule(document, name);
    }

    /// <summary>
    /// Converts the <see cref="Schedule"/> instance to XML.
    /// </summary>
    /// <returns>The <see cref="XDocument"/> instance</returns>
    public XDocument ToXml()
    {
        return new XDocument(
            new XmlDocumentDeclaration(),
            new XElement(
                "members",
                new XElement(
                    "churches",
                    this.Churches.Select(x => x.Value.ToXml())),
                new XElement(
                    "teams",
                    this.Teams.Select(x => x.Value.ToXml())),
                new XElement(
                    "quizzers",
                    this.Quizzers.Select(x => x.Value.ToXml())),
                new XElement(
                    "schedule",
                    this.Rounds.Select(x => x.Value.ToXml()))));
    }

    /// <summary>
    /// Loads the churches from the <see cref="XDocument"/>
    /// </summary>
    /// <param name="document">The <see cref="XDocument"/></param>
    /// <returns>The <see cref="Dictionary{int, Church}"/></returns>
    private static Dictionary<int, Church> LoadChurches(XDocument document)
    {
        return document.XPathSelectElements("/members/churches/church")
            .Select(Church.FromXml)
            .ToDictionary(k => k.Id, v => v);
    }

    /// <summary>
    /// Loads the quizzers from the <see cref="XDocument"/>
    /// </summary>
    /// <param name="document">The <see cref="XDocument"/></param>
    /// <returns>The <see cref="Dictionary{int, Quizzer}"/></returns>
    private static Dictionary<int, Quizzer> LoadQuizzers(XDocument document)
    {
        return document.XPathSelectElements("/members/quizzers/quizzer")
            .Select(Quizzer.FromXml)
            .ToDictionary(k => k.Id, v => v);
    }

    /// <summary>
    /// Loads the rounds from the <see cref="XDocument"/>
    /// </summary>
    /// <param name="document">The <see cref="XDocument"/></param>
    /// <returns>The <see cref="Dictionary{int, Round}"/></returns>
    private static Dictionary<int, Round> LoadRounds(XDocument document)
    {
        return document.XPathSelectElements("/members/schedule/round")
            .Select(Round.FromXml)
            .ToDictionary(k => k.Id, v => v);
    }

    /// <summary>
    /// Loads the teams from the <see cref="XDocument"/>
    /// </summary>
    /// <param name="document">The <see cref="XDocument"/></param>
    /// <returns>The <see cref="Dictionary{int, Team}"/></returns>
    private static Dictionary<int, Team> LoadTeams(XDocument document)
    {
        return document.XPathSelectElements("/members/teams/team")
            .Select(Team.FromXml)
            .ToDictionary(k => k.Id, v => v);
    }

    /// <summary>
    /// Populates the schedule.
    /// </summary>
    /// <param name="document">The <see cref="XDocument"/></param>
    /// <param name="name">The name</param>
    /// <returns>The <see cref="Schedule"/></returns>
    private static Schedule PopulateSchedule(XDocument document, string name)
    {
        var churches = LoadChurches(document);
        var teams = LoadTeams(document);
        var quizzers = LoadQuizzers(document);
        var rounds = LoadRounds(document);
        return new Schedule(name, churches, quizzers, teams, rounds);
    }
}

/// <summary>
/// Extension methods for <see cref="Schedule"/> objects
/// </summary>
public static class ScheduleExtensions
{
    /// <summary>
    /// Yields the schedule object with the provided name.
    /// </summary>
    /// <param name="schedule">The schedule.</param>
    /// <param name="name">The name.</param>
    /// <returns>The schedule</returns>
    public static Schedule WithName(this Schedule schedule, string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            schedule.Name = name;
        }

        return schedule;
    }
}
