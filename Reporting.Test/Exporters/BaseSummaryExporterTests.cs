namespace Reporting.Test.Exporters;

using System;
using System.Collections.Generic;
using System.Linq;

using Bogus;

using MatchMaker.Models;
using MatchMaker.Reporting.Exporters;
using MatchMaker.Reporting.Models;
using MatchMaker.Reporting.Policies;

using Xunit;

public class BaseSummaryExporterTests
{
    [Fact]
    public void GetTeamInfo_WithZeroEliminationTeams_MarksNoTeamsAsElimination()
    {
        var summary = CreateTestSummary("Test Tournament");
        summary.NumberOfEliminationTeams = 0;

        var teams = TestExporter.GetTeamInfoForTest(summary).ToArray();

        Assert.All(teams, x => Assert.False(x.IsElimination));
    }

    [Fact]
    public void GetTeamInfo_WithPositiveEliminationTeams_MarksOnlyQualifyingTeams()
    {
        var summary = CreateTestSummary("Test Tournament");
        summary.NumberOfEliminationTeams = 1;

        var teams = TestExporter.GetTeamInfoForTest(summary).ToArray();

        Assert.Single(teams, x => x.IsElimination);
        Assert.Equal(1, teams.Single(x => x.IsElimination).Place);
    }

    [Fact]
    public void GetTeamInfo_WithEliminationTeamsExceedingTeamCount_MarksAllTeams()
    {
        var summary = CreateTestSummary("Test Tournament");
        summary.NumberOfEliminationTeams = 100;

        var teams = TestExporter.GetTeamInfoForTest(summary).ToArray();

        Assert.All(teams, x => Assert.True(x.IsElimination));
    }

    [Fact]
    public void GetTeamInfo_WithTiedPlaces_MarksExactlyTheRequestedCount()
    {
        // All 4 teams tie for first place, so a naive "Place <= N" comparison would mark every
        // team as elimination-qualifying. IsElimination must instead agree with
        // Summary.EliminationTeamIds, which breaks ties deterministically and always selects
        // exactly N teams.
        var summary = CreateTestSummary("Test Tournament", numberOfTeams: 4, useTiedPlaces: true);
        summary.NumberOfEliminationTeams = 2;

        var teams = TestExporter.GetTeamInfoForTest(summary).ToArray();

        Assert.Equal(2, teams.Count(x => x.IsElimination));
        Assert.Equal(
            summary.EliminationTeamIds.OrderBy(x => x),
            teams.Where(x => x.IsElimination).Select(x => x.Id).OrderBy(x => x));
    }

    private static Summary CreateTestSummary(string name, int numberOfTeams = 2, bool useTiedPlaces = false)
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

        // Every team result uses place = 1 (a win) so every team ties under a null/win-based
        // policy, exercising the tie-break path in Summary.EliminationTeamIds.
        var teamResults = Enumerable.Range(1, numberOfTeams).Select(x => new TeamResult(x, 100 - x, x - 1, 1)).ToList();
        var quizzerResults = Enumerable.Range(1, numberOfTeams).Select(x => new QuizzerResult(x, 100 - x, x - 1)).ToList();
        var matchResult = new MatchResult(1, 1, 1, teamResults, quizzerResults);
        var matches = new Dictionary<int, MatchResult> { { 1, matchResult } };

        var result = new Result(schedule, matches);
        var policies = useTiedPlaces
            ? [new NullTeamRankingPolicy()]
            : new TeamRankingPolicy[] { new WinPercentageTeamRankingPolicy() };

        return Summary.FromResult(result, policies);
    }

    private sealed class TestExporter : BaseSummaryExporter
    {
        public override void Export(Summary summary, string folder)
        {
            throw new NotSupportedException("This class exists only to expose protected members for testing.");
        }

        public static IEnumerable<TeamInfo> GetTeamInfoForTest(Summary summary)
        {
            return GetTeamInfo(summary);
        }
    }
}
