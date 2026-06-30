namespace Reporting.Test.Models;

using System.Collections.Generic;

using Bogus;

using MatchMaker.Models;

using Xunit;

public class MatchResultTests
{
    [Fact]
    public void VerifyXmlConversion()
    {
        var faker = new Faker();

        var teamResults = new List<TeamResult>
        {
            new(1, faker.Random.Int(0, 500), faker.Random.Int(0, 20), 1),
            new(2, faker.Random.Int(0, 500), faker.Random.Int(0, 20), 2),
        };

        var quizzerResults = new List<QuizzerResult>
        {
            new(101, faker.Random.Int(0, 100), faker.Random.Int(0, 5)),
            new(102, faker.Random.Int(0, 100), faker.Random.Int(0, 5)),
        };

        var expected = new MatchResult(
            faker.Random.Int(1, 9999),
            faker.Random.Int(1, 10),
            faker.Random.Int(1, 50),
            teamResults,
            quizzerResults);

        var actual = MatchResult.FromXml(expected.ToXml());

        Assert.Equal(expected.ScheduleId, actual.Id);
        Assert.Equal(expected.Room, actual.Room);
        Assert.Equal(expected.Round, actual.Round);
        Assert.Equal(expected.TeamResults.Count, actual.TeamResults.Count);
        Assert.Equal(expected.QuizzerResults.Count, actual.QuizzerResults.Count);
    }

    [Theory]
    [InlineData(1, 1, 101)]
    [InlineData(3, 5, 305)]
    [InlineData(10, 2, 1002)]
    [InlineData(99, 99, 9999)]
    public void ScheduleId_IsRoundTimeOneHundredPlusRoom(int round, int room, int expectedScheduleId)
    {
        var result = new MatchResult(1, room, round, [], []);
        Assert.Equal(expectedScheduleId, result.ScheduleId);
    }

    [Fact]
    public void ToXml_UsesScheduleIdAsId()
    {
        var result = new MatchResult(99, 3, 7, [], []);
        var xml = result.ToXml();

        var idAttr = xml.Attribute("id")?.Value;
        Assert.Equal(result.ScheduleId.ToString(), idAttr);
    }

    [Fact]
    public void ToXml_OrdersTeamResultsByTeamId()
    {
        var teamResults = new List<TeamResult>
        {
            new(5, 200, 0, 2),
            new(2, 300, 1, 1),
        };

        var result = new MatchResult(1, 1, 1, teamResults, []);
        var xml = result.ToXml();

        var teamIds = xml.Elements("team")
            .Select(e => int.Parse(e.Attribute("id")!.Value))
            .ToList();

        Assert.Equal([2, 5], teamIds);
    }

    [Fact]
    public void ToXml_OrdersQuizzerResultsByQuizzerId()
    {
        var quizzerResults = new List<QuizzerResult>
        {
            new(50, 80, 2),
            new(10, 60, 1),
        };

        var result = new MatchResult(1, 1, 1, [], quizzerResults);
        var xml = result.ToXml();

        var quizzerIds = xml.Elements("quizzer")
            .Select(e => int.Parse(e.Attribute("id")!.Value))
            .ToList();

        Assert.Equal([10, 50], quizzerIds);
    }
}
