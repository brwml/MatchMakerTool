namespace MatchMaker.Reporting.Policies;

using System;
using System.Collections.Generic;

using MatchMaker.Models;

/// <summary>
/// Creates the default collection of <see cref="TeamRankingPolicy"/> instances for a given
/// <see cref="TournamentType"/>, so callers do not need to hard-code which ranking signals apply
/// to which tournament format.
/// </summary>
/// <remarks>
/// Elimination formats (single, double, and triple) rank teams by loss count first, since teams
/// play a varying number of matches depending on how far they advance, unlike round robin or
/// Swiss where every team plays a fixed number of rounds; win percentage is not a meaningful
/// ranking signal in elimination formats for that reason.
/// </remarks>
public static class TeamRankingPolicyFactory
{
    /// <summary>
    /// Gets the default <see cref="TeamRankingPolicy"/> chain for the given <see cref="TournamentType"/>.
    /// </summary>
    /// <param name="type">The tournament type.</param>
    /// <returns>The <see cref="IEnumerable{TeamRankingPolicy}"/> instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="type"/> is not a recognized <see cref="TournamentType"/> value.
    /// </exception>
    public static IEnumerable<TeamRankingPolicy> GetDefaultPolicies(TournamentType type)
    {
        return type switch
        {
            TournamentType.SingleElimination or TournamentType.DoubleElimination or TournamentType.TripleElimination =>
                [new LossCountTeamRankingPolicy(), new HeadToHeadTeamRankingPolicy(), new ScoreTeamRankingPolicy(), new ErrorTeamRankingPolicy()],
            TournamentType.RoundRobin or TournamentType.Swiss or TournamentType.SwissWithTopCut =>
                [new WinPercentageTeamRankingPolicy(), new HeadToHeadTeamRankingPolicy(), new ScoreTeamRankingPolicy(), new ErrorTeamRankingPolicy()],
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown tournament type."),
        };
    }
}
