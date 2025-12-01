namespace Reporting.Test.Models;

using System;
using System.Collections.Generic;

using Bogus;

using MatchMaker.Models;
using MatchMaker.Reporting.Models;

using Xunit;

public class QuizzerSummaryTests
{
    [Fact]
    public void QuizzerSummary_CreatedFromResult_HasCorrectQuizzerId()
    {
        var result = CreateTestResult();
        var quizzerSummaries = QuizzerSummary.FromResult(result);

        Assert.NotEmpty(quizzerSummaries);
        Assert.All(quizzerSummaries.Values, qs => Assert.True(qs.QuizzerId > 0));
    }

    [Fact]
    public void QuizzerSummary_CreatedFromResult_AllQuizzersPresent()
    {
        var result = CreateTestResult();
        var quizzerSummaries = QuizzerSummary.FromResult(result);

        Assert.Equal(result.Schedule.Quizzers.Count, quizzerSummaries.Count);
    }

    [Fact]
    public void QuizzerSummary_CreatedFromResult_HasScoreData()
    {
        var result = CreateTestResult();
        var quizzerSummaries = QuizzerSummary.FromResult(result);

        Assert.NotEmpty(quizzerSummaries);
        Assert.All(quizzerSummaries.Values, qs => Assert.True(qs.AverageScore >= 0));
    }

    [Fact]
    public void QuizzerSummary_CreatedFromResult_InitialPlaceSet()
    {
        var result = CreateTestResult();
        var quizzerSummaries = QuizzerSummary.FromResult(result);

        Assert.NotEmpty(quizzerSummaries);
        Assert.All(quizzerSummaries.Values, qs => Assert.True(qs.Place > 0));
    }

    private static Result CreateTestResult()
    {
        var faker = new Faker();

        var churches = new Dictionary<int, Church>
        {
            { 1, new Church(1, "Church 1") }
        };

        var teams = new Dictionary<int, Team>
        {
            { 1, new Team(1, "Team 1", "T1", 0) }
        };

        var quizzers = new Dictionary<int, Quizzer>
        {
            { 1, new Quizzer(1, faker.Name.FirstName(Bogus.DataSets.Name.Gender.Male), faker.Name.LastName(), Gender.Male, DateTime.Now.Year, 1, 1) },
            { 2, new Quizzer(2, faker.Name.FirstName(Bogus.DataSets.Name.Gender.Female), faker.Name.LastName(), Gender.Female, DateTime.Now.Year, 1, 1) }
        };

        var round = new Round(1, new Dictionary<int, MatchSchedule>(), DateOnly.FromDateTime(DateTime.Now), TimeOnly.FromDateTime(DateTime.Now));
        var rounds = new Dictionary<int, Round> { { 1, round } };

        var schedule = new Schedule("Test", churches, quizzers, teams, rounds);

        var teamResults = new List<TeamResult>
        {
            new(1, 100, 1, 1)
        };
        var quizzerResults = new List<QuizzerResult>
        {
            new(1, 60, 1),
            new(2, 40, 0)
        };
        var matchResult = new MatchResult(1, 1, 1, teamResults, quizzerResults);
        var matches = new Dictionary<int, MatchResult> { { 1, matchResult } };

        return new Result(schedule, matches);
    }
}
