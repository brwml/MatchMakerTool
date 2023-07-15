namespace Reporting.Test.Models;

using System.Collections.Generic;

using Bogus;

using MatchMaker.Models;

using Xunit;

public class ChurchTests
{
    [Theory]
    [MemberData(nameof(ChurchTestData))]
    public void VerifyXmlConversion(Church expected)
    {
        var actual = Church.FromXml(expected.ToXml());
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Name, actual.Name);
    }

    public static IEnumerable<object[]> ChurchTestData()
    {
        var faker = new Faker();

        for (var i = 0; i < 100; i++)
        {
            yield return new object[] { new Church(faker.Random.Int(), faker.Random.String(faker.Random.Int(10, 100))) };
        }
    }
}
