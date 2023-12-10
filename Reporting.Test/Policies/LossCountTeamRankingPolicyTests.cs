namespace Reporting.Test.Policies;

using System.Collections.Generic;
using System.Linq;

using MatchMaker.Models;
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
        policy.Rank(summaries, Result.Null);

        Assert.Equal(places, summaries.ToDictionary(x => x.TeamId, y => y.Place));
    }

    public static TheoryData<IEnumerable<TeamSummary>, IDictionary<int, int>> GetTestCases()
    {
        var data = new TheoryData<IEnumerable<TeamSummary>, IDictionary<int, int>>();

        AddRow(data, GetTestCase1());
        AddRow(data, GetTestCase2());

        return data;
    }

    private static void AddRow(TheoryData<IEnumerable<TeamSummary>, IDictionary<int, int>> data, (IEnumerable<TeamSummary>, IDictionary<int, int>) tuple)
    {
        data.Add(tuple.Item1, tuple.Item2);
    }

    private static (IEnumerable<TeamSummary>, IDictionary<int, int>) GetTestCase1()
    {
        var summaries = new List<TeamSummary>
        {
            new() { TeamId = 1, Losses = 1 },
            new() { TeamId = 2, Losses = 2 }
        };

        var places = new Dictionary<int, int>
        {
            { 1, 1 },
            { 2, 2 }
        };

        return (summaries, places);
    }

    private static (IEnumerable<TeamSummary>, IDictionary<int, int>) GetTestCase2()
    {
        var summaries = new List<TeamSummary>
        {
            new() { TeamId = 1, Losses = 1 },
            new() { TeamId = 2, Losses = 1 }
        };

        var places = new Dictionary<int, int>
        {
            { 1, 1 },
            { 2, 1 }
        };

        return (summaries, places);
    }
}
