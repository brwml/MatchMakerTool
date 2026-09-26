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

    [Fact]
    public void Summary_ByDefault_HasZeroEliminationTeams()
    {
        var result = CreateTestResult("Tournament 1");
        var policies = new TeamRankingPolicy[] { new NullTeamRankingPolicy() };

        var summary = Summary.FromResult(result, policies);

        Assert.Equal(0, summary.NumberOfEliminationTeams);
    }

    [Fact]
    public void Summary_NumberOfEliminationTeams_CanBeSet()
    {
        var result = CreateTestResult("Tournament 1");
        var policies = new TeamRankingPolicy[] { new NullTeamRankingPolicy() };
        var summary = Summary.FromResult(result, policies);

        summary.NumberOfEliminationTeams = 1;

        Assert.Equal(1, summary.NumberOfEliminationTeams);
    }

    [Fact]
    public void Summary_EliminationTeamIds_ByDefault_IsEmpty()
    {
        var result = CreateTestResult("Tournament 1");
        var policies = new TeamRankingPolicy[] { new NullTeamRankingPolicy() };
        var summary = Summary.FromResult(result, policies);

        Assert.Empty(summary.EliminationTeamIds);
    }

    [Fact]
    public void Summary_EliminationTeamIds_SelectsExactlyTheRequestedCount()
    {
        var result = CreateTestResult("Tournament 1", numberOfTeams: 4);
        var policies = new TeamRankingPolicy[] { new WinPercentageTeamRankingPolicy() };
        var summary = Summary.FromResult(result, policies);

        summary.NumberOfEliminationTeams = 3;

        Assert.Equal(3, summary.EliminationTeamIds.Count);
    }

    [Fact]
    public void Summary_EliminationTeamIds_WhenPlacesAreTied_BreaksTiesByTeamId()
    {
        // NullTeamRankingPolicy assigns the same Place to every team, so all 4 teams here are
        // tied. The selection must still return exactly 2 teams, chosen deterministically.
        var result = CreateTestResult("Tournament 1", numberOfTeams: 4);
        var policies = new TeamRankingPolicy[] { new NullTeamRankingPolicy() };
        var summary = Summary.FromResult(result, policies);

        summary.NumberOfEliminationTeams = 2;

        Assert.Equal([1, 2], summary.EliminationTeamIds);
    }

    private static Result CreateTestResult(string name, int numberOfTeams = 2)
    {
        var faker = new Faker();

        var churches = Enumerable.Range(1, numberOfTeams).ToDictionary(x => x, x => new Church(x, $"Church {x}"));
        var teams = Enumerable.Range(1, numberOfTeams).ToDictionary(x => x, x => new Team(x, $"Team {x}", $"T{x}", 0));

        var quizzers = Enumerable.Range(1, numberOfTeams).ToDictionary(
            x => x,
            x => new Quizzer(x, faker.Name.FirstName(), faker.Name.LastName(), Gender.Male, DateTime.Now.Year, x, x));

        var round = new Round(1, new Dictionary<int, MatchSchedule>(), DateOnly.FromDateTime(DateTime.Now), TimeOnly.FromDateTime(DateTime.Now));
        var rounds = new Dictionary<int, Round> { { 1, round } };

        var schedule = new Schedule(name, churches, quizzers, teams, rounds);

        // Descending scores/errors so ranking policies that differentiate by score (for example
        // WinPercentageTeamRankingPolicy) produce a strict order across every team.
        var teamResults = Enumerable.Range(1, numberOfTeams)
            .Select(x => new TeamResult(x, 100 - x, x - 1, 1))
            .ToList();
        var quizzerResults = Enumerable.Range(1, numberOfTeams)
            .Select(x => new QuizzerResult(x, 100 - x, x - 1))
            .ToList();

        var matchResult = new MatchResult(1, 1, 1, teamResults, quizzerResults);
        var matches = new Dictionary<int, MatchResult> { { 1, matchResult } };

        return new Result(schedule, matches);
    }
}
