namespace MatchMaker.Reporting.Models;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

using MatchMaker.Models;
using MatchMaker.Reporting.Policies;

/// <summary>
/// Defines the <see cref="Summary" />
/// </summary>
/// <remarks>
/// Initializes an instance of the <see cref="Summary"/> class.
/// </remarks>
/// <param name="result">The results</param>
/// <param name="teamSummaries">The team summaries</param>
/// <param name="quizzerSummaries">The quizzer summaries</param>
public class Summary(Result result, IEnumerable<TeamRankingPolicy> policies)
{
    /// <summary>
    /// Gets or sets the quizzer summaries
    /// </summary>
    public IDictionary<int, QuizzerSummary> QuizzerSummaries { get; } = QuizzerSummary.FromResult(result);

    /// <summary>
    /// Gets or sets the Result
    /// </summary>
    public Result Result { get; } = result;

    /// <summary>
    /// Gets or sets the team summaries
    /// </summary>
    public IDictionary<int, TeamSummary> TeamSummaries { get; } = TeamSummary.FromResult(result, policies);

    /// <summary>
    /// Gets the Name
    /// </summary>
    public string Name => this.Result.Name;

    /// <summary>
    /// Gets or sets the number of teams participating in an elimination tournament. When greater
    /// than zero, the top <see cref="EliminationTeamIds"/> teams are marked in team reports as
    /// advancing to the elimination tournament.
    /// </summary>
    public int NumberOfEliminationTeams
    {
        get; set;
    }

    /// <summary>
    /// Gets the identifiers of the teams that qualify for the elimination tournament: the top
    /// <see cref="NumberOfEliminationTeams"/> teams, ordered by <see cref="TeamSummary.Place"/>
    /// with ties broken deterministically by team identifier. Empty when
    /// <see cref="NumberOfEliminationTeams"/> is zero or negative. This is the single source of
    /// truth for elimination-team membership, shared by the team report legend/asterisk and the
    /// elimination tournament artifact so the two always agree, even when ties in
    /// <see cref="TeamSummary.Place"/> occur at the qualification boundary.
    /// </summary>
    public IReadOnlyList<int> EliminationTeamIds =>
        this.NumberOfEliminationTeams <= 0
            ? []
            : this.TeamSummaries.Values
                .OrderBy(x => x.Place)
                .ThenBy(x => x.TeamId)
                .Take(this.NumberOfEliminationTeams)
                .Select(x => x.TeamId)
                .ToList();

    /// <summary>
    /// Creates a <see cref="Summary"/> based on a <see cref="Result"/> and collection of ranking policies.
    /// </summary>
    /// <param name="result">The <see cref="Result"/></param>
    /// <param name="policies">The <see cref="TeamRankingPolicy"/> instances</param>
    /// <returns>The <see cref="Summary"/></returns>
    public static Summary FromResult(Result result, IEnumerable<TeamRankingPolicy> policies)
    {
        Trace.WriteLine($"Creating summary for tournament: {result.Name}");
        Trace.Indent();

        try
        {
            var summary = new Summary(result, policies);
            Trace.WriteLine($"Summary created with {summary.TeamSummaries.Count} teams and {summary.QuizzerSummaries.Count} quizzers");
            return summary;
        }
        finally
        {
            Trace.Unindent();
        }
    }

    /// <summary>
    /// Converts the <see cref="Summary"/> instance to XML.
    /// </summary>
    /// <returns>The <see cref="XDocument"/> instance</returns>
    public XDocument ToXml()
    {
        Trace.WriteLine("Converting summary to XML format");
        var scheduleXml = this.Result.Schedule.ToXml();
        var resultXml = this.Result.ToXml();

        scheduleXml.Root?.Add(resultXml.Descendants("results"));

        return scheduleXml;
    }
}
