namespace Reporting.Test.Policies;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Bogus;

using MatchMaker.Reporting.Models;
using MatchMaker.Reporting.Policies;

using Xunit;

public class ErrorQuizzerRankingPolicyTests
{
    [Theory]
    [MemberData(nameof(GetTestCases))]
    [SuppressMessage("Usage", "xUnit1045:Avoid using TheoryData type arguments that might not be serializable", Justification = "<Pending>")]
    public void QuizzerErrorTests(IEnumerable<QuizzerSummary> summaries, IDictionary<int, int> places)
    {
        var policy = new ErrorQuizzerRankingPolicy();
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

        return (summaries, places);
    }

    private static (IEnumerable<QuizzerSummary>, IDictionary<int, int>) GetTestCase2()
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

        return (summaries, places);
    }
}
