namespace MatchMaker.Models;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

/// <summary>
/// Defines the <see cref="MatchResult" />
/// </summary>
/// <remarks>
/// Initializes an instance of the <see cref="MatchResult"/> class.
/// </remarks>
/// <param name="id">The result identifier</param>
/// <param name="room">The room</param>
/// <param name="round">The round</param>
/// <param name="teamResults">The team results</param>
/// <param name="quizzerResults">The quizzer results</param>
[DebuggerDisplay("MatchResult {Id} (Round {Round}, Room {Room})")]
public class MatchResult(int id, int room, int round, IList<TeamResult> teamResults, IList<QuizzerResult> quizzerResults)
{
    /// <summary>
    /// Gets or sets the match identifier
    /// </summary>
    public int Id { get; } = id;

    /// <summary>
    /// Gets or sets the quizzer results
    /// </summary>
    public IList<QuizzerResult> QuizzerResults { get; } = quizzerResults;

    /// <summary>
    /// Gets or sets the room number
    /// </summary>
    public int Room { get; } = room;

    /// <summary>
    /// Gets or sets the round number
    /// </summary>
    public int Round { get; } = round;

    /// <summary>
    /// Gets the schedule identifier
    /// </summary>
    public int ScheduleId => (this.Round * 100) + this.Room;

    /// <summary>
    /// Gets or sets the team results
    /// </summary>
    public IList<TeamResult> TeamResults { get; } = teamResults;

    /// <summary>
    /// Create a <see cref="MatchResult"/> instance from the XML element.
    /// </summary>
    /// <param name="xml">The <see cref="XElement"/> instance</param>
    /// <returns>The <see cref="MatchResult"/> instance</returns>
    public static MatchResult FromXml(XElement xml)
    {
        var id = xml.GetAttribute<int>("id");
        var round = xml.GetAttribute<int>("round");
        var room = xml.GetAttribute<int>("room");
        var teamResults = xml.Elements("team").Select(TeamResult.FromXml).ToArray();
        var quizzerResults = xml.Elements("quizzer").Select(QuizzerResult.FromXml).ToArray();

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
