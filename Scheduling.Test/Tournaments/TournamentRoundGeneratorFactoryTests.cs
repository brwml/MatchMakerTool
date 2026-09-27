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

    [Theory]
    [InlineData(TournamentType.DoubleElimination)]
    [InlineData(TournamentType.TripleElimination)]
    [InlineData(TournamentType.Swiss)]
    [InlineData(TournamentType.SwissWithTopCut)]
    public void For_NotYetImplementedTypes_ThrowsNotSupportedException(TournamentType type)
    {
        Assert.Throws<NotSupportedException>(() => TournamentRoundGeneratorFactory.For(type));
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
}
