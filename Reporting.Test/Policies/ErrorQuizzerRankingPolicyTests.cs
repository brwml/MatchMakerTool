namespace Reporting.Test.Policies;

using System.Collections.Generic;
using System.Linq;

using Bogus;

using MatchMaker.Reporting.Models;
using MatchMaker.Reporting.Policies;

using Xunit;

public class ErrorQuizzerRankingPolicyTests
{
    [Theory]
    [MemberData(nameof(GetTestCases))]
    public void QuizzerErrorTests(IEnumerable<QuizzerSummary> summaries, IDictionary<int, int> places)
    {
        var policy = new ErrorQuizzerRankingPolicy();
        policy.Rank(summaries);

        Assert.Equal(places, summaries.ToDictionary(x => x.QuizzerId, y => y.Place));
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

        var summaries = new List<QuizzerSummary>
        {
            new() { QuizzerId = 1, TotalErrors = errors },
            new() { QuizzerId = 2, TotalErrors = errors + 1 }
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
        var errors = faker.Random.Number(1, 100);

        var summaries = new List<QuizzerSummary>
        {
            new() { QuizzerId = 1, TotalErrors = errors },
            new() { QuizzerId = 2, TotalErrors = errors }
        };

        var places = new Dictionary<int, int>
        {
            { 1, 1 },
            { 2, 1 }
        };

        return [summaries, places];
    }
}
