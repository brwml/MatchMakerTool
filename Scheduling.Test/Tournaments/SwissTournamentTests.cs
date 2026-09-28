namespace Scheduling.Test.Tournaments;

using System;
using System.Collections.Generic;
using System.Linq;

using MatchMaker.Models;
using MatchMaker.Scheduling.Tournaments;

using Xunit;

public class SwissTournamentTests
{
    [Fact]
    public void Create_WithNullSchedule_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => SwissTournament.Create(null!, [1, 2]));
    }

    [Fact]
    public void Create_WithNullSeededTeamIds_ThrowsArgumentNullException()
    {
        var schedule = EliminationTournamentTests.CreateSchedule(4);
        Assert.Throws<ArgumentNullException>(() => SwissTournament.Create(schedule, null!));
    }

    [Fact]
    public void Create_WithFewerThanTwoTeams_ThrowsArgumentOutOfRangeException()
    {
        var schedule = EliminationTournamentTests.CreateSchedule(4);
        Assert.Throws<ArgumentOutOfRangeException>(() => SwissTournament.Create(schedule, [1]));
    }

    [Fact]
    public void Create_SetsTypeToSwiss()
    {
        var schedule = EliminationTournamentTests.CreateSchedule(4);
        var result = SwissTournament.Create(schedule, [1, 2, 3, 4]);

        Assert.Equal(TournamentType.Swiss, result.Type);
    }

    [Fact]
    public void Create_WithFourTeams_PairsTopHalfAgainstBottomHalf()
    {
        var schedule = EliminationTournamentTests.CreateSchedule(4);
        var result = SwissTournament.Create(schedule, [1, 2, 3, 4]);

        var round = Assert.Single(result.Rounds).Value;
        var matches = round.Matches.Values.OrderBy(x => x.Id).ToList();

        Assert.Equal([1, 3], matches[0].Teams);
        Assert.Equal([2, 4], matches[1].Teams);
    }

    [Fact]
    public void Create_WithOddNumberOfTeams_WeakestSeedSitsOut()
    {
        var schedule = EliminationTournamentTests.CreateSchedule(5);
        var result = SwissTournament.Create(schedule, [1, 2, 3, 4, 5]);

        var round = Assert.Single(result.Rounds).Value;

        Assert.Equal(2, round.Matches.Count);
        Assert.Equal(5, result.GetByeTeamId(round));
    }

    [Fact]
    public void IsComplete_WithTwoTeams_IsTrueAfterOneResolvedRound()
    {
        var schedule = SwissTournament.Create(EliminationTournamentTests.CreateSchedule(2), [1, 2]);
        var round = schedule.Rounds.Single().Value;
        var matchId = round.Matches.Keys.Single();

        Assert.False(SwissTournament.IsComplete(schedule, Result.Null));

        var result = CreateResult(schedule, (round, matchId, 1, 2));

        Assert.True(SwissTournament.IsComplete(schedule, result));
        Assert.Null(SwissTournament.AdvanceRound(schedule, result));
    }

    [Fact]
    public void AdvanceRound_WithIncompleteRound_ReturnsNull()
    {
        var schedule = SwissTournament.Create(EliminationTournamentTests.CreateSchedule(4), [1, 2, 3, 4]);
        var round = schedule.Rounds.Single().Value;
        var match = round.Matches.Values.OrderBy(x => x.Id).First();

        var result = CreateResult(schedule, (round, match.Id, match.Teams[0], match.Teams[1]));

        Assert.Null(SwissTournament.AdvanceRound(schedule, result));
    }

    [Fact]
    public void AdvanceRound_WithFourTeams_PairsByStandingsAvoidingRematches()
    {
        var schedule = SwissTournament.Create(EliminationTournamentTests.CreateSchedule(4), [1, 2, 3, 4]);
        var round1 = schedule.Rounds.Single().Value;

        // Round 1 pairs (1, 3) and (2, 4). Team 1 and team 2 win, so they are tied at the top of
        // the standings, and teams 3 and 4 are tied at the bottom.
        var result = CreateResult(schedule, (round1, round1.Matches.Values.First(m => m.Teams.Contains(1)).Id, 1, 3), (round1, round1.Matches.Values.First(m => m.Teams.Contains(2)).Id, 2, 4));

        Assert.False(SwissTournament.IsComplete(schedule, result));

        var round2 = SwissTournament.AdvanceRound(schedule, result);

        Assert.NotNull(round2);

        var pairs = round2.Matches.Values.Select(m => m.Teams.OrderBy(id => id).ToList()).ToList();

        // Teams 1 and 2 (both undefeated) are paired together instead of repeating round 1's
        // (1, 3) or (2, 4) pairings, and likewise for teams 3 and 4 (both winless).
        Assert.Contains(pairs, p => p.SequenceEqual([1, 2]));
        Assert.Contains(pairs, p => p.SequenceEqual([3, 4]));
    }

    [Fact]
    public void AdvanceRound_WithOddTeamCount_RotatesByeAwayFromPriorByeTeam()
    {
        var schedule = SwissTournament.Create(EliminationTournamentTests.CreateSchedule(5), [1, 2, 3, 4, 5]);
        var round1 = schedule.Rounds.Single().Value;

        // Round 1 pairs (1, 3) and (2, 4); team 5 (the weakest seed) has the bye.
        Assert.Equal(5, schedule.GetByeTeamId(round1));

        // Team 1 beats team 3, and team 2 beats team 4.
        var result = CreateResult(
            schedule,
            (round1, round1.Matches.Values.First(m => m.Teams.Contains(1)).Id, 1, 3),
            (round1, round1.Matches.Values.First(m => m.Teams.Contains(2)).Id, 2, 4));

        var round2 = SwissTournament.AdvanceRound(schedule, result);

        Assert.NotNull(round2);

        // Team 5 already had the bye in round 1, so round 2's bye rotates to a different team
        // instead of leaving team 5 out again.
        var byeTeamId2 = schedule.GetByeTeamId(round2);

        Assert.NotNull(byeTeamId2);
        Assert.NotEqual(5, byeTeamId2);
    }

    /// <summary>
    /// Creates a <see cref="Result"/> recording the given winner/loser outcomes, accumulated
    /// across however many rounds are represented in <paramref name="outcomes"/>.
    /// </summary>
    private static Result CreateResult(Schedule schedule, params (Round Round, int MatchId, int WinnerTeamId, int LoserTeamId)[] outcomes)
    {
        var matches = outcomes.Select(outcome =>
        {
            var teamResults = new List<TeamResult>
            {
                new(outcome.WinnerTeamId, 10, 0, 1),
                new(outcome.LoserTeamId, 0, 0, 2),
            };

            return new MatchResult(0, outcome.Round.Matches[outcome.MatchId].Room, outcome.Round.Id, teamResults, []);
        }).ToDictionary(m => m.ScheduleId, m => m);

        return new Result(schedule, matches);
    }
}
