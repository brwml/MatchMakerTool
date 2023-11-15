namespace Scheduling.Test.Teams;

using System.Collections.Generic;

using Bogus;

using Humanizer;

using MatchMaker.Models;
using MatchMaker.Scheduling.Teams;

using Xunit;

public class OrderedTeamAssignerTests
{
    [Theory(DisplayName = "Ordered Team Assignment Tests ")]
    [MemberData(nameof(GetOrderedTeamsTestData))]
    public void Tests(int numberOfRooms, int numberOfTeams, Schedule inputSchedule, Schedule expectedSchedule)
    {
        var assigner = new OrderedTeamAssigner();
        var actualSchedule = assigner.Create(inputSchedule, numberOfRooms, numberOfTeams);

        foreach (var quizzer in actualSchedule.Quizzers)
        {
            Assert.Equal(expectedSchedule.Quizzers[quizzer.Key].TeamId, quizzer.Value.TeamId);
        }
    }

    public static IEnumerable<object[]> GetOrderedTeamsTestData()
    {
        var faker = new Faker();

        for (var teams = 2; teams < 100; teams++)
        {
            var quizzers = faker.Random.Int(teams * 2, teams * 5);
            var cases = BuildCase(faker.Random.Number(1, teams), teams, quizzers);

            foreach (var scheduleCase in cases)
            {
                yield return Unroll(scheduleCase);
            }
        }
    }

    private static object[] Unroll((Schedule, int, int, Schedule) tuple)
    {
        return [tuple.Item2, tuple.Item3, tuple.Item1, tuple.Item4];
    }

    private static IEnumerable<(Schedule, int, int, Schedule)> BuildCase(int countChurches, int countTeams, int countQuizzers)
    {
        var churches = BuildChurches(countChurches).ToList();
        var originalTeams = BuildTeams(countTeams).ToList();
        var originalQuizzers = BuildQuizzers(countQuizzers, churches, originalTeams).ToList();

        var expectedTeams = BuildTeams(countTeams).ToList();
        var expectedQuizzers = BuildQuizzers(countQuizzers, churches, expectedTeams).ToList();

        var originalSchedule = new Schedule("Original", churches.ToDictionary(x => x.Id, x => x), originalQuizzers.ToDictionary(x => x.Id, x => x), originalTeams.ToDictionary(x => x.Id, x => x), new Dictionary<int, Round>());
        var expectedSchedule = new Schedule("Expected", churches.ToDictionary(x => x.Id, x => x), expectedQuizzers.ToDictionary(x => x.Id, x => x), expectedTeams.ToDictionary(x => x.Id, x => x), new Dictionary<int, Round>());

        yield return (
            originalSchedule,
            0,
            countTeams,
            expectedSchedule
            );
        yield return (
            originalSchedule,
            0,
            countTeams,
            expectedSchedule
            );
    }

    private static IEnumerable<Church> BuildChurches(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var id = i + 1;
            yield return new Church(id, $"Church {id.ToWords().Titleize()}");
        }
    }

    private static IEnumerable<Quizzer> BuildQuizzers(int count, IList<Church> churches, IList<Team> teams)
    {
        var church = 0;
        var team = 0;

        var faker = new Faker();

        for (var i = 0; i < count; i++)
        {
            var id = i + 1;
            var gender = id % 2 == 0 ? Gender.Male : Gender.Female;
            var firstName = faker.Name.FirstName(gender is Gender.Male ? Bogus.DataSets.Name.Gender.Male : Bogus.DataSets.Name.Gender.Female);
            var lastName = faker.Name.LastName();
            var year = DateTime.Now.Year;
            var rookieYear = faker.Random.Int(year - 5, year);
            yield return new Quizzer(id, firstName, lastName, gender, rookieYear, GetTeamId(team++, teams.Count), GetChurchId(church++, churches.Count));
        }
    }

    private static int GetChurchId(int id, int count)
    {
        return (id % count) + 1;
    }

    private static int GetTeamId(int id, int count)
    {
        var (quotient, remainder) = Math.DivRem(id, count);

        if (quotient % 2 == 0)
        {
            return remainder + 1;
        }
        else
        {
            return count - remainder;
        }
    }

    private static IEnumerable<Team> BuildTeams(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var id = i + 1;
            yield return new Team(id, $"Team {id.ToWords().Titleize()}", id.ToString("D02"), 0);
        }
    }
}
