namespace Reporting.Test.Policies;

using System.Collections.Generic;
using System.Linq;

using Bogus;

using MatchMaker.Reporting.Models;
using MatchMaker.Reporting.Policies;

using Xunit;

public class WinPercentageTeamRankingPolicyTests
{
    [Theory]
    [MemberData(nameof(GetTestCases))]
    public void TeamWinTests(IEnumerable<TeamSummary> summaries, IDictionary<int, int> places)
    {
        var policy = new WinPercentageTeamRankingPolicy();
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
        var wins = faker.Random.Number(1000, 10000);
        var losses = faker.Random.Number(1000, 10000);

        var summaries = new List<TeamSummary>
            {
                new TeamSummary { TeamId = 1, Wins = wins, Losses = losses },
                new TeamSummary { TeamId = 2, Wins = wins, Losses = losses + 1 }
            };

        var places = new Dictionary<int, int>
            {
                { 1, 1 },
                { 2, 2 }
            };

        return new object[] { summaries, places };
    }

    private static object[] GetTestCase2()
    {
        var faker = new Faker();
        var wins = faker.Random.Number(1000, 10000);
        var losses = faker.Random.Number(1000, 10000);

        var summaries = new List<TeamSummary>
            {
                new TeamSummary { TeamId = 1, Wins = wins, Losses = losses },
                new TeamSummary { TeamId = 2, Wins = wins, Losses = losses }
            };

        var places = new Dictionary<int, int>
            {
                { 1, 1 },
                { 2, 1 }
            };

        return new object[] { summaries, places };
    }
}
