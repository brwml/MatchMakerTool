namespace MatchMaker.Scheduling.Teams;

using System;

using Humanizer;

using MatchMaker.Models;

public class OrderedTeamAssigner : ITeamAssigner
{
    public Schedule Create(Schedule schedule, int numberOfRooms, int numberOfTeams)
    {
        var teams = CreateTeams(GetNumberOfTeams(numberOfRooms, numberOfTeams));
        var quizzers = CreateQuizzers(schedule.Quizzers, teams);

        return new Schedule(
            schedule.Name,
            schedule.Churches,
            quizzers,
            teams,
            new Dictionary<int, Round>());
    }

    private static IDictionary<int, Quizzer> CreateQuizzers(IDictionary<int, Quizzer> quizzers, IDictionary<int, Team> teams)
    {
        var result = new Dictionary<int, Quizzer>();

        var quizzerId = 0;

        foreach (var quizzer in quizzers.OrderBy(x => x.Key).Select(x => x.Value))
        {
            quizzerId++;
            var (quotient, remainder) = Math.DivRem(quizzerId - 1, teams.Count);
            var teamId = (quotient % 2 == 0) ? remainder + 1 : teams.Count - remainder;

            result.Add(
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

        return result;
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
        if (numberOfTeams > 0)
        {
            return numberOfTeams;
        }

        return numberOfRooms * 2;
    }
}
