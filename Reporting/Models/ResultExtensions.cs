namespace MatchMaker.Reporting.Models;

using System.Collections.Generic;

using MatchMaker.Models;
using MatchMaker.Reporting.Policies;

internal static class ResultExtensions
{
    /// <summary>
    /// Creates a <see cref="Summary"/> based on a <see cref="Result"/> and collection of ranking policies.
    /// </summary>
    /// <param name="result">The <see cref="Result"/></param>
    /// <param name="policies">The <see cref="TeamRankingPolicy"/> instances</param>
    /// <returns>The <see cref="Summary"/></returns>
    public static Summary ToSummary(this Result result, IEnumerable<TeamRankingPolicy> policies)
    {
        return new Summary(result, policies);
    }
}
