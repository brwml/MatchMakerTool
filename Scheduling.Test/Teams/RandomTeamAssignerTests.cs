namespace Scheduling.Test.Teams;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Bogus;

using MatchMaker.Models;
using MatchMaker.Scheduling.Teams;

using Xunit;

public class RandomTeamAssignerTests
{
    [Theory(DisplayName = "Random Team Assignment Tests")]
    [MemberData(nameof(GetRandomTeamsTestData))]
    [SuppressMessage("Usage", "xUnit1045:Avoid using TheoryData type arguments that might not be serializable", Justification = "<Pending>")]
    public void TestRandomAssignment(int numberOfRooms, int numberOfTeams, Schedule inputSchedule)
    {
        var assigner = new RandomTeamAssigner();
        var actualSchedule = assigner.Create(inputSchedule, numberOfRooms, numberOfTeams);

        Assert.NotNull(actualSchedule);
        Assert.Equal(inputSchedule.Quizzers.Count, actualSchedule.Quizzers.Count);
        Assert.True(actualSchedule.Teams.Count >= Math.Max(numberOfTeams, numberOfRooms * 2));

        var allTeamsAssigned = new HashSet<int>();
        foreach (var quizzer in actualSchedule.Quizzers.Values)
        {
            Assert.True(actualSchedule.Teams.ContainsKey(quizzer.TeamId));
            allTeamsAssigned.Add(quizzer.TeamId);
        }

        Assert.True(allTeamsAssigned.Count > 0);
    }

    [Theory]
    [MemberData(nameof(GetRandomTeamsTestData))]
    [SuppressMessage("Usage", "xUnit1045:Avoid using TheoryData type arguments that might not be serializable", Justification = "<Pending>")]
    public void TestAllQuizzersAssigned(int numberOfRooms, int numberOfTeams, Schedule inputSchedule)
    {
        var assigner = new RandomTeamAssigner();
        var actualSchedule = assigner.Create(inputSchedule, numberOfRooms, numberOfTeams);

        Assert.Equal(inputSchedule.Quizzers.Count, actualSchedule.Quizzers.Count);

        for (var i = 1; i <= actualSchedule.Quizzers.Count; i++)
        {
            Assert.True(actualSchedule.Quizzers.ContainsKey(i));
        }
    }

    [Theory]
    [MemberData(nameof(GetRandomTeamsTestData))]
    [SuppressMessage("Usage", "xUnit1045:Avoid using TheoryData type arguments that might not be serializable", Justification = "<Pending>")]
    public void TestTeamsDistributedAcrossQuizzers(int numberOfRooms, int numberOfTeams, Schedule inputSchedule)
    {
        var assigner = new RandomTeamAssigner();
        var actualSchedule = assigner.Create(inputSchedule, numberOfRooms, numberOfTeams);

        var teamAssignments = new Dictionary<int, int>();
        foreach (var quizzer in actualSchedule.Quizzers.Values)
        {
            if (!teamAssignments.TryGetValue(quizzer.TeamId, out var value))
            {
                value = 0;
                teamAssignments[quizzer.TeamId] = value;
            }

            teamAssignments[quizzer.TeamId] = ++value;
        }

        Assert.True(teamAssignments.Count > 0);
        foreach (var count in teamAssignments.Values)
        {
            Assert.True(count > 0);
        }
    }

    public static TheoryData<int, int, Schedule> GetRandomTeamsTestData()
    {
        var data = new TheoryData<int, int, Schedule>();
        var faker = new Faker();

        for (var teams = 2; teams < 20; teams++)
        {
            var quizzers = faker.Random.Int(teams * 2, teams * 3);
            var schedule = BuildSchedule(faker.Random.Number(1, teams), teams, quizzers);

            data.Add(faker.Random.Number(1, teams), teams, schedule);
        }

        return data;
    }

    private static Schedule BuildSchedule(int countChurches, int countTeams, int countQuizzers)
    {
        var churches = BuildChurches(countChurches).ToList();
        var teams = BuildTeams(countTeams).ToList();
        var quizzers = BuildQuizzers(countQuizzers, churches, teams).ToList();

        return new Schedule(
            "Test Schedule",
            churches.ToDictionary(x => x.Id, x => x),
            quizzers.ToDictionary(x => x.Id, x => x),
            teams.ToDictionary(x => x.Id, x => x),
            new Dictionary<int, Round>());
    }

    private static IEnumerable<Church> BuildChurches(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var id = i + 1;
            yield return new Church(id, $"Church {id}");
        }
    }

    private static IEnumerable<Quizzer> BuildQuizzers(int count, List<Church> churches, List<Team> teams)
    {
        var faker = new Faker();

        for (var i = 0; i < count; i++)
        {
            var id = i + 1;
            var gender = id % 2 == 0 ? Gender.Male : Gender.Female;
            var firstName = faker.Name.FirstName();
            var lastName = faker.Name.LastName();
            var year = DateTime.Now.Year;
            var rookieYear = faker.Random.Int(year - 5, year);
            var churchId = (i % churches.Count) + 1;
            var teamId = (i % teams.Count) + 1;

            yield return new Quizzer(id, firstName, lastName, gender, rookieYear, teamId, churchId);
        }
    }

    private static IEnumerable<Team> BuildTeams(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var id = i + 1;
            yield return new Team(id, $"Team {id}", id.ToString("D02", CultureInfo.InvariantCulture), 0);
        }
    }
}
