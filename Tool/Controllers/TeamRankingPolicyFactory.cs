namespace MatchMaker.Tool.Controllers;

using System.Collections.Generic;
using System.Linq;

using MatchMaker.Reporting.Policies;

/// <summary>
/// A factory class that creates <see cref="TeamRankingPolicy"/> instances.
/// </summary>
internal class TeamRankingPolicyFactory
{
    /// <summary>
    /// Gets a collection of <see cref="TeamRankingPolicy"/> instances.
    /// </summary>
    /// <param name="policies">The policy definitions</param>
    /// <returns>The <see cref="IEnumerable{TeamRankingPolicy}"/> instance</returns>
    public static IEnumerable<TeamRankingPolicy> GetTeamRankingPolicies(string policies)
    {
        return policies.Select(GetTeamRankingPolicy);
    }

    /// <summary>
    /// Gets a <see cref="TeamRankingPolicy"/> instance.
    /// </summary>
    /// <param name="c">The policy character definition</param>
    /// <returns>The <see cref="TeamRankingPolicy"/> instance</returns>
    public static TeamRankingPolicy GetTeamRankingPolicy(char c)
    {
        return char.ToUpperInvariant(c) switch
        {
            'W' => new WinPercentageTeamRankingPolicy(),
            'S' => new ScoreTeamRankingPolicy(),
            'E' => new ErrorTeamRankingPolicy(),
            'H' => new HeadToHeadTeamRankingPolicy(),
            'L' => new LossCountTeamRankingPolicy(),
            _ => new NullTeamRankingPolicy(),
        };
    }
}
