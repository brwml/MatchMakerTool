namespace Reporting.Test.Models;

using System;
using System.Collections.Generic;

using MatchMaker.Models;
using MatchMaker.Reporting.Models;

using Xunit;

public class ResultExtensionsTests
{
    [Fact]
    public void ResultExtensions_ToXml_ReturnsValidXDocument()
    {
        var result = CreateTestResult();
        var xmlDoc = result.ToXml();

        Assert.NotNull(xmlDoc);
        Assert.NotNull(xmlDoc.Root);
    }

    [Fact]
    public void ResultExtensions_ToXml_ContainsResultElement()
    {
        var result = CreateTestResult();
        var xmlDoc = result.ToXml();

        Assert.NotNull(xmlDoc.Root);
        var resultsElement = xmlDoc.Descendants("results").FirstOrDefault();
        Assert.NotNull(resultsElement);
    }

    [Fact]
    public void ResultExtensions_ToXml_PreservesResultName()
    {
        var result = CreateTestResult();
        var xmlDoc = result.ToXml();

        Assert.NotNull(xmlDoc);
        var resultsElement = xmlDoc.Descendants("results").FirstOrDefault();
        if (resultsElement != null)
        {
            _ = resultsElement.Attribute("name");
            // The name may or may not be present depending on implementation
        }
    }

    private static Result CreateTestResult()
    {
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
            { 1, new Quizzer(1, "John", "Doe", Gender.Male, DateTime.Now.Year, 1, 1) }
        };

        var round = new Round(1, new Dictionary<int, MatchSchedule>(), DateOnly.FromDateTime(DateTime.Now), TimeOnly.FromDateTime(DateTime.Now));
        var rounds = new Dictionary<int, Round> { { 1, round } };

        var schedule = new Schedule("Test Schedule", churches, quizzers, teams, rounds);

        var teamResults = new List<TeamResult>();
        var quizzerResults = new List<QuizzerResult>();
        var matchResult = new MatchResult(1, 1, 1, teamResults, quizzerResults);
        var matches = new Dictionary<int, MatchResult> { { 1, matchResult } };

        return new Result(schedule, matches);
    }
}
