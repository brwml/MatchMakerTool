namespace Scheduling.Test.Tournaments;

using System;
using System.Collections.Generic;
using System.Linq;

using MatchMaker.Models;
using MatchMaker.Scheduling.Tournaments;

using Xunit;

public class TournamentRoundGeneratorFactoryTests
{
    [Fact]
    public void For_RoundRobin_ReturnsRoundRobinRoundGenerator()
    {
        Assert.IsType<RoundRobinRoundGenerator>(TournamentRoundGeneratorFactory.For(TournamentType.RoundRobin));
    }

    [Fact]
    public void For_SingleElimination_ReturnsSingleEliminationRoundGenerator()
    {
        Assert.IsType<SingleEliminationRoundGenerator>(TournamentRoundGeneratorFactory.For(TournamentType.SingleElimination));
    }

    [Fact]
    public void For_DoubleElimination_ReturnsDoubleEliminationRoundGenerator()
    {
        Assert.IsType<DoubleEliminationRoundGenerator>(TournamentRoundGeneratorFactory.For(TournamentType.DoubleElimination));
    }

    [Fact]
    public void For_TripleElimination_ReturnsTripleEliminationRoundGenerator()
    {
        Assert.IsType<TripleEliminationRoundGenerator>(TournamentRoundGeneratorFactory.For(TournamentType.TripleElimination));
    }

    [Fact]
    public void For_Swiss_ReturnsSwissRoundGenerator()
    {
        Assert.IsType<SwissRoundGenerator>(TournamentRoundGeneratorFactory.For(TournamentType.Swiss));
    }

    [Fact]
    public void For_SwissWithTopCut_ReturnsSwissWithTopCutRoundGenerator()
    {
        Assert.IsType<SwissWithTopCutRoundGenerator>(TournamentRoundGeneratorFactory.For(TournamentType.SwissWithTopCut));
    }

