namespace Reporting.Test.Policies;

using System.Collections.Generic;

using MatchMaker.Models;
using MatchMaker.Reporting.Models;
using MatchMaker.Reporting.Policies;

using Xunit;

public class NullTeamRankingPolicyTests
{
    [Fact]
    public void NullTeamRankingPolicy_NoEffect_PlacesUnchanged()
    {
        var policy = new NullTeamRankingPolicy();
        var summaries = new List<TeamSummary>
        {
            new() { TeamId = 1, Place = 1, TotalScore = 100 },
            new() { TeamId = 2, Place = 1, TotalScore = 100 },
            new() { TeamId = 3, Place = 1, TotalScore = 100 }
        };

        var originalPlaces = new Dictionary<int, int>
        {
            { 1, summaries[0].Place },
            { 2, summaries[1].Place },
            { 3, summaries[2].Place }
        };

        policy.Rank(summaries, Result.Null);

        Assert.Equal(originalPlaces[1], summaries[0].Place);
        Assert.Equal(originalPlaces[2], summaries[1].Place);
        Assert.Equal(originalPlaces[3], summaries[2].Place);
    }

    [Fact]
    public void NullTeamRankingPolicy_SingleTeamGroup_NoChange()
    {
        var policy = new NullTeamRankingPolicy();
        var summaries = new List<TeamSummary>
        {
            new() { TeamId = 1, Place = 1, TotalScore = 100 }
        };

        policy.Rank(summaries, Result.Null);

        Assert.Equal(1, summaries[0].Place);
    }

    [Fact]
    public void NullTeamRankingPolicy_EmptyGroup_NoException()
    {
        var policy = new NullTeamRankingPolicy();
        var summaries = new List<TeamSummary>();

        Assert.Null(Record.Exception(() => policy.Rank(summaries, Result.Null)));
    }
}
