namespace Scheduling.Test.Tournaments;

using System;
using System.Collections.Generic;
using System.Linq;

using MatchMaker.Models;
using MatchMaker.Scheduling.Tournaments;

using Xunit;

public class RoundRobinTournamentTests
{
    [Theory]
    [MemberData(nameof(CreateRoundParameters))]
    public void VerifyCorrectNumberOfRounds(int numTeams, int availableRooms, int expectedRounds)
    {
        var schedule = CreateSchedule(numTeams);
        schedule = RoundRobinTournament.Create(schedule, availableRooms);

        Assert.Equal(expectedRounds, schedule.Rounds.Count);
    }

    [Theory]
    [MemberData(nameof(CreateRoundParameters))]
    public void VerifyCorrectNumberOfRooms(int numTeams, int availableRooms, int expectedRounds)
    {
        var schedule = CreateSchedule(numTeams);
        schedule = RoundRobinTournament.Create(schedule, availableRooms);

        Assert.True(expectedRounds > 0);

        foreach (var round in schedule.Rounds.Select(x => x.Value))
        {
            var matches = round.Matches.Select(x => x.Value).ToList();
            Assert.Equal(1, matches.Min(x => x.Room));
            Assert.Equal(Math.Min(matches.Count, availableRooms), matches.Max(x => x.Room));
            Assert.Equal(Math.Min(matches.Count, availableRooms), matches.Select(x => x.Room).Distinct().Count());
        }
    }

    [Theory]
    [MemberData(nameof(CreateRoundParameters))]
    public void VerifyTeamsAppearOncePerRound(int numTeams, int availableRooms, int expectedRounds)
    {
        var schedule = CreateSchedule(numTeams);
        schedule = RoundRobinTournament.Create(schedule, availableRooms);

        Assert.True(expectedRounds > 0);

        foreach (var round in schedule.Rounds.Select(x => x.Value))
        {
            var matches = round.Matches.Select(x => x.Value).ToList();
            var teamSet = new HashSet<int>();

            foreach (var match in matches)
            {
                foreach (var team in match.Teams)
                {
                    Assert.False(teamSet.Contains(team));
                    teamSet.Add(team);
                }
            }
        }
    }

    [Theory]
    [MemberData(nameof(CreateRoundParameters))]
    public void VerifyNoRepeatedMatches(int numTeams, int availableRooms, int expectedRounds)
    {
        var schedule = CreateSchedule(numTeams);
        schedule = RoundRobinTournament.Create(schedule, availableRooms);

        Assert.True(expectedRounds > 0);

        var matchSet = new HashSet<int>();

        foreach (var round in schedule.Rounds.Select(x => x.Value))
        {
            foreach (var match in round.Matches.Select(x => x.Value))
            {
                Assert.False(matchSet.Contains(match.Id));
                matchSet.Add(match.Id);
            }
        }
    }

    [Theory]
    [InlineData(8, 8, 7, 4)]
    [InlineData(7, 7, 7, 3)]
    public void VerifyRoomReductionWhenNecessary(int numTeams, int availableRooms, int expectedRounds, int maxRoom)
    {
        var schedule = CreateSchedule(numTeams);
        schedule = RoundRobinTournament.Create(schedule, availableRooms);

        Assert.Equal(expectedRounds, schedule.Rounds.Count);
        Assert.Equal(maxRoom, schedule.Rounds.SelectMany(x => x.Value.Matches).Select(x => x.Value.Room).Max());
    }

    // --- Guard tests ---

