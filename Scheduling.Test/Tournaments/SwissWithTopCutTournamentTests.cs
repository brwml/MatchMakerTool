namespace Scheduling.Test.Tournaments;

using System;
using System.Collections.Generic;
using System.Linq;

using MatchMaker.Models;
using MatchMaker.Scheduling.Tournaments;

using Xunit;

public class SwissWithTopCutTournamentTests
{
    [Theory]
    [InlineData(2, 2)]
    [InlineData(3, 2)]
    [InlineData(4, 2)]
    [InlineData(5, 2)]
    [InlineData(6, 2)]
    [InlineData(8, 4)]
    [InlineData(9, 4)]
    [InlineData(10, 4)]
    [InlineData(12, 4)]
    [InlineData(16, 8)]
    public void GetTopCutSize_ReturnsLargestPowerOfTwoAtMostHalfTheField(int teamCount, int expected)
    {
        Assert.Equal(expected, SwissWithTopCutTournament.GetTopCutSize(teamCount));
    }

    [Fact]
    public void Create_SetsTypeToSwissWithTopCut()
    {
        var schedule = EliminationTournamentTests.CreateSchedule(4);
        var result = SwissWithTopCutTournament.Create(schedule, [1, 2, 3, 4]);

        Assert.Equal(TournamentType.SwissWithTopCut, result.Type);
    }

    [Fact]
    public void Create_WithFourTeams_PairsTopHalfAgainstBottomHalfLikeSwiss()
    {
        var schedule = EliminationTournamentTests.CreateSchedule(4);
        var result = SwissWithTopCutTournament.Create(schedule, [1, 2, 3, 4]);

        var round = Assert.Single(result.Rounds).Value;
        var matches = round.Matches.Values.OrderBy(x => x.Id).ToList();

        Assert.Equal([1, 3], matches[0].Teams);
        Assert.Equal([2, 4], matches[1].Teams);
    }

    [Fact]
    public void AdvanceRound_WithNullArguments_ThrowsArgumentNullException()
    {
        var schedule = SwissWithTopCutTournament.Create(EliminationTournamentTests.CreateSchedule(4), [1, 2, 3, 4]);

        Assert.Throws<ArgumentNullException>(() => SwissWithTopCutTournament.AdvanceRound(null!, Result.Null));
        Assert.Throws<ArgumentNullException>(() => SwissWithTopCutTournament.AdvanceRound(schedule, null!));
    }

    [Fact]
    public void Tournament_TransitionsFromSwissToTopCutAndCompletes()
    {
        var schedule = SwissWithTopCutTournament.Create(EliminationTournamentTests.CreateSchedule(4), [1, 2, 3, 4]);
        var round1 = schedule.Rounds.Single().Value;

        // Swiss round 1: (1, 3) and (2, 4). Team 1 beats team 3; team 4 beats team 2.
        var result = CreateResult(
            schedule,
            (round1, round1.Matches.Values.First(m => m.Teams.Contains(1)).Id, 1, 3),
            (round1, round1.Matches.Values.First(m => m.Teams.Contains(2)).Id, 4, 2));

        Assert.False(SwissWithTopCutTournament.IsComplete(schedule, result));

        var round2 = SwissWithTopCutTournament.AdvanceRound(schedule, result);
        Assert.NotNull(round2);
        Assert.Equal(2, schedule.Rounds.Count);

        // Swiss round 2 pairs the two round-1 winners (1 and 4) together, and the two round-1
        // losers (2 and 3) together. Team 1 wins again (now 2-0); team 3 beats team 2 (team 3
        // finishes 1-1, ranked ahead of team 4's 1-1 record by the ascending-id tie-break, since
        // team 4 lost this round while team 2 remains winless).
        var matchWithTeam1 = round2.Matches.Values.Single(m => m.Teams.Contains(1));
        var matchWithTeam3 = round2.Matches.Values.Single(m => m.Teams.Contains(3));

        result = CreateResult(
            schedule,
            (round1, round1.Matches.Values.First(m => m.Teams.Contains(1)).Id, 1, 3),
            (round1, round1.Matches.Values.First(m => m.Teams.Contains(2)).Id, 4, 2),
            (round2, matchWithTeam1.Id, 1, 4),
            (round2, matchWithTeam3.Id, 3, 2));

        Assert.False(SwissWithTopCutTournament.IsComplete(schedule, result));

        // The Swiss stage (2 rounds for 4 teams) is now complete; the next call creates the
        // top-cut bracket's first round, seeded from the final Swiss standings (team 1: 2 wins;
        // team 3: 1 win, ranked ahead of team 4's 1 win by the ascending-id tie-break), for a cut
        // size of two.
        var cutRound = SwissWithTopCutTournament.AdvanceRound(schedule, result);

        Assert.NotNull(cutRound);
        Assert.Equal(3, schedule.Rounds.Count);

        var cutMatch = Assert.Single(cutRound.Matches).Value;
        Assert.Equal(new HashSet<int> { 1, 3 }, cutMatch.Teams.ToHashSet());

        // Team 1 wins the top-cut final; the tournament is now complete.
        result = CreateResult(
            schedule,
            (round1, round1.Matches.Values.First(m => m.Teams.Contains(1)).Id, 1, 3),
            (round1, round1.Matches.Values.First(m => m.Teams.Contains(2)).Id, 4, 2),
            (round2, matchWithTeam1.Id, 1, 4),
            (round2, matchWithTeam3.Id, 3, 2),
            (cutRound, cutMatch.Id, cutMatch.Teams[0], cutMatch.Teams[1]));

        Assert.Null(SwissWithTopCutTournament.AdvanceRound(schedule, result));
        Assert.True(SwissWithTopCutTournament.IsComplete(schedule, result));
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
