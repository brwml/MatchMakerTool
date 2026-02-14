namespace Reporting.Test.Models;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Bogus;

using MatchMaker.Models;

using Xunit;

public class ScheduleTests
{
    [Theory]
    [MemberData(nameof(GetScheduleTests))]
    [SuppressMessage("Usage", "xUnit1045:Avoid using TheoryData type arguments that might not be serializable", Justification = "<Pending>")]
    public void Test(Schedule expected)
    {
        var actual = Schedule.FromXml(expected.ToXml(), expected.Name);
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Teams.Select(x => x.Key), actual.Teams.Select(x => x.Key));
        Assert.Equal(expected.Quizzers.Select(x => x.Key), actual.Quizzers.Select(x => x.Key));
        Assert.Equal(expected.Churches.Select(x => x.Key), actual.Churches.Select(x => x.Key));
        Assert.Equal(expected.Rounds.Select(x => x.Key), actual.Rounds.Select(x => x.Key));
    }

    public static TheoryData<Schedule> GetScheduleTests()
    {
        var data = new TheoryData<Schedule>();

        for (var i = 0; i < 100; i++)
        {
            var faker = new Faker();

            var churches = new[]
            {
                new Church(faker.Random.Int(), faker.Random.String2(faker.Random.Int(10, 100))),
                new Church(faker.Random.Int(), faker.Random.String2(faker.Random.Int(10, 100))),
            };

            var teams = new[]
            {
                new Team(faker.Random.Int(), faker.Random.String2(faker.Random.Int(10, 100)), faker.Random.String2(1), faker.Random.Int()),
                new Team(faker.Random.Int(), faker.Random.String2(faker.Random.Int(10, 100)), faker.Random.String2(1), faker.Random.Int()),
            };

            var quizzers = new[]
            {
                new Quizzer(faker.Random.Int(), faker.Random.String2(faker.Random.Int(10, 100)), faker.Random.String2(faker.Random.Int(10, 100)), faker.Random.Enum(Gender.Unknown), faker.Random.Int(), faker.Random.ArrayElement(teams).Id, faker.Random.ArrayElement(churches).Id),
                new Quizzer(faker.Random.Int(), faker.Random.String2(faker.Random.Int(10, 100)), faker.Random.String2(faker.Random.Int(10, 100)), faker.Random.Enum(Gender.Unknown), faker.Random.Int(), faker.Random.ArrayElement(teams).Id, faker.Random.ArrayElement(churches).Id),
                new Quizzer(faker.Random.Int(), faker.Random.String2(faker.Random.Int(10, 100)), faker.Random.String2(faker.Random.Int(10, 100)), faker.Random.Enum(Gender.Unknown), faker.Random.Int(), faker.Random.ArrayElement(teams).Id, faker.Random.ArrayElement(churches).Id),
                new Quizzer(faker.Random.Int(), faker.Random.String2(faker.Random.Int(10, 100)), faker.Random.String2(faker.Random.Int(10, 100)), faker.Random.Enum(Gender.Unknown), faker.Random.Int(), faker.Random.ArrayElement(teams).Id, faker.Random.ArrayElement(churches).Id),
                new Quizzer(faker.Random.Int(), faker.Random.String2(faker.Random.Int(10, 100)), faker.Random.String2(faker.Random.Int(10, 100)), faker.Random.Enum(Gender.Unknown), faker.Random.Int(), faker.Random.ArrayElement(teams).Id, faker.Random.ArrayElement(churches).Id),
                new Quizzer(faker.Random.Int(), faker.Random.String2(faker.Random.Int(10, 100)), faker.Random.String2(faker.Random.Int(10, 100)), faker.Random.Enum(Gender.Unknown), faker.Random.Int(), faker.Random.ArrayElement(teams).Id, faker.Random.ArrayElement(churches).Id),
                new Quizzer(faker.Random.Int(), faker.Random.String2(faker.Random.Int(10, 100)), faker.Random.String2(faker.Random.Int(10, 100)), faker.Random.Enum(Gender.Unknown), faker.Random.Int(), faker.Random.ArrayElement(teams).Id, faker.Random.ArrayElement(churches).Id),
                new Quizzer(faker.Random.Int(), faker.Random.String2(faker.Random.Int(10, 100)), faker.Random.String2(faker.Random.Int(10, 100)), faker.Random.Enum(Gender.Unknown), faker.Random.Int(), faker.Random.ArrayElement(teams).Id, faker.Random.ArrayElement(churches).Id),
                new Quizzer(faker.Random.Int(), faker.Random.String2(faker.Random.Int(10, 100)), faker.Random.String2(faker.Random.Int(10, 100)), faker.Random.Enum(Gender.Unknown), faker.Random.Int(), faker.Random.ArrayElement(teams).Id, faker.Random.ArrayElement(churches).Id),
                new Quizzer(faker.Random.Int(), faker.Random.String2(faker.Random.Int(10, 100)), faker.Random.String2(faker.Random.Int(10, 100)), faker.Random.Enum(Gender.Unknown), faker.Random.Int(), faker.Random.ArrayElement(teams).Id, faker.Random.ArrayElement(churches).Id),
            };

            var matches = new[]
            {
                new MatchSchedule(faker.Random.Int(), faker.Random.Int(), [.. teams.Select(x => x.Id)])
            };

            var rounds = new[]
            {
                new Round(faker.Random.Int(), matches.ToDictionary(x => x.Id, x => x), DateOnly.FromDateTime(faker.Date.Future()), TimeOnly.FromDateTime(faker.Date.Future()))
            };

            data.Add(
                new Schedule(
                    faker.Random.String2(faker.Random.Int(10, 100)),
                    churches.ToDictionary(x => x.Id, x => x),
                    quizzers.ToDictionary(x => x.Id, x => x),
                    teams.ToDictionary(x => x.Id, x => x),
                    rounds.ToDictionary(x => x.Id, x => x)));
        }

        return data;
    }
}
