namespace Reporting.Test.Models;

using System;
using System.Collections.Generic;
using System.Linq;

using Bogus;

using MatchMaker.Models;
using MatchMaker.Reporting.Models;
using MatchMaker.Reporting.Policies;

using Xunit;

public class SummaryTests
{
    [Fact]
    public void Summary_CreatedFromResult_HasCorrectName()
    {
        var result = CreateTestResult("Tournament 1");
        var policies = new TeamRankingPolicy[] { new NullTeamRankingPolicy() };

        var summary = Summary.FromResult(result, policies);

        Assert.Equal("Tournament 1", summary.Name);
    }

    [Fact]
    public void Summary_CreatedFromResult_HasTeamSummaries()
    {
        var result = CreateTestResult("Tournament 1");
        var policies = new TeamRankingPolicy[] { new NullTeamRankingPolicy() };

        var summary = Summary.FromResult(result, policies);

        Assert.NotEmpty(summary.TeamSummaries);
    }

    [Fact]
    public void Summary_CreatedFromResult_HasQuizzerSummaries()
    {
        var result = CreateTestResult("Tournament 1");
        var policies = new TeamRankingPolicy[] { new NullTeamRankingPolicy() };

        var summary = Summary.FromResult(result, policies);

        Assert.NotEmpty(summary.QuizzerSummaries);
    }

    [Fact]
    public void Summary_ToXml_ReturnsValidXDocument()
    {
        var result = CreateTestResult("Tournament 1");
        var policies = new TeamRankingPolicy[] { new NullTeamRankingPolicy() };
        var summary = Summary.FromResult(result, policies);

        var xmlDoc = summary.ToXml();

        Assert.NotNull(xmlDoc);
        Assert.NotNull(xmlDoc.Root);
    }

    [Fact]
    public void Summary_WithMultiplePolicies_AppliesAllPolicies()
    {
        var result = CreateTestResult("Tournament 1");
        var policies = new TeamRankingPolicy[] 
        { 
            new NullTeamRankingPolicy(),
            new ErrorTeamRankingPolicy()
        };

        var summary = Summary.FromResult(result, policies);

        Assert.NotEmpty(summary.TeamSummaries);
        foreach (var teamSummary in summary.TeamSummaries.Values)
        {
            Assert.NotNull(teamSummary);
        }
    }

    private static Result CreateTestResult(string name)
    {
        var faker = new Faker();

        var churches = new Dictionary<int, Church>
        {
            { 1, new Church(1, "Church 1") },
            { 2, new Church(2, "Church 2") }
        };

        var teams = new Dictionary<int, Team>
        {
            { 1, new Team(1, "Team 1", "T1", 0) },
            { 2, new Team(2, "Team 2", "T2", 0) }
        };

        var quizzers = new Dictionary<int, Quizzer>
        {
            { 1, new Quizzer(1, faker.Name.FirstName(Bogus.DataSets.Name.Gender.Male), faker.Name.LastName(), Gender.Male, DateTime.Now.Year, 1, 1) },
            { 2, new Quizzer(2, faker.Name.FirstName(Bogus.DataSets.Name.Gender.Female), faker.Name.LastName(), Gender.Female, DateTime.Now.Year, 2, 2) }
        };

        var round = new Round(1, new Dictionary<int, MatchSchedule>(), DateOnly.FromDateTime(DateTime.Now), TimeOnly.FromDateTime(DateTime.Now));
        var rounds = new Dictionary<int, Round> { { 1, round } };

        var schedule = new Schedule(name, churches, quizzers, teams, rounds);

        var teamResults = new List<TeamResult>
        {
            new(1, 90, 0, 1),
            new(2, 80, 1, 2)
        };
        var quizzerResults = new List<QuizzerResult>
        {
            new(1, 90, 0),
            new(2, 80, 1)
        };
        var matchResult = new MatchResult(1, 1, 1, teamResults, quizzerResults);
        var matches = new Dictionary<int, MatchResult> { { 1, matchResult } };

        return new Result(schedule, matches);
    }
}
