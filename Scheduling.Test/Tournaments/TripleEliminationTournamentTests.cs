namespace Scheduling.Test.Tournaments;

using System;
using System.Collections.Generic;
using System.Linq;

using MatchMaker.Models;
using MatchMaker.Scheduling.Tournaments;

using Xunit;

public class TripleEliminationTournamentTests
{
    [Fact]
    public void Create_SetsTypeToTripleElimination()
    {
        var schedule = EliminationTournamentTests.CreateSchedule(4);
        var result = TripleEliminationTournament.Create(schedule, [1, 2, 3, 4]);

        Assert.Equal(TournamentType.TripleElimination, result.Type);
    }

    [Fact]
    public void Create_WithFourTeams_PairsStrongestAgainstWeakest()
    {
        var schedule = EliminationTournamentTests.CreateSchedule(4);
        var result = TripleEliminationTournament.Create(schedule, [1, 2, 3, 4]);

        var round = Assert.Single(result.Rounds).Value;
        var matches = round.Matches.Values.OrderBy(x => x.Id).ToList();

        Assert.Equal([1, 4], matches[0].Teams);
        Assert.Equal([2, 3], matches[1].Teams);
    }

    [Fact]
    public void AdvanceRound_WithNullArguments_ThrowsArgumentNullException()
    {
        var schedule = TripleEliminationTournament.Create(EliminationTournamentTests.CreateSchedule(4), [1, 2, 3, 4]);

        Assert.Throws<ArgumentNullException>(() => TripleEliminationTournament.AdvanceRound(null!, Result.Null));
        Assert.Throws<ArgumentNullException>(() => TripleEliminationTournament.AdvanceRound(schedule, null!));
    }

    [Fact]
    public void AdvanceRound_TeamWithTwoLosses_SurvivesIntoNextRound()
    {
        var schedule = TripleEliminationTournament.Create(EliminationTournamentTests.CreateSchedule(2), [1, 2]);
        var round1 = schedule.Rounds.Single().Value;
        var match1Id = round1.Matches.Keys.Single();

        // Team 2 loses twice in a row but must not be eliminated until its third loss.
        var result1 = CreateResult(schedule, (round1, match1Id, 1, 2));
        var round2 = TripleEliminationTournament.AdvanceRound(schedule, result1);
        Assert.NotNull(round2);

        var match2Id = round2.Matches.Keys.Single();
        var result2 = CreateResult(schedule, (round1, match1Id, 1, 2), (round2, match2Id, 1, 2));
        var round3 = TripleEliminationTournament.AdvanceRound(schedule, result2);

        Assert.NotNull(round3);
        Assert.False(TripleEliminationTournament.IsComplete(schedule, result2));

        var teamsInRound3 = round3.Matches.Values.SelectMany(m => m.Teams).ToHashSet();
        Assert.Contains(2, teamsInRound3);
    }

    [Fact]
    public void Tournament_CompletesOnlyAfterThirdLoss()
    {
        var schedule = TripleEliminationTournament.Create(EliminationTournamentTests.CreateSchedule(2), [1, 2]);
        var round1 = schedule.Rounds.Single().Value;
        var match1Id = round1.Matches.Keys.Single();

        var result1 = CreateResult(schedule, (round1, match1Id, 1, 2));
        var round2 = TripleEliminationTournament.AdvanceRound(schedule, result1);
        Assert.NotNull(round2);

        var match2Id = round2.Matches.Keys.Single();
        var result2 = CreateResult(schedule, (round1, match1Id, 1, 2), (round2, match2Id, 1, 2));
        var round3 = TripleEliminationTournament.AdvanceRound(schedule, result2);
        Assert.NotNull(round3);

        var match3Id = round3.Matches.Keys.Single();
        var result3 = CreateResult(schedule, (round1, match1Id, 1, 2), (round2, match2Id, 1, 2), (round3, match3Id, 1, 2));

        Assert.Null(TripleEliminationTournament.AdvanceRound(schedule, result3));
        Assert.True(TripleEliminationTournament.IsComplete(schedule, result3));
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
