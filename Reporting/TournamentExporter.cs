using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MatchMaker.Reporting
{
    public class TournamentExporter
    {
        public static void Create(Summary summary, int numberOfTournamentTeams, int numberOfAlternateTeams, string outputFolder)
        {
            var quizzers = GetQuizzers(summary, numberOfTournamentTeams);
            var teams = GetTeams(summary, quizzers, numberOfAlternateTeams);

            var fileName = Path.Combine(outputFolder, $"{summary.Name}_TournamentTeams.xlsx");

            using (var workbook = new XLWorkbook())
            {
                var sheet = workbook.AddWorksheet("Teams");

                for (var column = 0; column < teams.Count; column++)
                {
                    var team = teams[column];

                    for (var row = 0; row < team.Count; row++)
                    {
                        var quizzer = team[row];
                        sheet.Cell(row + 1, column + 1).SetValue($"{quizzer.FirstName} {quizzer.LastName} ({quizzer.ChurchId})");
                    }
                }

                workbook.SaveAs(fileName);
            }
        }

        private static List<List<Quizzer>> DistributeChurches(List<List<Quizzer>> teams)
        {
            for (int team = 0; team < teams.Count; team++)
            {
                for (int member = 0; member < teams[team].Count; member++)
                {
                    var quizzer = teams[team][member];
                    var churchId = quizzer.ChurchId;
                    var quizzerId = quizzer.Id;

                    if (teams[team].Any(x => x.ChurchId == churchId && x.Id != quizzerId))
                    {
                        var newTeam = FindTeamWithoutSameChurches(teams, team, member, churchId);

                        if (newTeam != team)
                        {
                            var newMember = Math.Min(member, teams[newTeam].Count - 1);
                            teams[team][member] = teams[newTeam][newMember];
                            teams[newTeam][newMember] = quizzer;
                        }
                    }
                }
            }

            return teams;
        }

        private static int FindTeamWithoutSameChurches(List<List<Quizzer>> quizzers, int fromTeam, int quizzer, int church)
        {
            var churches = quizzers[fromTeam].Select(x => x.ChurchId);

            for (int team = 0; team < quizzers.Count; team++)
            {
                var teamQuizzer = Math.Min(quizzers[team].Count - 1, quizzer);

                if (team != fromTeam && churches.All(c => quizzers[team][teamQuizzer].ChurchId != c) && quizzers[team].All(x => x.ChurchId != church))
                {
                    return team;
                }
            }

            return fromTeam;
        }

        private static List<Quizzer> GetQuizzers(Summary summary, int numberOfTournamentTeams)
        {
            return (from teamSummary in summary.TeamSummaries
                    join quizzer in summary.Result.Schedule.Quizzers on teamSummary.Value.TeamId equals quizzer.Value.TeamId
                    join quizzerSummary in summary.QuizzerSummaries on quizzer.Value.Id equals quizzerSummary.Value.QuizzerId
                    where teamSummary.Value.Place > numberOfTournamentTeams
                    orderby quizzerSummary.Value.Place
                    select quizzer.Value).ToList();
        }

        private static List<List<Quizzer>> GetTeams(Summary summary, List<Quizzer> quizzers, int numberOfAlternateTeams)
        {
            var teams = new List<List<Quizzer>>();

            Func<int, int> CalculateTeam = (index) =>
            {
                var row = (index / numberOfAlternateTeams) % 2;
                if (row == 0)
                    return index % numberOfAlternateTeams;
                return numberOfAlternateTeams - index % numberOfAlternateTeams - 1;
            };

            for (int i = 0; i < numberOfAlternateTeams; i++)
            {
                teams.Add(new List<Quizzer>());
            }

            for (int i = 0; i < quizzers.Count; i++)
            {
                teams[CalculateTeam(i)].Add(quizzers[i]);
            }

            return DistributeChurches(teams);
        }
    }
}