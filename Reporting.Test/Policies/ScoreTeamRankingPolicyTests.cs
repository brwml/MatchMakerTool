﻿namespace Reporting.Test.Policies;

using System.Collections.Generic;
using System.Linq;

using Bogus;

using MatchMaker.Models;
using MatchMaker.Reporting.Models;
using MatchMaker.Reporting.Policies;

using Xunit;

public class ScoreTeamRankingPolicyTests
{
    [Theory]
    [MemberData(nameof(GetTestCases))]
    public void TeamScoreTests(IEnumerable<TeamSummary> summaries, IDictionary<int, int> places)
    {
        var policy = new ScoreTeamRankingPolicy();
        policy.Rank(summaries, Result.Null);

        Assert.Equal(places, summaries.ToDictionary(x => x.TeamId, y => y.Place));
    }

    public static IEnumerable<object[]> GetTestCases()
    {
        return new List<object[]>
            {
                GetTestCase1(),
                GetTestCase2()
            };
    }

    private static object[] GetTestCase1()
    {
        var faker = new Faker();
        var score = faker.Random.Number(100, 1000);

        var summaries = new List<TeamSummary>
            {
                new() { TeamId = 1, TotalScore = score, Wins = 1 },
                new() { TeamId = 2, TotalScore = score - 10, Wins = 1 }
            };

        var places = new Dictionary<int, int>
            {
                { 1, 1 },
                { 2, 2 }
            };

        return [summaries, places];
    }

    private static object[] GetTestCase2()
    {
        var faker = new Faker();
        var score = faker.Random.Number(100, 1000);

        var summaries = new List<TeamSummary>
            {
                new() { TeamId = 1, TotalScore = score, Wins = 1 },
                new() { TeamId = 2, TotalScore = score, Wins = 1 }
            };

        var places = new Dictionary<int, int>
            {
                { 1, 1 },
                { 2, 1 }
            };

        return [summaries, places];
    }
}
