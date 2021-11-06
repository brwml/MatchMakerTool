namespace MatchMaker.Reporting.Models;

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;

using Ardalis.GuardClauses;

using MatchMaker.Reporting.Policies;

/// <summary>
/// Defines the <see cref="Summary" />
/// </summary>
[DataContract]
public class Summary
{
    /// <summary>
    /// Initializes an instance of the <see cref="Summary"/> class.
    /// </summary>
    /// <param name="result">The results</param>
    /// <param name="teamSummaries">The team summaries</param>
    /// <param name="quizzerSummaries">The quizzer summaries</param>
    public Summary(Result result, IEnumerable<TeamRankingPolicy> policies)
    {
        this.Result = result;
        this.TeamSummaries = TeamSummary.FromResult(result, policies);
        this.QuizzerSummaries = QuizzerSummary.FromResult(result);
    }

    /// <summary>
    /// Gets or sets the quizzer summaries
    /// </summary>
    [DataMember]
    public IDictionary<int, QuizzerSummary> QuizzerSummaries { get; set; }

    /// <summary>
    /// Gets or sets the Result
    /// </summary>
    [DataMember]
    public Result Result { get; set; }

    /// <summary>
    /// Gets or sets the team summaries
    /// </summary>
    [DataMember]
    public IDictionary<int, TeamSummary> TeamSummaries { get; set; }

    /// <summary>
    /// Gets the Name
    /// </summary>
    [IgnoreDataMember]
    public string Name => this.Result?.Name ?? string.Empty;

    /// <summary>
    /// Creates a <see cref="Summary"/> based on a <see cref="Result"/> and collection of ranking policies.
    /// </summary>
    /// <param name="result">The <see cref="Result"/></param>
    /// <param name="policies">The <see cref="TeamRankingPolicy"/> instances</param>
    /// <returns>The <see cref="Summary"/></returns>
    public static Summary FromResult(Result result, IEnumerable<TeamRankingPolicy> policies)
    {
        Guard.Against.Null(result, nameof(result));
        Guard.Against.NullOrEmpty(policies, nameof(policies));

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
