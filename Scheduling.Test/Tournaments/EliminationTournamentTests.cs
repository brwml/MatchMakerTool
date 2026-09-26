namespace Scheduling.Test.Tournaments;

using System;
using System.Collections.Generic;
using System.Linq;

using MatchMaker.Models;
using MatchMaker.Scheduling.Tournaments;

using Xunit;

public class EliminationTournamentTests
{
    [Fact]
    public void Create_WithNullSchedule_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => EliminationTournament.Create(null!, [1, 2]));
    }

    [Fact]
    public void Create_WithNullSeededTeamIds_ThrowsArgumentNullException()
    {
        var schedule = CreateSchedule(4);
        Assert.Throws<ArgumentNullException>(() => EliminationTournament.Create(schedule, null!));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void Create_WithFewerThanTwoTeams_ThrowsArgumentOutOfRangeException(int count)
    {
        var schedule = CreateSchedule(4);
        var seededTeamIds = Enumerable.Range(1, count).ToList();

        Assert.Throws<ArgumentOutOfRangeException>(() => EliminationTournament.Create(schedule, seededTeamIds));
    }

    [Fact]
    public void Create_WithFourTeams_PairsStrongestAgainstWeakest()
    {
        var schedule = CreateSchedule(4);
        var result = EliminationTournament.Create(schedule, [1, 2, 3, 4]);

        var round = Assert.Single(result.Rounds).Value;
        var matches = round.Matches.Values.OrderBy(x => x.Id).ToList();

        Assert.Equal(2, matches.Count);
        Assert.Equal([1, 4], matches[0].Teams);
        Assert.Equal([2, 3], matches[1].Teams);
    }

    [Fact]
    public void Create_WithFourTeams_IncludesOnlySeededTeamsAndTheirQuizzers()
    {
        var schedule = CreateSchedule(6);
        var result = EliminationTournament.Create(schedule, [1, 2, 3, 4]);

        Assert.Equal(4, result.Teams.Count);
        Assert.Equal([1, 2, 3, 4], result.Teams.Keys.OrderBy(x => x));
        Assert.All(result.Quizzers.Values, x => Assert.True(x.TeamId <= 4));
        Assert.Equal(schedule.Quizzers.Count(x => x.Value.TeamId <= 4), result.Quizzers.Count);
    }

    [Fact]
    public void Create_WithOddNumberOfTeams_TopSeedReceivesBye()
    {
        var schedule = CreateSchedule(5);
        var result = EliminationTournament.Create(schedule, [1, 2, 3, 4, 5]);

        var round = Assert.Single(result.Rounds).Value;

        Assert.Equal(2, round.Matches.Count);
        Assert.Equal(1, result.GetByeTeamId(round));

        var teamsInMatches = round.Matches.Values.SelectMany(x => x.Teams).ToHashSet();
        Assert.DoesNotContain(1, teamsInMatches);
        Assert.Equal(new HashSet<int> { 2, 3, 4, 5 }, teamsInMatches);
    }

    [Fact]
    public void Create_AssignsSequentialRoomsStartingAtOne()
    {
        var schedule = CreateSchedule(8);
        var result = EliminationTournament.Create(schedule, [1, 2, 3, 4, 5, 6, 7, 8]);

        var rooms = result.Rounds.Single().Value.Matches.Values.Select(x => x.Room).OrderBy(x => x).ToList();
        Assert.Equal([1, 2, 3, 4], rooms);
    }

    [Fact]
    public void Create_WithoutStartDateOrTime_UsesCurrentDateAndTime()
    {
        var schedule = CreateSchedule(4);
        var before = DateTime.Now;
        var result = EliminationTournament.Create(schedule, [1, 2, 3, 4]);
        var after = DateTime.Now;

        var round = result.Rounds.Single().Value;

        Assert.InRange(round.Date, DateOnly.FromDateTime(before), DateOnly.FromDateTime(after));
    }

    [Fact]
    public void Create_WithExplicitStartDateAndTime_UsesGivenValues()
    {
        var schedule = CreateSchedule(4);
        var date = new DateOnly(2025, 6, 1);
        var time = new TimeOnly(14, 30);

        var result = EliminationTournament.Create(schedule, [1, 2, 3, 4], date, time);
        var round = result.Rounds.Single().Value;

        Assert.Equal(date, round.Date);
        Assert.Equal(time, round.Time);
    }

    private static Schedule CreateSchedule(int numberOfTeams)
    {
        var teams = Enumerable.Range(1, numberOfTeams).ToDictionary(x => x, x => new Team(x, $"Team {x}", x.ToString(System.Globalization.CultureInfo.InvariantCulture), 0));
        var churches = new Dictionary<int, Church> { { 1, new Church(1, "Church 1") } };

        var quizzers = new Dictionary<int, Quizzer>();
        var quizzerId = 1;

        foreach (var teamId in teams.Keys)
        {
            for (var i = 0; i < 3; i++)
            {
                quizzers.Add(quizzerId, new Quizzer(quizzerId, $"First{quizzerId}", $"Last{quizzerId}", Gender.Male, DateTime.Now.Year, teamId, 1));
                quizzerId++;
            }
        }

        return new Schedule("Test Tournament", churches, quizzers, teams, new Dictionary<int, Round>());
    }
}
