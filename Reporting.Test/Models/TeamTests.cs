namespace Reporting.Test.Models;

using Bogus;

using MatchMaker.Models;

using Xunit;

public class TeamTests
{
    [Fact]
    public void VerifyXmlConversion()
    {
        var faker = new Faker();

        var expected = new Team(
            faker.Random.Int(1, 9999),
            faker.Random.String2(faker.Random.Int(10, 50)),
            faker.Random.String2(faker.Random.Int(2, 5)),
            faker.Random.Int(1, 10));

        var actual = Team.FromXml(expected.ToXml());

        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Abbreviation, actual.Abbreviation);
        Assert.Equal(expected.Division, actual.Division);
    }

    [Fact]
    public void ToXml_ProducesTeamElement()
    {
        var team = new Team(42, "Test Team", "TT", 1);
        var xml = team.ToXml();

        Assert.Equal("team", xml.Name.LocalName);
        Assert.Equal("42", xml.Attribute("id")?.Value);
        Assert.Equal("1", xml.Attribute("div")?.Value);
        Assert.Equal("TT", xml.Attribute("abbrev")?.Value);
        Assert.Equal("Test Team", xml.Value);
    }

    [Theory]
    [InlineData(1, "Alpha", "A", 0)]
    [InlineData(100, "Beta Team", "BT", 3)]
    [InlineData(9999, "Gamma", "GG", 99)]
    public void RoundTrip_PreservesAllFields(int id, string name, string abbrev, int division)
    {
        var expected = new Team(id, name, abbrev, division);
        var actual = Team.FromXml(expected.ToXml());

        Assert.Equal(id, actual.Id);
        Assert.Equal(name, actual.Name);
        Assert.Equal(abbrev, actual.Abbreviation);
        Assert.Equal(division, actual.Division);
    }
}
