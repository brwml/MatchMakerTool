namespace Reporting.Test.Policies;

using System;
using System.Linq;

using MatchMaker.Models;
using MatchMaker.Reporting.Policies;

using Xunit;

public class TeamRankingPolicyFactoryTests
{
    [Theory]
    [InlineData(TournamentType.SingleElimination)]
    [InlineData(TournamentType.DoubleElimination)]
    [InlineData(TournamentType.TripleElimination)]
    public void GetDefaultPolicies_ForEliminationFormats_StartsWithLossCountPolicy(TournamentType type)
    {
        var policies = TeamRankingPolicyFactory.GetDefaultPolicies(type).ToList();

        Assert.IsType<LossCountTeamRankingPolicy>(policies[0]);
        Assert.DoesNotContain(policies, p => p is WinPercentageTeamRankingPolicy);
    }

    [Theory]
    [InlineData(TournamentType.RoundRobin)]
    [InlineData(TournamentType.Swiss)]
    [InlineData(TournamentType.SwissWithTopCut)]
    public void GetDefaultPolicies_ForNonEliminationFormats_StartsWithWinPercentagePolicy(TournamentType type)
    {
        var policies = TeamRankingPolicyFactory.GetDefaultPolicies(type).ToList();

        Assert.IsType<WinPercentageTeamRankingPolicy>(policies[0]);
        Assert.DoesNotContain(policies, p => p is LossCountTeamRankingPolicy);
    }

    [Fact]
    public void GetDefaultPolicies_ForUnknownType_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TeamRankingPolicyFactory.GetDefaultPolicies((TournamentType)(-1)).ToList());
    }
}
