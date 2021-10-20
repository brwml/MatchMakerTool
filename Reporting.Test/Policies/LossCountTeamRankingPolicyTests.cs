namespace Reporting.Test.Policies;

using System.Collections.Generic;
using System.Linq;

using MatchMaker.Reporting.Models;
using MatchMaker.Reporting.Policies;

using Xunit;

public class LossCountTeamRankingPolicyTests
{
    [Theory]
    [MemberData(nameof(GetTestCases))]
    public void LossCountTests(IEnumerable<TeamSummary> summaries, IDictionary<int, int> places)
    {
        var policy = new LossCountTeamRankingPolicy();
        policy.Rank(summaries, null);

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
        var summaries = new List<TeamSummary>
            {
                new TeamSummary { TeamId = 1, Losses = 1 },
                new TeamSummary { TeamId = 2, Losses = 2 }
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
        var summaries = new List<TeamSummary>
            {
                new TeamSummary { TeamId = 1, Losses = 1 },
                new TeamSummary { TeamId = 2, Losses = 1 }
            };

        var places = new Dictionary<int, int>
            {
                { 1, 1 },
                { 2, 1 }
            };

        return new object[] { summaries, places };
    }

}
