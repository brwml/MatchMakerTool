namespace MatchMaker.Reporting.Models;

using System.Collections.Generic;
using System.Xml.Linq;

using Ardalis.GuardClauses;

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
    /// Creates a <see cref="Summary"/> based on a <see cref="Result"/> and collection of ranking policies.
    /// </summary>
    /// <param name="result">The <see cref="Result"/></param>
    /// <param name="policies">The <see cref="TeamRankingPolicy"/> instances</param>
    /// <returns>The <see cref="Summary"/></returns>
    public static Summary FromResult(Result result, IEnumerable<TeamRankingPolicy> policies)
    {
        Guard.Against.Null(result);
        Guard.Against.NullOrEmpty(policies);

        return new Summary(result, policies);
    }

    /// <summary>
    /// Converts the <see cref="Summary"/> instance to XML.
    /// </summary>
    /// <returns>The <see cref="XDocument"/> instance</returns>
    public XDocument ToXml()
    {
        var scheduleXml = this.Result.Schedule.ToXml();
        var resultXml = this.Result.ToXml();

        scheduleXml.Root?.Add(resultXml.Descendants("results"));

        return scheduleXml;
    }
}
