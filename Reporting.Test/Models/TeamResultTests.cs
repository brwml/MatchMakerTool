namespace Reporting.Test.Models;

using Bogus;

using MatchMaker.Models;

using Xunit;

public class TeamResultTests
{
    [Fact]
    public void VerifyXmlConversion()
    {
        var faker = new Faker();

        var expected = new TeamResult(
            faker.Random.Int(1, 9999),
            faker.Random.Int(0, 500),
            faker.Random.Int(0, 50),
            faker.Random.Int(1, 2));

        var actual = TeamResult.FromXml(expected.ToXml());

        Assert.Equal(expected.TeamId, actual.TeamId);
        Assert.Equal(expected.Score, actual.Score);
        Assert.Equal(expected.Errors, actual.Errors);
        Assert.Equal(expected.Place, actual.Place);
    }

    [Fact]
    public void ToXml_ProducesTeamElement()
    {
        var result = new TeamResult(7, 300, 5, 1);
        var xml = result.ToXml();

        Assert.Equal("team", xml.Name.LocalName);
        Assert.Equal("7", xml.Attribute("id")?.Value);
        Assert.Equal("300", xml.Attribute("score")?.Value);
        Assert.Equal("5", xml.Attribute("errors")?.Value);
        Assert.Equal("1", xml.Attribute("place")?.Value);
    }

    [Fact]
    public void DefaultPlace_IsOne()
    {
        var result = new TeamResult(1, 100, 2);
        Assert.Equal(1, result.Place);
    }

    [Theory]
    [InlineData(1, 0, 0, 1)]
    [InlineData(5, 250, 10, 2)]
    [InlineData(999, 500, 0, 1)]
    public void RoundTrip_PreservesAllFields(int id, int score, int errors, int place)
    {
        var expected = new TeamResult(id, score, errors, place);
        var actual = TeamResult.FromXml(expected.ToXml());

        Assert.Equal(id, actual.TeamId);
        Assert.Equal(score, actual.Score);
        Assert.Equal(errors, actual.Errors);
        Assert.Equal(place, actual.Place);
    }
}
