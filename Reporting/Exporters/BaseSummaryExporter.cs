namespace MatchMaker.Reporting.Exporters;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using MatchMaker.Models;
using MatchMaker.Reporting.Models;

/// <summary>
/// The base exporter class provides consistent implementations for all exporters.
/// </summary>
public abstract class BaseSummaryExporter : ISummaryExporter
{
    /// <summary>
    /// Exports the tournament summary information.
    /// </summary>
    /// <param name="summary">The tournament summary</param>
    /// <param name="folder">The root folder</param>
    public abstract void Export(Summary summary, string folder);

    /// <summary>
    /// Gets the church instance for the quizzer.
    /// </summary>
    /// <param name="summary"></param>
    /// <param name="quizzer"></param>
    /// <returns>The church</returns>
    protected static Church GetChurch(Summary summary, Quizzer quizzer)
    {
        return summary.Result.Schedule.Churches[quizzer.ChurchId];
    }

    /// <summary>
    /// Gets the team instance for the quizzer.
    /// </summary>
    /// <param name="summary"></param>
    /// <param name="quizzer"></param>
    /// <returns>The team</returns>
    protected static Team GetTeam(Summary summary, Quizzer quizzer)
    {
        return summary.Result.Schedule.Teams[quizzer.TeamId];
    }

    /// <summary>
    /// The quizzer information for the tournament.
    /// </summary>
    /// <param name="summary">The tournament summary</param>
    /// <returns>The quizzer information</returns>
    protected static IEnumerable<QuizzerInfo> GetQuizzerInfo(Summary summary)
    {
        Trace.WriteLine("Building quizzer information");
        Trace.Indent();

        try
        {
            var quizzers = summary.Result.Schedule.Quizzers;

            var quizzerInfo = summary.QuizzerSummaries
                                     .Join(quizzers,
                                           x => x.Key,
                                           x => x.Key,
                                           (s, q) => new QuizzerInfo(q.Value, s.Value, GetChurch(summary, q.Value), GetTeam(summary, q.Value)))
                                     .OrderBy(x => (x.Place, x.LastName, x.FirstName)).ToArray();

            Trace.WriteLine($"Sorted {quizzerInfo.Length} quizzers by place");

            for (var i = 1; i < quizzerInfo.Length; i++)
            {
                quizzerInfo[i].ShowPlace = quizzerInfo[i - 1].Place != quizzerInfo[i].Place;
            }

            var rookieYear = summary.Result.Schedule.Rounds.Min(x => x.Value.Date).AddDays(-180).Year;

            foreach (var quizzer in quizzerInfo)
            {
                quizzer.IsRookie = quizzer.RookieYear == rookieYear;
            }

            Trace.WriteLine($"Quizzer information prepared successfully");
            return quizzerInfo;
        }
        finally
        {
            Trace.Unindent();
        }
    }

    /// <summary>
    /// Gets the team information for the tournament.
    /// </summary>
    /// <param name="summary">The tournament summary</param>
    /// <returns>The team information</returns>
    protected static IEnumerable<TeamInfo> GetTeamInfo(Summary summary)
    {
        Trace.WriteLine("Building team information");
        Trace.Indent();

        try
        {
            var teams = summary.Result.Schedule.Teams;

            var teamInfo = summary.TeamSummaries
                          .Join(teams,
                                x => x.Key,
                                x => x.Key,
                                (s, t) => new TeamInfo(t.Value, s.Value))
                          .OrderBy(x => (x.Place, x.Name)).ToArray();

            Trace.WriteLine($"Sorted {teamInfo.Length} teams by place");

            for (var i = 1; i < teamInfo.Length; i++)
            {
                teamInfo[i].ShowPlace = teamInfo[i - 1].Place != teamInfo[i].Place;
            }

            Trace.WriteLine("Team information prepared successfully");
            return teamInfo;
        }
        finally
        {
            Trace.Unindent();
        }
    }
}
