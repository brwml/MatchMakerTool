namespace Reporting.Test.Models;

using System.Collections.Generic;

using Bogus;

using MatchMaker.Models;

using Xunit;

public class MatchScheduleTests
{
    [Theory]
    [MemberData(nameof(MatchSchedules))]
    public void VerifyXmlConversion(MatchSchedule expected)
    {
        var actual = MatchSchedule.FromXml(expected.ToXml());
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Room, expected.Room);
        Assert.Equal(expected.Teams, actual.Teams);
    }

    public static TheoryData<MatchSchedule> MatchSchedules()
    {
        var data = new TheoryData<MatchSchedule>();
        var faker = new Faker();

        for (var i = 0; i < 100; i++)
        {
            data.Add(
                new MatchSchedule(faker.Random.Int(), faker.Random.Int(), new List<int> { faker.Random.Int(), faker.Random.Int() }));
        }

        return data;
    }
}
