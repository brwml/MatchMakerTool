namespace Reporting.Test.Models;

using Bogus;

using MatchMaker.Models;

using Xunit;

public class ChurchTests
{
    [Fact]
    public void VerifyXmlConversion()
    {
        var faker = new Faker();

        var expected = new Church(faker.Random.Int(), faker.Random.String(faker.Random.Int(10, 100)));
        var actual = Church.FromXml(expected.ToXml());

        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Name, actual.Name);
    }
}
