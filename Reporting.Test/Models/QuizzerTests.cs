﻿namespace Reporting.Test.Models;

using System.Collections.Generic;

using Bogus;

using MatchMaker.Reporting.Models;

using Xunit;

public class QuizzerTests
{
    [Theory]
    [MemberData(nameof(QuizzerTestData))]
    public void VerifyXmlConversion(Quizzer expected)
    {
        var actual = Quizzer.FromXml(expected.ToXml());
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.FirstName.Trim(), actual.FirstName);
        Assert.Equal(expected.LastName.Trim(), actual.LastName);
        Assert.Equal(expected.Gender, actual.Gender);
        Assert.Equal(expected.TeamId, actual.TeamId);
        Assert.Equal(expected.ChurchId, actual.ChurchId);
        Assert.Equal(expected.RookieYear, actual.RookieYear);
    }

    public static IEnumerable<object[]> QuizzerTestData()
    {
        var faker = new Faker();

        for (var i = 0; i < 100; i++)
        {
            yield return new object[] { new Quizzer(faker.Random.Int(), faker.Random.String(10, 20), faker.Random.String(10, 20), faker.Random.Enum(Gender.Unknown), faker.Random.Int(), faker.Random.Int(), faker.Random.Int()) };
        }
    }
}