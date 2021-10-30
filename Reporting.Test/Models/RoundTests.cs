namespace Reporting.Test.Models;

using System;
using System.Collections.Generic;
using System.Xml.Linq;

using MatchMaker.Reporting.Models;

using Xunit;

public class RoundTests
{
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
