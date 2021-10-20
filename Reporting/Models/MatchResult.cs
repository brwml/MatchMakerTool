namespace MatchMaker.Reporting.Models;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;

using Ardalis.GuardClauses;

/// <summary>
/// Defines the <see cref="MatchResult" />
/// </summary>
[DataContract]
public class MatchResult
{
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

        return new MatchResult
        {
            Id = xml.GetAttribute<int>("id"),
            Round = xml.GetAttribute<int>("round"),
            Room = xml.GetAttribute<int>("room"),
            TeamResults = xml.Elements("team").Select(x => TeamResult.FromXml(x)).ToArray(),
            QuizzerResults = xml.Elements("quizzer").Select(x => QuizzerResult.FromXml(x)).ToArray()
        };
    }
}
