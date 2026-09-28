namespace Scheduling.Test.Tournaments;

using System;
using System.Collections.Generic;
using System.Linq;

using MatchMaker.Models;
using MatchMaker.Scheduling.Tournaments;

using Xunit;

public class DoubleEliminationTournamentTests
{
    [Fact]
    public void Create_SetsTypeToDoubleElimination()
    {
        var schedule = EliminationTournamentTests.CreateSchedule(4);
        var result = DoubleEliminationTournament.Create(schedule, [1, 2, 3, 4]);

        Assert.Equal(TournamentType.DoubleElimination, result.Type);
    }

    [Fact]
    public void Create_WithFourTeams_PairsStrongestAgainstWeakest()
    {
        var schedule = EliminationTournamentTests.CreateSchedule(4);
        var result = DoubleEliminationTournament.Create(schedule, [1, 2, 3, 4]);

        var round = Assert.Single(result.Rounds).Value;
        var matches = round.Matches.Values.OrderBy(x => x.Id).ToList();

        Assert.Equal([1, 4], matches[0].Teams);
        Assert.Equal([2, 3], matches[1].Teams);
    }

    [Fact]
    public void AdvanceRound_WithNullArguments_ThrowsArgumentNullException()
    {
        var schedule = DoubleEliminationTournament.Create(EliminationTournamentTests.CreateSchedule(4), [1, 2, 3, 4]);

        Assert.Throws<ArgumentNullException>(() => DoubleEliminationTournament.AdvanceRound(null!, Result.Null));
        Assert.Throws<ArgumentNullException>(() => DoubleEliminationTournament.AdvanceRound(schedule, null!));
    }

    [Fact]
    public void AdvanceRound_TeamWithOneLoss_SurvivesIntoNextRound()
    {
        var schedule = DoubleEliminationTournament.Create(EliminationTournamentTests.CreateSchedule(4), [1, 2, 3, 4]);
        var round1 = schedule.Rounds.Single().Value;

        // Round 1: match 1 = (1, 4), match 2 = (2, 3). Teams 1 and 2 win; teams 3 and 4 each have
        // exactly one loss and must not yet be eliminated.
        var result = CreateResult(schedule, (round1, 1, 1, 4), (round1, 2, 2, 3));
        var round2 = DoubleEliminationTournament.AdvanceRound(schedule, result);

        Assert.NotNull(round2);

        var teamsInRound2 = round2.Matches.Values.SelectMany(m => m.Teams).ToHashSet();

        Assert.Equal(new HashSet<int> { 1, 2, 3, 4 }, teamsInRound2);
        Assert.False(DoubleEliminationTournament.IsComplete(schedule, result));
    }

    [Fact]
    public void AdvanceRound_TeamWithTwoLosses_IsEliminated()
    {
        var schedule = DoubleEliminationTournament.Create(EliminationTournamentTests.CreateSchedule(4), [1, 2, 3, 4]);
        var round1 = schedule.Rounds.Single().Value;

        // Round 1: team 1 beats team 4, team 2 beats team 3 (each loser now has one loss).
        var result = CreateResult(schedule, (round1, 1, 1, 4), (round1, 2, 2, 3));
        var round2 = DoubleEliminationTournament.AdvanceRound(schedule, result);

        Assert.NotNull(round2);

        // Round 2 re-pairs every team that has fewer than two losses: (1 vs 3) and (2 vs 4).
        // Resolve both matches so team 3 loses again (its second loss) while team 1 and team 2
        // remain undefeated.
        var matchWithTeam3 = round2.Matches.Values.Single(m => m.Teams.Contains(3));
        var matchWithTeam4 = round2.Matches.Values.Single(m => m.Teams.Contains(4));
        var opponentOf3 = matchWithTeam3.Teams.First(t => t != 3);
        var opponentOf4 = matchWithTeam4.Teams.First(t => t != 4);

        var updatedResult = CreateResult(
            schedule,
            (round1, 1, 1, 4),
            (round1, 2, 2, 3),
            (round2, matchWithTeam3.Id, opponentOf3, 3),
            (round2, matchWithTeam4.Id, opponentOf4, 4));

        var round3 = DoubleEliminationTournament.AdvanceRound(schedule, updatedResult);

        Assert.NotNull(round3);

        var teamsStillIn = round3.Matches.Values.SelectMany(m => m.Teams).ToHashSet();

        Assert.DoesNotContain(3, teamsStillIn);
        Assert.DoesNotContain(4, teamsStillIn);
        Assert.Equal(new HashSet<int> { 1, 2 }, teamsStillIn);
    }

    [Fact]
    public void Tournament_CompletesWhenOnlyOneTeamHasFewerThanTwoLosses()
    {
        var schedule = DoubleEliminationTournament.Create(EliminationTournamentTests.CreateSchedule(2), [1, 2]);
        var round1 = schedule.Rounds.Single().Value;
        var matchId = round1.Matches.Keys.Single();

        // Team 1 wins twice in a row (team 2 never wins), so team 2 reaches two losses and the
        // tournament completes without needing a bracket reset.
        var result1 = CreateResult(schedule, (round1, matchId, 1, 2));
        var round2 = DoubleEliminationTournament.AdvanceRound(schedule, result1);

        Assert.NotNull(round2);
        Assert.False(DoubleEliminationTournament.IsComplete(schedule, result1));

        var match2Id = round2.Matches.Keys.Single();
        var result2 = CreateResult(schedule, (round1, matchId, 1, 2), (round2, match2Id, 1, 2));

        Assert.Null(DoubleEliminationTournament.AdvanceRound(schedule, result2));
        Assert.True(DoubleEliminationTournament.IsComplete(schedule, result2));
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
