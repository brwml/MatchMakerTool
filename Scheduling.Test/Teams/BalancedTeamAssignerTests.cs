namespace Scheduling.Test.Teams;

using System;
using System.Collections.Generic;
using System.Linq;

using MatchMaker.Models;
using MatchMaker.Scheduling.Teams;

using Xunit;

public class BalancedTeamAssignerTests
{
    [Fact]
    public void Create_WithNullSchedule_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => BalancedTeamAssigner.Create(null!, [1, 2], 2));
    }

    [Fact]
    public void Create_WithNullRankedQuizzerIds_ThrowsArgumentNullException()
    {
        var schedule = CreateSchedule(2, 1);
        Assert.Throws<ArgumentNullException>(() => BalancedTeamAssigner.Create(schedule, null!, 2));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidNumberOfTeams_ThrowsArgumentOutOfRangeException(int numberOfTeams)
    {
        var schedule = CreateSchedule(2, 1);
        Assert.Throws<ArgumentOutOfRangeException>(() => BalancedTeamAssigner.Create(schedule, [1, 2], numberOfTeams));
    }

    [Fact]
    public void Create_ReturnsScheduleWithNoRounds()
    {
        var schedule = CreateSchedule(4, 1);
        var quizzerIds = schedule.Quizzers.Keys.ToList();

        var result = BalancedTeamAssigner.Create(schedule, quizzerIds, 2);

        Assert.Empty(result.Rounds);
    }

    [Theory]
    [InlineData(1, "Team A")]
    [InlineData(2, "Team B")]
    [InlineData(26, "Team Z")]
    [InlineData(27, "Team AA")]
    [InlineData(28, "Team AB")]
    [InlineData(52, "Team AZ")]
    [InlineData(53, "Team BA")]
    public void Create_NamesTeamsUsingExcelStyleLetters(int numberOfTeams, string expectedLastTeamName)
    {
        var schedule = CreateSchedule(numberOfTeams, 1);
        var quizzerIds = schedule.Quizzers.Keys.ToList();

        var result = BalancedTeamAssigner.Create(schedule, quizzerIds, numberOfTeams);

        Assert.Equal(expectedLastTeamName, result.Teams[numberOfTeams].Name);
    }

    [Fact]
    public void Create_IncludesEveryRankedQuizzerExactlyOnce()
    {
        var schedule = CreateSchedule(12, 1);
        var quizzerIds = schedule.Quizzers.Keys.ToList();

        var result = BalancedTeamAssigner.Create(schedule, quizzerIds, 3);

        Assert.Equal(quizzerIds.Count, result.Quizzers.Count);
        Assert.Equal(quizzerIds.OrderBy(x => x), result.Quizzers.Keys.OrderBy(x => x));
    }

    [Fact]
    public void Create_DistributesQuizzersEvenlyAcrossTeams()
    {
        var schedule = CreateSchedule(12, 1);
        var quizzerIds = schedule.Quizzers.Keys.ToList();

        var result = BalancedTeamAssigner.Create(schedule, quizzerIds, 3);

        var counts = result.Quizzers.Values.GroupBy(x => x.TeamId).Select(x => x.Count());
        Assert.All(counts, x => Assert.Equal(4, x));
    }

    [Fact]
    public void Create_UsesSnakeDraftOrderForRankedQuizzers()
    {
        // 6 quizzers ranked 1..6 (best first), 3 teams: expect draft order
        // team1: 1, 6 ; team2: 2, 5 ; team3: 3, 4
        var schedule = CreateSchedule(6, 6);
        var quizzerIds = schedule.Quizzers.Keys.OrderBy(x => x).ToList();

        var result = BalancedTeamAssigner.Create(schedule, quizzerIds, 3);

        Assert.Equal(new HashSet<int> { quizzerIds[0], quizzerIds[5] }, result.Quizzers.Values.Where(x => x.TeamId == 1).Select(x => x.Id).ToHashSet());
        Assert.Equal(new HashSet<int> { quizzerIds[1], quizzerIds[4] }, result.Quizzers.Values.Where(x => x.TeamId == 2).Select(x => x.Id).ToHashSet());
        Assert.Equal(new HashSet<int> { quizzerIds[2], quizzerIds[3] }, result.Quizzers.Values.Where(x => x.TeamId == 3).Select(x => x.Id).ToHashSet());
    }

    [Fact]
    public void Create_PreservesQuizzerDetailsOtherThanTeamId()
    {
        var schedule = CreateSchedule(4, 1);
        var quizzerIds = schedule.Quizzers.Keys.ToList();

        var result = BalancedTeamAssigner.Create(schedule, quizzerIds, 2);

        foreach (var (id, quizzer) in result.Quizzers)
        {
            var original = schedule.Quizzers[id];
            Assert.Equal(original.FirstName, quizzer.FirstName);
            Assert.Equal(original.LastName, quizzer.LastName);
            Assert.Equal(original.Gender, quizzer.Gender);
            Assert.Equal(original.RookieYear, quizzer.RookieYear);
            Assert.Equal(original.ChurchId, quizzer.ChurchId);
        }
    }

    [Fact]
    public void Create_WhenChurchConflictIsResolvable_MinimizesSameChurchTeams()
    {
        // 4 quizzers from 2 churches (2 each), 2 teams. Naive snake draft (rank order 1,2,3,4 with
        // churches A,A,B,B) would place quizzers 1 & 4 on team 1 and 2 & 3 on team 2 -- team2 would
        // have both church B quizzers. The swap pass should redistribute so each team has one
        // quizzer from each church.
        var churches = new Dictionary<int, Church>
        {
            { 1, new Church(1, "Church A") },
            { 2, new Church(2, "Church B") }
        };

        var quizzers = new Dictionary<int, Quizzer>
        {
            { 1, new Quizzer(1, "First1", "Last1", Gender.Male, DateTime.Now.Year, 1, 1) },
            { 2, new Quizzer(2, "First2", "Last2", Gender.Male, DateTime.Now.Year, 1, 1) },
            { 3, new Quizzer(3, "First3", "Last3", Gender.Male, DateTime.Now.Year, 1, 2) },
            { 4, new Quizzer(4, "First4", "Last4", Gender.Male, DateTime.Now.Year, 1, 2) }
        };

        var teams = new Dictionary<int, Team> { { 1, new Team(1, "Team 1", "T1", 0) } };
        var schedule = new Schedule("Test", churches, quizzers, teams, new Dictionary<int, Round>());

        var result = BalancedTeamAssigner.Create(schedule, [1, 2, 3, 4], 2);

        foreach (var team in result.Teams.Keys)
        {
            var churchIds = result.Quizzers.Values.Where(x => x.TeamId == team).Select(x => x.ChurchId).ToList();
            Assert.Equal(churchIds.Count, churchIds.Distinct().Count());
        }
    }

    [Fact]
    public void Create_WhenChurchConflictIsUnresolvable_DoesNotThrow()
    {
        // All quizzers from the same church; no swap can remove the duplication, so the
        // algorithm should terminate without error, leaving the conflicts in place.
        var churches = new Dictionary<int, Church> { { 1, new Church(1, "Church A") } };

        var quizzers = Enumerable.Range(1, 6)
            .ToDictionary(x => x, x => new Quizzer(x, $"First{x}", $"Last{x}", Gender.Male, DateTime.Now.Year, 1, 1));

        var teams = new Dictionary<int, Team> { { 1, new Team(1, "Team 1", "T1", 0) } };
        var schedule = new Schedule("Test", churches, quizzers, teams, new Dictionary<int, Round>());

        var result = BalancedTeamAssigner.Create(schedule, [1, 2, 3, 4, 5, 6], 2);

        Assert.Equal(6, result.Quizzers.Count);
    }

    [Fact]
    public void Create_WhenResolvingChurchConflict_PrefersSwapWithClosestRank()
    {
        // 8 quizzers ranked 1 (best) through 8 (worst), 2 teams. The snake draft places
        // {1, 4, 5, 8} on team 1 and {2, 3, 6, 7} on team 2. Quizzers 5 (rank 4) and 8 (rank 7)
        // share a church, so team 1 has a conflict. Every member of team 2 is a valid swap
        // target, but a first-valid-candidate strategy would pick quizzer 2 (the first in the
        // list), while the closest-ranked valid candidate is quizzer 6 (rank 5, only one place
        // away from quizzer 5's rank 4). This distinguishes rank-aware selection from a naive
        // first-match search, since the two strategies choose different swap partners.
        var churches = Enumerable.Range(1, 7).ToDictionary(x => x, x => new Church(x, $"Church {x}"));

        var churchIds = new Dictionary<int, int> { { 1, 2 }, { 2, 3 }, { 3, 4 }, { 4, 5 }, { 5, 1 }, { 6, 6 }, { 7, 7 }, { 8, 1 } };

        var quizzers = churchIds.ToDictionary(
            x => x.Key,
            x => new Quizzer(x.Key, $"First{x.Key}", $"Last{x.Key}", Gender.Male, DateTime.Now.Year, 1, x.Value));

        var teams = new Dictionary<int, Team> { { 1, new Team(1, "Team 1", "T1", 0) } };
        var schedule = new Schedule("Test", churches, quizzers, teams, new Dictionary<int, Round>());

        var result = BalancedTeamAssigner.Create(schedule, [1, 2, 3, 4, 5, 6, 7, 8], 2);

        // Quizzer 5's church conflict with quizzer 8 must be resolved.
        Assert.NotEqual(result.Quizzers[5].TeamId, result.Quizzers[8].TeamId);

        // Quizzers 5 and 6 (the closest-ranked valid swap pair) traded teams; quizzer 2 (the
        // first-encountered, but farther-ranked, candidate) was left in place.
        var teamOfQuizzer5 = result.Quizzers[5].TeamId;
        var teamOfQuizzer6 = result.Quizzers[6].TeamId;

        Assert.NotEqual(teamOfQuizzer5, teamOfQuizzer6);
        Assert.Equal(teamOfQuizzer6, result.Quizzers[1].TeamId);
        Assert.Equal(teamOfQuizzer6, result.Quizzers[4].TeamId);
        Assert.Equal(teamOfQuizzer6, result.Quizzers[8].TeamId);
        Assert.Equal(teamOfQuizzer5, result.Quizzers[2].TeamId);
        Assert.Equal(teamOfQuizzer5, result.Quizzers[3].TeamId);
        Assert.Equal(teamOfQuizzer5, result.Quizzers[7].TeamId);

        // Confirm every church is now conflict-free.
        foreach (var team in result.Teams.Keys)
        {
            var groupChurchIds = result.Quizzers.Values.Where(x => x.TeamId == team).Select(x => x.ChurchId).ToList();
            Assert.Equal(groupChurchIds.Count, groupChurchIds.Distinct().Count());
        }
    }

    [Fact]
    public void Create_WhenOneChurchConflictIsUnresolvable_StillResolvesAnotherInTheSameGroup()
    {
        // 8 quizzers ranked 1 (best) through 8 (worst), 2 teams. The snake draft places
        // {1, 4, 5, 8} on team 1 and {2, 3, 6, 7} on team 2. Team 1 has two church conflicts:
        // quizzers 1 & 4 (church 1, unresolvable because quizzer 2 on team 2 is also church 1)
        // and quizzers 5 & 8 (church 2, resolvable by swapping with a team 2 quizzer from a
        // different church). The algorithm must not give up on the whole group just because one
        // of its conflicts cannot be resolved.
        var churches = Enumerable.Range(1, 5).ToDictionary(x => x, x => new Church(x, $"Church {x}"));

        var churchIds = new Dictionary<int, int>
        {
            { 1, 1 }, { 2, 1 }, { 3, 3 }, { 4, 1 }, { 5, 2 }, { 6, 4 }, { 7, 5 }, { 8, 2 }
        };

        var quizzers = churchIds.ToDictionary(
            x => x.Key,
            x => new Quizzer(x.Key, $"First{x.Key}", $"Last{x.Key}", Gender.Male, DateTime.Now.Year, 1, x.Value));

        var teams = new Dictionary<int, Team> { { 1, new Team(1, "Team 1", "T1", 0) } };
        var schedule = new Schedule("Test", churches, quizzers, teams, new Dictionary<int, Round>());

        var result = BalancedTeamAssigner.Create(schedule, [1, 2, 3, 4, 5, 6, 7, 8], 2);

        // The church-1 conflict between quizzers 1 and 4 cannot be resolved (quizzer 2, also
        // church 1, occupies the only other team) and should remain.
        Assert.Equal(result.Quizzers[1].TeamId, result.Quizzers[4].TeamId);

        // The church-2 conflict between quizzers 5 and 8 should have been resolved.
        Assert.NotEqual(result.Quizzers[5].TeamId, result.Quizzers[8].TeamId);
    }

    [Fact]
    public void Create_WithFewerQuizzersThanTeams_LeavesSomeTeamsEmpty()
    {
        var schedule = CreateSchedule(2, 1);
        var quizzerIds = schedule.Quizzers.Keys.ToList();

        var result = BalancedTeamAssigner.Create(schedule, quizzerIds, 5);

        Assert.Equal(5, result.Teams.Count);
        Assert.Equal(2, result.Quizzers.Count);
    }

    private static Schedule CreateSchedule(int numberOfQuizzers, int numberOfChurches)
    {
        var churches = Enumerable.Range(1, numberOfChurches).ToDictionary(x => x, x => new Church(x, $"Church {x}"));

        var quizzers = new Dictionary<int, Quizzer>();

        for (var i = 1; i <= numberOfQuizzers; i++)
        {
            var churchId = ((i - 1) % numberOfChurches) + 1;
            quizzers.Add(i, new Quizzer(i, $"First{i}", $"Last{i}", Gender.Male, DateTime.Now.Year, 1, churchId));
        }

        var teams = new Dictionary<int, Team> { { 1, new Team(1, "Team 1", "T1", 0) } };

        return new Schedule("Test Tournament", churches, quizzers, teams, new Dictionary<int, Round>());
    }
}
