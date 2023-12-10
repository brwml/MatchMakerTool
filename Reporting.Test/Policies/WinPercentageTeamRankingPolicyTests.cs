namespace Reporting.Test.Policies;

using System.Collections.Generic;
using System.Linq;

using Bogus;

using MatchMaker.Models;
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

    private static (List<TeamSummary>, Dictionary<int, int>) GetTestCase1()
    {
        var faker = new Faker();
        var wins = faker.Random.Number(1000, 10000);
        var losses = faker.Random.Number(1000, 10000);

        var summaries = new List<TeamSummary>
        {
            new() { TeamId = 1, Wins = wins, Losses = losses },
            new() { TeamId = 2, Wins = wins, Losses = losses + 1 }
        };

        var places = new Dictionary<int, int>
        {
            { 1, 1 },
            { 2, 2 }
        };

        return (summaries, places);
    }

    private static (List<TeamSummary>, Dictionary<int, int>) GetTestCase2()
    {
        var faker = new Faker();
        var wins = faker.Random.Number(1000, 10000);
        var losses = faker.Random.Number(1000, 10000);

        var summaries = new List<TeamSummary>
        {
            new() { TeamId = 1, Wins = wins, Losses = losses },
            new() { TeamId = 2, Wins = wins, Losses = losses }
        };

        var places = new Dictionary<int, int>
        {
            { 1, 1 },
            { 2, 1 }
        };

        return (summaries, places);
    }
}
