namespace Reporting.Test.Models;

using System.Collections.Generic;

using Bogus;

using MatchMaker.Models;

using Xunit;

public class MatchScheduleTests
{
    [Fact]
    public void VerifyXmlConversion()
    {
        var faker = new Faker();

        var expected = new MatchSchedule(faker.Random.Int(), faker.Random.Int(), [faker.Random.Int(), faker.Random.Int()]);
        var actual = MatchSchedule.FromXml(expected.ToXml());

        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Room, expected.Room);
        Assert.Equal(expected.Teams, actual.Teams);
    }
}
