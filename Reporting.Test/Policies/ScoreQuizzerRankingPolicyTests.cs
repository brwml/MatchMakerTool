namespace Reporting.Test.Policies;

using System.Collections.Generic;
using System.Linq;

using Bogus;

using MatchMaker.Reporting.Models;
using MatchMaker.Reporting.Policies;

using Xunit;

public class ScoreQuizzerRankingPolicyTests
{
    [Theory]
    [MemberData(nameof(GetTestCases))]
    public void ScoreTests(IEnumerable<QuizzerSummary> summaries, IDictionary<int, int> places)
    {
        var policy = new ScoreQuizzerRankingPolicy();
        policy.Rank(summaries);

        Assert.Equal(places, summaries.ToDictionary(x => x.QuizzerId, y => y.Place));
    }

    public static TheoryData<IEnumerable<QuizzerSummary>, IDictionary<int, int>> GetTestCases()
    {
        var data = new TheoryData<IEnumerable<QuizzerSummary>, IDictionary<int, int>>();

        AddRow(data, GetTestCase1());
        AddRow(data, GetTestCase2());

        return data;
    }

    private static void AddRow(TheoryData<IEnumerable<QuizzerSummary>, IDictionary<int, int>> data, (IEnumerable<QuizzerSummary>, IDictionary<int, int>) tuple)
    {
        data.Add(tuple.Item1, tuple.Item2);
    }

    private static (IEnumerable<QuizzerSummary>, IDictionary<int, int>) GetTestCase1()
    {
        var faker = new Faker();
        var points1 = faker.Random.Number(100, 200);
        var rounds1 = faker.Random.Number(10, 20);

        var points2 = faker.Random.Number(100, 200);
        var rounds2 = faker.Random.Number(100, 200);

        var summaries = new List<QuizzerSummary>
        {
            new() { QuizzerId = 1, TotalScore = points1, TotalRounds = rounds1 },
            new() { QuizzerId = 2, TotalScore = points2, TotalRounds = rounds2 }
        };

        var places = new Dictionary<int, int>
        {
            { 1, 1 },
            { 2, 2 }
        };

        return (summaries, places);
    }

    private static (IEnumerable<QuizzerSummary>, IDictionary<int, int>) GetTestCase2()
    {
        var faker = new Faker();
        var x = faker.Random.Number(10, 20);
        var y = faker.Random.Number(10, 20);
        var z = faker.Random.Number(10, 20);

        var summaries = new List<QuizzerSummary>
        {
            new() { QuizzerId = 1, TotalRounds = x, TotalScore = x * y },
            new() { QuizzerId = 2, TotalRounds = x * z, TotalScore = x * y * z }
        };

        var places = new Dictionary<int, int>
        {
            { 1, 1 },
            { 2, 1 }
        };

        return (summaries, places);
    }
}
