namespace Scheduling.Test.Tournaments;

using System.Collections.Generic;
using System.Linq;

using MatchMaker.Models;
using MatchMaker.Scheduling.Tournaments;

using Xunit;

public class ScheduleExtensionsTests
{
    // --- GetByeTeamId ---

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(8)]
    public void GetByeTeamId_WithEvenTeams_ReturnsNullForAllRounds(int numTeams)
    {
        var schedule = RoundRobinTournamentTests.CreateSchedule(numTeams);
        schedule = RoundRobinTournament.Create(schedule, numTeams / 2);

        Assert.All(schedule.Rounds.Values, r => Assert.Null(schedule.GetByeTeamId(r)));
    }

    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(7)]
    [InlineData(9)]
    public void GetByeTeamId_WithOddTeamsFullRotation_ReturnsOneByePerRound(int numTeams)
    {
        var schedule = RoundRobinTournamentTests.CreateSchedule(numTeams);
        schedule = RoundRobinTournament.Create(schedule, numTeams / 2);

        Assert.All(schedule.Rounds.Values, r => Assert.NotNull(schedule.GetByeTeamId(r)));
    }

    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(7)]
    [InlineData(9)]
    public void GetByeTeamId_WithOddTeamsFullRotation_EachTeamHasByeExactlyOnce(int numTeams)
    {
        var schedule = RoundRobinTournamentTests.CreateSchedule(numTeams);
        schedule = RoundRobinTournament.Create(schedule, numTeams / 2);

        var byeIds = schedule.Rounds.Values
            .Select(r => schedule.GetByeTeamId(r))
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToList();

        // Every team has exactly one bye in a full odd round-robin
        Assert.Equal(numTeams, byeIds.Count);
        Assert.Equal(numTeams, byeIds.Distinct().Count());
    }

    [Theory]
    [InlineData(7, 2)]   // matchCount=3, effectiveRooms=2 → 3 idle per round
    [InlineData(9, 3)]   // matchCount=4, effectiveRooms=3 → 3 idle per round
    [InlineData(11, 4)]  // matchCount=5, effectiveRooms=4 → 3 idle per round
    public void GetByeTeamId_WithLimitedRoomsCreatingPartialRotations_ReturnsNull(int numTeams, int rooms)
    {
        var schedule = RoundRobinTournamentTests.CreateSchedule(numTeams);
        schedule = RoundRobinTournament.Create(schedule, rooms);

        // With rooms < matchCount, every round is partial: multiple teams idle, no single bye
        var partialRounds = schedule.Rounds.Values
            .Where(r => r.Matches.Count < numTeams / 2)
            .ToList();

        Assert.NotEmpty(partialRounds);
        Assert.All(partialRounds, r => Assert.Null(schedule.GetByeTeamId(r)));
    }

    // --- GetByeTeam ---

    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(7)]
    public void GetByeTeam_WithOddTeamsFullRotation_ReturnsTeamPresentInSchedule(int numTeams)
    {
        var schedule = RoundRobinTournamentTests.CreateSchedule(numTeams);
        schedule = RoundRobinTournament.Create(schedule, numTeams / 2);

        foreach (var round in schedule.Rounds.Values)
        {
            var byeTeam = schedule.GetByeTeam(round);
            Assert.NotNull(byeTeam);
            Assert.True(schedule.Teams.ContainsKey(byeTeam.Id));
        }
    }

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(6)]
    public void GetByeTeam_WithEvenTeams_ReturnsNullForAllRounds(int numTeams)
    {
        var schedule = RoundRobinTournamentTests.CreateSchedule(numTeams);
        schedule = RoundRobinTournament.Create(schedule, numTeams / 2);

        Assert.All(schedule.Rounds.Values, r => Assert.Null(schedule.GetByeTeam(r)));
    }

    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(7)]
    public void GetByeTeam_IdAndObjectAreConsistent(int numTeams)
    {
        var schedule = RoundRobinTournamentTests.CreateSchedule(numTeams);
        schedule = RoundRobinTournament.Create(schedule, numTeams / 2);

        foreach (var round in schedule.Rounds.Values)
        {
            var byeId = schedule.GetByeTeamId(round);
            var byeTeam = schedule.GetByeTeam(round);

            if (byeId.HasValue)
            {
                Assert.NotNull(byeTeam);
                Assert.Equal(byeId.Value, byeTeam.Id);
            }
            else
            {
                Assert.Null(byeTeam);
            }
        }
    }
}
