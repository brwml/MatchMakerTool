﻿namespace MatchMaker.Reporting.Models;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;

using Ardalis.GuardClauses;

/// <summary>
/// Defines the <see cref="MatchResult" />
/// </summary>
[DataContract]
[DebuggerDisplay("MatchResult {Id} (Round {Round}, Room {Room})")]
public class MatchResult
{
    /// <summary>
    /// Initializes an instance of the <see cref="MatchResult"/> class.
    /// </summary>
    /// <param name="id">The result identifier</param>
    /// <param name="room">The room</param>
    /// <param name="round">The round</param>
    /// <param name="teamResults">The team results</param>
    /// <param name="quizzerResults">The quizzer results</param>
    public MatchResult(int id, int room, int round, IList<TeamResult> teamResults, IList<QuizzerResult> quizzerResults)
    {
        this.Id = id;
        this.Room = room;
        this.Round = round;
        this.TeamResults = teamResults;
        this.QuizzerResults = quizzerResults;
    }

    /// <summary>
    /// Gets or sets the match identifier
    /// </summary>
    [DataMember]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the quizzer results
    /// </summary>
    [DataMember]
    public IList<QuizzerResult> QuizzerResults { get; set; }

    /// <summary>
    /// Gets or sets the room number
    /// </summary>
    [DataMember]
    public int Room { get; set; }

    /// <summary>
    /// Gets or sets the round number
    /// </summary>
    [DataMember]
    public int Round { get; set; }

    /// <summary>
    /// Gets the schedule identifier
    /// </summary>
    [IgnoreDataMember]
    public int ScheduleId => (this.Round * 100) + this.Room;

    /// <summary>
    /// Gets or sets the team results
    /// </summary>
    [DataMember]
    public IList<TeamResult> TeamResults { get; set; }

    /// <summary>
    /// Create a <see cref="MatchResult"/> instance from the XML element.
    /// </summary>
    /// <param name="xml">The <see cref="XElement"/> instance</param>
    /// <returns>The <see cref="MatchResult"/> instance</returns>
    public static MatchResult FromXml(XElement xml)
    {
        Guard.Against.Null(xml, nameof(xml));

        var id = xml.GetAttribute<int>("id");
        var round = xml.GetAttribute<int>("round");
        var room = xml.GetAttribute<int>("room");
        var teamResults = xml.Elements("team").Select(x => TeamResult.FromXml(x)).ToArray();
        var quizzerResults = xml.Elements("quizzer").Select(x => QuizzerResult.FromXml(x)).ToArray();

        return new MatchResult(id, room, round, teamResults, quizzerResults);
    }

    /// <summary>
    /// Converts the <see cref="MatchResult"/> instance to XML.
    /// </summary>
    /// <returns>The <see cref="XElement"/> instance</returns>
    public XElement ToXml()
    {
        return new XElement(
            "match",
            new XAttribute("id", this.ScheduleId),
            new XAttribute("round", this.Round),
            new XAttribute("room", this.Room),
            this.TeamResults.OrderBy(x => x.TeamId).Select(x => x.ToXml()),
            this.QuizzerResults.OrderBy(x => x.QuizzerId).Select(x => x.ToXml()));
    }
}
