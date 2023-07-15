namespace MatchMaker.Scheduling.Teams;

using Humanizer;

using MatchMaker.Models;

public class RandomTeamAssigner : ITeamAssigner
{
    public Schedule Create(Schedule schedule, int numberOfRooms, int numberOfTeams)
    {
        var teams = CreateTeams(GetNumberOfTeams(numberOfRooms, numberOfTeams));
        var quizzers = CreateQuizzers(schedule, teams);

        return new Schedule(
            schedule.Name,
            schedule.Churches,
            quizzers,
            teams,
            new Dictionary<int, Round>());
    }

    private static Dictionary<int, Quizzer> CreateQuizzers(Schedule schedule, IDictionary<int, Team> teams)
    {
        var teamId = 0;
        var quizzerId = 0;

        var quizzers = new Dictionary<int, Quizzer>();

        foreach (var quizzer in schedule.Quizzers.OrderBy(_ => Random.Shared.Next()).Select(x => x.Value))
        {
            teamId = ((teamId + 1) % teams.Count) + 1;
            quizzerId++;

            quizzers.Add(
                quizzerId,
                new Quizzer(
                    quizzerId,
                    quizzer.FirstName,
                    quizzer.LastName,
                    quizzer.Gender,
                    quizzer.RookieYear,
                    teamId,
                    quizzer.ChurchId));
        }

        return quizzers;
    }

    private static IDictionary<int, Team> CreateTeams(int numberOfTeams)
    {
        var teamMap = new Dictionary<int, Team>();

        for (var i = 1; i <= numberOfTeams; i++)
        {
            teamMap.Add(i, new Team(i, $"Team {i.ToWords().Titleize()}", i.ToString(), 0));
        }

        return teamMap;
    }

    private static int GetNumberOfTeams(int numberOfRooms, int numberOfTeams)
    {
        return Math.Max(numberOfTeams, numberOfRooms * 2);
    }
}
