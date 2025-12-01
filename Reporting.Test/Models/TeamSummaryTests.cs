namespace Reporting.Test.Models;

using System;
using System.Collections.Generic;

using Bogus;

using MatchMaker.Models;
using MatchMaker.Reporting.Models;
using MatchMaker.Reporting.Policies;

using Xunit;

public class TeamSummaryTests
{
    [Fact]
    public void TeamSummary_CreatedFromResult_HasCorrectTeamId()
    {
        var result = CreateTestResult();
        var teamSummaries = TeamSummary.FromResult(result, new List<TeamRankingPolicy>());

        Assert.NotEmpty(teamSummaries);
        Assert.All(teamSummaries.Values, ts => Assert.True(ts.TeamId > 0));
    }

    [Fact]
    public void TeamSummary_CreatedFromResult_HasWinsAndLosses()
    {
        var result = CreateTestResult();
        var teamSummaries = TeamSummary.FromResult(result, new List<TeamRankingPolicy>());

        Assert.NotEmpty(teamSummaries);
        Assert.All(teamSummaries.Values, ts => Assert.True(ts.Wins >= 0 || ts.Losses >= 0));
    }

    [Fact]
    public void TeamSummary_CreatedFromResult_AllTeamsPresent()
    {
        var result = CreateTestResult();
        var teamSummaries = TeamSummary.FromResult(result, new List<TeamRankingPolicy>());

        Assert.Equal(result.Schedule.Teams.Count, teamSummaries.Count);
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
            { 1, new Team(1, "Team 1", "T1", 0) },
            { 2, new Team(2, "Team 2", "T2", 0) }
        };

        var quizzers = new Dictionary<int, Quizzer>
        {
            { 1, new Quizzer(1, faker.Name.FirstName(Bogus.DataSets.Name.Gender.Male), faker.Name.LastName(), Gender.Male, DateTime.Now.Year, 1, 1) },
            { 2, new Quizzer(2, faker.Name.FirstName(Bogus.DataSets.Name.Gender.Female), faker.Name.LastName(), Gender.Female, DateTime.Now.Year, 2, 1) }
        };

        var round = new Round(1, new Dictionary<int, MatchSchedule>(), DateOnly.FromDateTime(DateTime.Now), TimeOnly.FromDateTime(DateTime.Now));
        var rounds = new Dictionary<int, Round> { { 1, round } };

        var schedule = new Schedule("Test", churches, quizzers, teams, rounds);

        var teamResults = new List<TeamResult>
        {
            new(1, 60, 0, 1),
            new(2, 40, 0, 2)
        };
        var quizzerResults = new List<QuizzerResult>
        {
            new(1, 60, 0),
            new(2, 40, 0)
        };
        var matchResult = new MatchResult(1, 1, 1, teamResults, quizzerResults);
        var matches = new Dictionary<int, MatchResult> { { 1, matchResult } };

        return new Result(schedule, matches);
    }
}