    [Fact]
    public void Create_WithNullSchedule_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => RoundRobinTournament.Create(null!, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_WithInvalidRoomCount_ThrowsArgumentOutOfRangeException(int rooms)
    {
        var schedule = CreateSchedule(4);
        Assert.Throws<ArgumentOutOfRangeException>(() => RoundRobinTournament.Create(schedule, rooms));
    }

    // --- Edge case tests ---

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void Create_WithTooFewTeams_ReturnsEmptySchedule(int numTeams)
    {
        var schedule = CreateSchedule(numTeams);
        schedule = RoundRobinTournament.Create(schedule, 1);

        Assert.Empty(schedule.Rounds);
    }

    // --- Algorithm completeness ---

    [Theory]
    [InlineData(2, 1)]
    [InlineData(3, 1)]
    [InlineData(4, 2)]
    [InlineData(5, 2)]
    [InlineData(6, 3)]
    [InlineData(7, 3)]
    [InlineData(8, 4)]
    [InlineData(10, 5)]
    [InlineData(11, 5)]
    [InlineData(20, 10)]
    public void VerifyEveryPairPlaysExactlyOnce(int numTeams, int availableRooms)
    {
        var schedule = CreateSchedule(numTeams);
        schedule = RoundRobinTournament.Create(schedule, availableRooms);

        var pairs = schedule.Rounds.Values
            .SelectMany(r => r.Matches.Values)
            .Select(m => (Math.Min(m.Teams[0], m.Teams[1]), Math.Max(m.Teams[0], m.Teams[1])))
            .ToList();

        var expectedCount = numTeams * (numTeams - 1) / 2;
        Assert.Equal(expectedCount, pairs.Count);
        Assert.Equal(expectedCount, pairs.Distinct().Count());
    }

    // --- Date/time tests ---

    [Fact]
    public void Create_WithStartDate_SetsDateOnAllRounds()
    {
        var date = new DateOnly(2025, 6, 15);
        var schedule = CreateSchedule(4);
        schedule = RoundRobinTournament.Create(schedule, 2, startDate: date);

        Assert.All(schedule.Rounds.Values, r => Assert.Equal(date, r.Date));
    }

    [Fact]
    public void Create_WithStartTimeAndDuration_SetsIncrementalStartTimesOnRounds()
    {
        var startTime = new TimeOnly(9, 0);
        var duration = TimeSpan.FromMinutes(45);
        var schedule = CreateSchedule(4);
        schedule = RoundRobinTournament.Create(schedule, 2, startTime: startTime, roundDuration: duration);

        var rounds = schedule.Rounds.Values.OrderBy(r => r.Id).ToList();
        for (var i = 0; i < rounds.Count; i++)
        {
            Assert.Equal(startTime.Add(TimeSpan.FromMinutes(45 * i)), rounds[i].Time);
        }
    }

    [Fact]
    public void Create_WithStartTimeOnly_SetsSameTimeOnAllRounds()
    {
        var startTime = new TimeOnly(10, 30);
        var schedule = CreateSchedule(4);
        schedule = RoundRobinTournament.Create(schedule, 2, startTime: startTime);

        Assert.All(schedule.Rounds.Values, r => Assert.Equal(startTime, r.Time));
    }

    [Fact]
    public void Create_WithDurationCausingMidnightRollover_AdvancesDateOnAffectedRounds()
    {
        // 4 teams, 2 rooms => 3 rounds (indices 0, 1, 2)
        // Round 0 => 23:00 on day D
        // Round 1 => 01:00 on D+1   (23:00 + 2h wraps past midnight)
        // Round 2 => 03:00 on D+1
        var date = new DateOnly(2025, 6, 15);
        var startTime = new TimeOnly(23, 0);
        var duration = TimeSpan.FromHours(2);
        var schedule = CreateSchedule(4);
        schedule = RoundRobinTournament.Create(schedule, 2, startDate: date, startTime: startTime, roundDuration: duration);

        var rounds = schedule.Rounds.Values.OrderBy(r => r.Id).ToList();
        Assert.Equal(3, rounds.Count);

        Assert.Equal(date, rounds[0].Date);
        Assert.Equal(new TimeOnly(23, 0), rounds[0].Time);

        Assert.Equal(date.AddDays(1), rounds[1].Date);
        Assert.Equal(new TimeOnly(1, 0), rounds[1].Time);

        Assert.Equal(date.AddDays(1), rounds[2].Date);
        Assert.Equal(new TimeOnly(3, 0), rounds[2].Time);
    }

    [Fact]
    public void Create_WithStartDateAndDuration_SetsDateAndIncrementalTimes()
    {
        var date = new DateOnly(2025, 3, 8);
        var startTime = new TimeOnly(8, 0);
        var duration = TimeSpan.FromMinutes(30);
        var schedule = CreateSchedule(4);
        schedule = RoundRobinTournament.Create(schedule, 2, startDate: date, startTime: startTime, roundDuration: duration);

        var rounds = schedule.Rounds.Values.OrderBy(r => r.Id).ToList();
        Assert.All(rounds, r => Assert.Equal(date, r.Date));

        for (var i = 0; i < rounds.Count; i++)
        {
            Assert.Equal(startTime.Add(TimeSpan.FromMinutes(30 * i)), rounds[i].Time);
        }
    }

    public static TheoryData<int, int, int> CreateRoundParameters()
    {
        var data = new TheoryData<int, int, int>();

        for (var numTeams = 2; numTeams <= 100; numTeams++)
        {
            for (var numRooms = 1; numRooms <= numTeams / 2; numRooms++)
            {
                var numMatches = numTeams * (numTeams - 1) / 2;
                var numRounds = (numMatches / numRooms) + (numMatches % numRooms != 0 ? 1 : 0);
                data.Add(numTeams, numRooms, numRounds);
            }
        }

        return data;
    }

    internal static Schedule CreateSchedule(int numTeams)
    {
        var teams = Enumerable.Range(1, numTeams).ToDictionary(x => x, x => new Team(x, string.Empty, string.Empty, 0));
        return new Schedule(string.Empty, new Dictionary<int, Church>(), new Dictionary<int, Quizzer>(), teams, new Dictionary<int, Round>());
    }
}