namespace Reporting.Test.Models;

using System;
using System.Collections.Generic;
using System.Xml.Linq;

using Bogus;

using MatchMaker.Reporting.Models;

using Xunit;

public class RoundTests
{
    [Theory]
    [MemberData(nameof(CreateXmlTests))]
    public void VerifyXml(Round expected)
    {
        var actual = Round.FromXml(expected.ToXml());
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Date, actual.Date);
        Assert.True(actual.Time.IsBetween(expected.Time.AddMinutes(-1), expected.Time.AddMinutes(1)));

        foreach (var match in actual.Matches)
        {
            Assert.True(expected.Matches.ContainsKey(match.Key));

            Assert.Equal(expected.Matches[match.Key].Id, match.Value.Id);
            Assert.Equal(expected.Matches[match.Key].Room, match.Value.Room);
            Assert.Equal(expected.Matches[match.Key].Teams, match.Value.Teams);
        }
    }

    [Theory]
    [MemberData(nameof(CreateRoundTests))]
    public void Verify(XElement xml, int id, DateOnly date, TimeOnly time)
    {
        var round = Round.FromXml(xml);

        Assert.Equal(id, round.Id);
        Assert.Equal(date, round.Date);
        Assert.True(round.Time.IsBetween(time.AddMinutes(-0.1), time.AddMinutes(0.1)));
    }

    public static IEnumerable<object[]> CreateRoundTests()
    {
        return new[]
        {
            new object[]{ Case1, 1, new DateOnly(2021, 11, 1), new TimeOnly(17, 55) },
            new object[]{ Case2, 1, DateOnly.FromDateTime(DateTime.Now), TimeOnly.FromDateTime(DateTime.Now) }
        };
    }

    public static IEnumerable<object[]> CreateXmlTests()
    {
        var faker = new Faker();

        for (var i = 0; i < 100; i++)
        {
            var matches = new[]
            {
                new MatchSchedule(faker.Random.Int(), faker.Random.Int(), new List<int>{ faker.Random.Int(), faker.Random.Int() }),
                new MatchSchedule(faker.Random.Int(), faker.Random.Int(), new List<int>{ faker.Random.Int(), faker.Random.Int() }),
                new MatchSchedule(faker.Random.Int(), faker.Random.Int(), new List<int>{ faker.Random.Int(), faker.Random.Int() }),
                new MatchSchedule(faker.Random.Int(), faker.Random.Int(), new List<int>{ faker.Random.Int(), faker.Random.Int() })
            };

            yield return new object[]
            {
                new Round(i, matches.ToDictionary(x => x.Id, y => y), DateOnly.FromDateTime(faker.Date.Future()), TimeOnly.FromDateTime(faker.Date.Future()))
            };
        }
    }

    private static XElement Case1 => new(
        "round",
        new XAttribute("id", 1),
        new XAttribute("date", "2021-11-01"),
        new XAttribute("time", "17:55"));

    private static XElement Case2 => new(
        "round",
        new XAttribute("id", 1),
        new XAttribute("date", "-1"),
        new XAttribute("time", "-1"));
}
