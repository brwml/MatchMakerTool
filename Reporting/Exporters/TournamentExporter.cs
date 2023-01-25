namespace MatchMaker.Reporting.Exporters;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Ardalis.GuardClauses;

using ClosedXML.Excel;

using MatchMaker.Reporting.Models;

/// <summary>
/// Defines the <see cref="TournamentExporter" />. This creates another special tournament based on the results of the previous tournament.
/// </summary>
public static class TournamentExporter
{
    /// <summary>
    /// Creates a special tournament where quizzers are placed on teams with other quizzers that they would not otherwise have an opportunity to associate.
    /// </summary>
    /// <param name="summary">The <see cref="Summary"/></param>
    /// <param name="numberOfTournamentTeams">The number of tournament teams</param>
    /// <param name="numberOfAlternateTeams">The number of alternate teams</param>
    /// <param name="outputFolder">The output folder</param>
    public static void Create(Summary summary, int numberOfTournamentTeams, int numberOfAlternateTeams, string outputFolder)
    {
        Guard.Against.Null(summary);
        Guard.Against.NegativeOrZero(numberOfTournamentTeams);
        Guard.Against.NegativeOrZero(numberOfAlternateTeams);
        Guard.Against.NullOrWhiteSpace(outputFolder);

        var quizzers = GetQuizzers(summary, numberOfTournamentTeams);
        var teams = GetTeams(quizzers, numberOfAlternateTeams);

        var fileName = Path.Combine(outputFolder, FormattableString.Invariant($"{summary.Name}_TournamentTeams.xlsx"));

        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet("Teams");

        for (var column = 0; column < teams.Count; column++)
        {
            var team = teams[column];

            for (var row = 0; row < team.Count; row++)
            {
                var quizzer = team[row];
                sheet.Cell(row + 1, column + 1).SetValue(FormattableString.Invariant($"{quizzer.FirstName} {quizzer.LastName}"));
            }

            sheet.Column(column + 1).Width = 20.0;
        }

        workbook.SaveAs(fileName);
    }

    /// <summary>
    /// Distributes quizzers over teams that do not include other quizzers from their church.
    /// </summary>
    /// <param name="teams">The unsorted collection of <see cref="List{List{Quizzer}}"/></param>
    /// <returns>The sorted collection of <see cref="List{List{Quizzer}}"/></returns>
    private static List<List<Quizzer>> DistributeChurches(List<List<Quizzer>> teams)
    {
        for (var team = 0; team < teams.Count; team++)
        {
            for (var member = 0; member < teams[team].Count; member++)
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

    /// <summary>
    /// Finds a team without another quizzer from the same church
    /// </summary>
    /// <param name="quizzers">The <see cref="List{List{Quizzer}}"/></param>
    /// <param name="fromTeam">The team the quizzer was original assigned</param>
    /// <param name="quizzer">The quizzer identifier</param>
    /// <param name="church">The church identifier</param>
    /// <returns>The new team assignment</returns>
    private static int FindTeamWithoutSameChurches(List<List<Quizzer>> quizzers, int fromTeam, int quizzer, int church)
    {
        var churches = quizzers[fromTeam].Select(x => x.ChurchId).ToList();

        for (var team = 0; team < quizzers.Count; team++)
        {
            var teamQuizzer = Math.Min(quizzers[team].Count - 1, quizzer);

            if (team != fromTeam && churches.All(c => quizzers[team][teamQuizzer].ChurchId != c) && quizzers[team].All(x => x.ChurchId != church))
            {
                return team;
            }
        }

        return fromTeam;
    }

    /// <summary>
    /// Gets the list of quizzers that are not on a team in the tournament, ranked by place.
    /// </summary>
    /// <param name="summary">The <see cref="Summary"/></param>
    /// <param name="numberOfTournamentTeams">The number of tournament teams</param>
    /// <returns>The <see cref="IList{Quizzer}"/></returns>
    private static IList<Quizzer> GetQuizzers(Summary summary, int numberOfTournamentTeams)
    {
        return (from teamSummary in summary.TeamSummaries
                join quizzer in summary.Result.Schedule.Quizzers on teamSummary.Value.TeamId equals quizzer.Value.TeamId
                join quizzerSummary in summary.QuizzerSummaries on quizzer.Value.Id equals quizzerSummary.Value.QuizzerId
                where teamSummary.Value.Place > numberOfTournamentTeams
                orderby quizzerSummary.Value.Place
                select quizzer.Value).ToList();
    }

    /// <summary>
    /// Gets the team assignments
    /// </summary>
    /// <param name="quizzers">The <see cref="List{Quizzer}"/></param>
    /// <param name="numberOfAlternateTeams">The number of alternate teams</param>
    /// <returns>The <see cref="List{List{Quizzer}}"/></returns>
    private static List<List<Quizzer>> GetTeams(IList<Quizzer> quizzers, int numberOfAlternateTeams)
    {
        var teams = new List<List<Quizzer>>();

        int CalculateTeam(int index)
        {
            var row = index / numberOfAlternateTeams % 2;

            return row == 0
                ? index % numberOfAlternateTeams
                : numberOfAlternateTeams - (index % numberOfAlternateTeams) - 1;
        }

        for (var i = 0; i < numberOfAlternateTeams; i++)
        {
            teams.Add(new List<Quizzer>());
        }

        for (var i = 0; i < quizzers.Count; i++)
        {
            teams[CalculateTeam(i)].Add(quizzers[i]);
        }

        return DistributeChurches(teams);
    }
}
