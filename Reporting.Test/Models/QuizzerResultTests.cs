namespace Reporting.Test.Models;

using Bogus;

using MatchMaker.Models;

using Xunit;

public class QuizzerResultTests
{
    [Fact]
    public void VerifyXmlConversion()
    {
        var faker = new Faker();

        var expected = new QuizzerResult(
            faker.Random.Int(1, 9999),
            faker.Random.Int(0, 300),
            faker.Random.Int(0, 20));

        var actual = QuizzerResult.FromXml(expected.ToXml());

        Assert.Equal(expected.QuizzerId, actual.QuizzerId);
        Assert.Equal(expected.Score, actual.Score);
        Assert.Equal(expected.Errors, actual.Errors);
    }

    [Fact]
    public void ToXml_ProducesQuizzerElement()
    {
        var result = new QuizzerResult(12, 80, 3);
        var xml = result.ToXml();

        Assert.Equal("quizzer", xml.Name.LocalName);
        Assert.Equal("12", xml.Attribute("id")?.Value);
        Assert.Equal("80", xml.Attribute("score")?.Value);
        Assert.Equal("3", xml.Attribute("errors")?.Value);
    }

    [Theory]
    [InlineData(1, 0, 0)]
    [InlineData(42, 120, 5)]
    [InlineData(999, 300, 20)]
    public void RoundTrip_PreservesAllFields(int id, int score, int errors)
    {
        var expected = new QuizzerResult(id, score, errors);
        var actual = QuizzerResult.FromXml(expected.ToXml());

        Assert.Equal(id, actual.QuizzerId);
        Assert.Equal(score, actual.Score);
        Assert.Equal(errors, actual.Errors);
    }
}