    [Fact]
    public void For_UnknownType_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TournamentRoundGeneratorFactory.For((TournamentType)(-1)));
    }

    [Fact]
    public void RoundRobinRoundGenerator_IsComplete_AlwaysReturnsTrue()
    {
        var generator = new RoundRobinRoundGenerator();
        var schedule = RoundRobinTournament.Create(RoundRobinTournamentTests.CreateSchedule(4), 2);

        Assert.True(generator.IsComplete(schedule, Result.Null));
    }

    [Fact]
    public void RoundRobinRoundGenerator_CreateNextRound_AlwaysReturnsNull()
    {
        var generator = new RoundRobinRoundGenerator();
        var schedule = RoundRobinTournament.Create(RoundRobinTournamentTests.CreateSchedule(4), 2);

        Assert.Null(generator.CreateNextRound(schedule, Result.Null));
    }

    [Fact]
    public void RoundRobinRoundGenerator_WithNullArguments_ThrowsArgumentNullException()
    {
        var generator = new RoundRobinRoundGenerator();
        var schedule = RoundRobinTournament.Create(RoundRobinTournamentTests.CreateSchedule(4), 2);

        Assert.Throws<ArgumentNullException>(() => generator.IsComplete(null!, Result.Null));
        Assert.Throws<ArgumentNullException>(() => generator.IsComplete(schedule, null!));
        Assert.Throws<ArgumentNullException>(() => generator.CreateNextRound(null!, Result.Null));
        Assert.Throws<ArgumentNullException>(() => generator.CreateNextRound(schedule, null!));
    }

    [Fact]
    public void SingleEliminationRoundGenerator_CreateNextRound_DelegatesToEliminationTournamentAdvanceRound()
    {
        var generator = new SingleEliminationRoundGenerator();
        var schedule = EliminationTournament.Create(EliminationTournamentTests.CreateSchedule(4), [1, 2, 3, 4]);
        var round = schedule.Rounds.Single().Value;

        var teamResults = new List<TeamResult> { new(1, 10, 0, 1), new(4, 0, 0, 2) };
        var teamResults2 = new List<TeamResult> { new(2, 10, 0, 1), new(3, 0, 0, 2) };
        var matches = new[]
        {
            new MatchResult(0, round.Matches[1].Room, round.Id, teamResults, []),
            new MatchResult(0, round.Matches[2].Room, round.Id, teamResults2, []),
        }.ToDictionary(m => m.ScheduleId, m => m);
        var result = new Result(schedule, matches);

        Assert.False(generator.IsComplete(schedule, result));

        var nextRound = generator.CreateNextRound(schedule, result);

        Assert.NotNull(nextRound);
        Assert.Equal(2, schedule.Rounds.Count);
    }

    [Fact]
    public void DoubleEliminationRoundGenerator_CreateNextRound_DelegatesToDoubleEliminationTournamentAdvanceRound()
    {
        var generator = new DoubleEliminationRoundGenerator();
        var schedule = DoubleEliminationTournament.Create(EliminationTournamentTests.CreateSchedule(4), [1, 2, 3, 4]);
        var round = schedule.Rounds.Single().Value;

        var teamResults = new List<TeamResult> { new(1, 10, 0, 1), new(4, 0, 0, 2) };
        var teamResults2 = new List<TeamResult> { new(2, 10, 0, 1), new(3, 0, 0, 2) };
        var matches = new[]
        {
            new MatchResult(0, round.Matches[1].Room, round.Id, teamResults, []),
            new MatchResult(0, round.Matches[2].Room, round.Id, teamResults2, []),
        }.ToDictionary(m => m.ScheduleId, m => m);
        var result = new Result(schedule, matches);

        Assert.False(generator.IsComplete(schedule, result));

        var nextRound = generator.CreateNextRound(schedule, result);

        Assert.NotNull(nextRound);
        Assert.Equal(2, schedule.Rounds.Count);
    }

    [Fact]
    public void TripleEliminationRoundGenerator_CreateNextRound_DelegatesToTripleEliminationTournamentAdvanceRound()
    {
        var generator = new TripleEliminationRoundGenerator();
        var schedule = TripleEliminationTournament.Create(EliminationTournamentTests.CreateSchedule(4), [1, 2, 3, 4]);
        var round = schedule.Rounds.Single().Value;

        var teamResults = new List<TeamResult> { new(1, 10, 0, 1), new(4, 0, 0, 2) };
        var teamResults2 = new List<TeamResult> { new(2, 10, 0, 1), new(3, 0, 0, 2) };
        var matches = new[]
        {
            new MatchResult(0, round.Matches[1].Room, round.Id, teamResults, []),
            new MatchResult(0, round.Matches[2].Room, round.Id, teamResults2, []),
        }.ToDictionary(m => m.ScheduleId, m => m);
        var result = new Result(schedule, matches);

        Assert.False(generator.IsComplete(schedule, result));

        var nextRound = generator.CreateNextRound(schedule, result);

        Assert.NotNull(nextRound);
        Assert.Equal(2, schedule.Rounds.Count);
    }

    [Fact]
    public void SwissRoundGenerator_CreateNextRound_DelegatesToSwissTournamentAdvanceRound()
    {
        var generator = new SwissRoundGenerator();
        var schedule = SwissTournament.Create(EliminationTournamentTests.CreateSchedule(4), [1, 2, 3, 4]);
        var round = schedule.Rounds.Single().Value;

        Assert.False(generator.IsComplete(schedule, Result.Null));

        var matches = round.Matches.Values.Select(match =>
        {
            var teamResults = new List<TeamResult> { new(match.Teams[0], 10, 0, 1), new(match.Teams[1], 0, 0, 2) };
            return new MatchResult(0, match.Room, round.Id, teamResults, []);
        }).ToDictionary(m => m.ScheduleId, m => m);
        var result = new Result(schedule, matches);

        var nextRound = generator.CreateNextRound(schedule, result);

        Assert.NotNull(nextRound);
        Assert.Equal(2, schedule.Rounds.Count);
    }

    [Fact]
    public void SwissWithTopCutRoundGenerator_CreateNextRound_DelegatesToSwissWithTopCutTournamentAdvanceRound()
    {
        var generator = new SwissWithTopCutRoundGenerator();
        var schedule = SwissWithTopCutTournament.Create(EliminationTournamentTests.CreateSchedule(4), [1, 2, 3, 4]);
        var round = schedule.Rounds.Single().Value;

        Assert.False(generator.IsComplete(schedule, Result.Null));

        var matches = round.Matches.Values.Select(match =>
        {
            var teamResults = new List<TeamResult> { new(match.Teams[0], 10, 0, 1), new(match.Teams[1], 0, 0, 2) };
            return new MatchResult(0, match.Room, round.Id, teamResults, []);
        }).ToDictionary(m => m.ScheduleId, m => m);
        var result = new Result(schedule, matches);

        var nextRound = generator.CreateNextRound(schedule, result);

        Assert.NotNull(nextRound);
        Assert.Equal(2, schedule.Rounds.Count);
    }
}
