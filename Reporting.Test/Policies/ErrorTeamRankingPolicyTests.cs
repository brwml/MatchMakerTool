namespace Reporting.Test.Policies;

using System.Collections.Generic;
using System.Linq;

using Bogus;

using MatchMaker.Models;
using MatchMaker.Reporting.Models;
using MatchMaker.Reporting.Policies;

using Xunit;

public class ErrorTeamRankingPolicyTests
{
    [Theory]
    [MemberData(nameof(GetTestCases))]
    public void TeamErrorTests(IEnumerable<TeamSummary> summaries, IDictionary<int, int> places)
    {
        var policy = new ErrorTeamRankingPolicy();
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
        var errors = faker.Random.Number(1, 100);

        var summaries = new List<TeamSummary>
            {
                new TeamSummary { TeamId = 1, TotalErrors = errors, Wins = 1 },
                new TeamSummary { TeamId = 2, TotalErrors = errors + 1, Wins = 1 }
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
        var errors = faker.Random.Number(1, 100);

        var summaries = new List<TeamSummary>
            {
                new TeamSummary { TeamId = 1, TotalErrors = errors, Wins = 1 },
                new TeamSummary { TeamId = 2, TotalErrors = errors, Wins = 1 }
            };

        var places = new Dictionary<int, int>
            {
                { 1, 1 },
                { 2, 1 }
            };

        return new object[] { summaries, places };
    }
}
