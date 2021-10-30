namespace Scheduling.Test;

using System;
using System.Collections.Generic;
using System.Linq;

using MatchMaker.Reporting.Models;
using MatchMaker.Scheduling;

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
                Assert.Equal(match.Id, CreateMatchId(match));
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

    public static IEnumerable<object[]> CreateRoundParameters()
    {
        for (var numTeams = 2; numTeams <= 100; numTeams++)
        {
            for (var numRooms = 1; numRooms <= numTeams / 2; numRooms++)
            {
                var numMatches = numTeams * (numTeams - 1) / 2;
                var numRounds = (numMatches / numRooms) + (numMatches % numRooms != 0 ? 1 : 0);
                yield return new object[] { numTeams, numRooms, numRounds };
            }
        }
    }

    private static int CreateMatchId(MatchSchedule match)
    {
        return (match.Teams.Min() << 16) + match.Teams.Max();
    }

    private static Schedule CreateSchedule(int numTeams)
    {
        return new Schedule
        {
            Teams = CreateSequence(numTeams).ToDictionary(x => x, x => new Team { Id = x })
        };
    }

    private static IEnumerable<int> CreateSequence(int num)
    {
        for (var i = 0; i < num; i++)
        {
            yield return i + 1;
        }
    }
}
