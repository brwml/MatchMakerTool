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
    public void Create_SetsTypeToSingleElimination()
    {
        var schedule = CreateSchedule(4);
        var result = EliminationTournament.Create(schedule, [1, 2, 3, 4]);

        Assert.Equal(TournamentType.SingleElimination, result.Type);
        Assert.Equal(TournamentType.SingleElimination, result.EffectiveType);
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

    // --- AdvanceRound / IsComplete / IsRoundComplete ---

    [Fact]
    public void AdvanceRound_WithNullSchedule_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => EliminationTournament.AdvanceRound(null!, Result.Null));
    }

    [Fact]
    public void AdvanceRound_WithNullResult_ThrowsArgumentNullException()
    {
        var schedule = EliminationTournament.Create(CreateSchedule(4), [1, 2, 3, 4]);
        Assert.Throws<ArgumentNullException>(() => EliminationTournament.AdvanceRound(schedule, null!));
    }

    [Fact]
    public void AdvanceRound_WithNoRounds_ThrowsInvalidOperationException()
    {
        var schedule = CreateSchedule(4);
        Assert.Throws<InvalidOperationException>(() => EliminationTournament.AdvanceRound(schedule, Result.Null));
    }

    [Fact]
    public void AdvanceRound_WithIncompleteRound_ReturnsNull()
    {
        var schedule = EliminationTournament.Create(CreateSchedule(4), [1, 2, 3, 4]);
        var round = schedule.Rounds.Single().Value;

        // Only one of the two matches in the round has a recorded result.
        var match = round.Matches.Values.OrderBy(x => x.Id).First();
        var result = CreateResult(schedule, (round, match.Id, match.Teams[0], match.Teams[1]));

        Assert.False(EliminationTournament.IsRoundComplete(round, result));
        Assert.Null(EliminationTournament.AdvanceRound(schedule, result));
        Assert.False(EliminationTournament.IsComplete(schedule, result));
    }

    [Fact]
    public void AdvanceRound_WithFourTeams_PairsSurvivingWinnersAndAddsRound()
    {
        var schedule = EliminationTournament.Create(CreateSchedule(4), [1, 2, 3, 4]);
        var round = schedule.Rounds.Single().Value;

        // Round 1: match 1 = (1, 4), match 2 = (2, 3). Team 1 and team 2 win.
        var result = CreateResult(schedule, (round, 1, 1, 4), (round, 2, 2, 3));

        Assert.True(EliminationTournament.IsRoundComplete(round, result));

        var nextRound = EliminationTournament.AdvanceRound(schedule, result);

        Assert.NotNull(nextRound);
        Assert.Equal(2, nextRound.Id);
        Assert.Same(nextRound, schedule.Rounds[2]);
        Assert.Equal(2, schedule.Rounds.Count);

        var match = Assert.Single(nextRound.Matches).Value;
        Assert.Equal([1, 2], match.Teams);
    }

    [Fact]
    public void AdvanceRound_WithFinalRoundDecided_ReturnsNullAndTournamentIsComplete()
    {
        var schedule = EliminationTournament.Create(CreateSchedule(2), [1, 2]);
        var round = schedule.Rounds.Single().Value;
        var result = CreateResult(schedule, (round, round.Matches.Keys.Single(), 1, 2));

        Assert.Null(EliminationTournament.AdvanceRound(schedule, result));
        Assert.True(EliminationTournament.IsComplete(schedule, result));
        Assert.Single(schedule.Rounds);
    }

    [Fact]
    public void AdvanceRound_WithOddTeamCount_ByeTeamCascadesUntilFinal()
    {
        var schedule = EliminationTournament.Create(CreateSchedule(5), [1, 2, 3, 4, 5]);
        var round1 = schedule.Rounds.Single().Value;

        Assert.Equal(1, schedule.GetByeTeamId(round1));

        // Round 1: match 1 = (2, 5), match 2 = (3, 4). Team 2 and team 3 win. A cumulative
        // Result (matching real usage, where every recorded round remains in the Result) is
        // built at each step so elimination history from earlier rounds is visible.
        var result1 = CreateResult(schedule, (round1, 1, 2, 5), (round1, 2, 3, 4));
        var round2 = EliminationTournament.AdvanceRound(schedule, result1);

        Assert.NotNull(round2);

        var round2Match = Assert.Single(round2.Matches).Value;
        Assert.Equal([2, 3], round2Match.Teams);
        Assert.DoesNotContain(1, round2Match.Teams);

        // Round 2: team 2 beats team 3; the bye team (1) still has not played.
        var result2 = CreateResult(schedule, (round1, 1, 2, 5), (round1, 2, 3, 4), (round2, round2Match.Id, 2, 3));
        var round3 = EliminationTournament.AdvanceRound(schedule, result2);

        Assert.NotNull(round3);

        var finalMatch = Assert.Single(round3.Matches).Value;
        Assert.Equal([1, 2], finalMatch.Teams);

        var finalResult = CreateResult(schedule, (round1, 1, 2, 5), (round1, 2, 3, 4), (round2, round2Match.Id, 2, 3), (round3, finalMatch.Id, 1, 2));
        Assert.Null(EliminationTournament.AdvanceRound(schedule, finalResult));
        Assert.True(EliminationTournament.IsComplete(schedule, finalResult));
    }

    /// <summary>
    /// Creates a <see cref="Result"/> recording the given winner/loser outcomes, accumulated
    /// across however many rounds are represented in <paramref name="outcomes"/>, matching real
    /// usage where a <see cref="Result"/> holds the full match history recorded so far.
    /// </summary>
    /// <param name="schedule">The schedule that owns the rounds.</param>
    /// <param name="outcomes">The (round, matchId, winnerTeamId, loserTeamId) tuples for each recorded match.</param>
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

    internal static Schedule CreateSchedule(int numberOfTeams)
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
