namespace Reporting.Test.Models;

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

    public static TheoryData<Church> ChurchTestData()
    {
        var faker = new Faker();
        var data = new TheoryData<Church>();

        for (var i = 0; i < 100; i++)
        {
            data.Add(new Church(faker.Random.Int(), faker.Random.String(faker.Random.Int(10, 100))));
        }

        return data;
    }
}
