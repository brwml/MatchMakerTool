using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Antlr4.StringTemplate;

namespace MatchMaker.Reporting
{
    public class HtmlExporter : IExporter
    {
        private const string EndOfFileMarker = "--NextPart--";
        private const string HeaderTextResource = "MatchMaker.Reporting.Templates.Html.Header.txt";
        private const string IndexTemplate = "MatchMaker.Reporting.Templates.Html.Index.stg";
        private const string QuizzerDetailTemplate = "MatchMaker.Reporting.Templates.Html.QuizzerDetail.stg";
        private const string QuizzerSummaryTemplate = "MatchMaker.Reporting.Templates.Html.QuizzerSummary.stg";
        private const string RootElement = "root";
        private const string StyleSheet = "MatchMaker.Reporting.Templates.Html.Style.txt";
        private const string TeamDetailTemplate = "MatchMaker.Reporting.Templates.Html.TeamDetail.stg";
        private const string TeamSummaryTemplate = "MatchMaker.Reporting.Templates.Html.TeamSummary.stg";

        public void Export(Summary summary, string folder)
        {
            var results = CreateResults(summary);
            File.WriteAllText(Path.Combine(folder, FormattableString.Invariant($"{summary.Name}.mhtml")), results);
        }

        private static string CreateResults(Summary summary)
        {
            var builder = new StringBuilder();

            WriteHeader(builder);

            WriteIndex(summary, builder);
            WriteTeamSummary(summary, builder);
            WriteTeamDetails(summary, builder);

            WriteQuizzerSummary(summary, builder);
            WriteQuizzerDetails(summary, builder);

            WriteStyleSheet(builder);

            builder.AppendLine(EndOfFileMarker);

            return builder.ToString();
        }

        private static string FormatQuizzerName(Quizzer quizzer)
        {
            return FormattableString.Invariant($"{quizzer.LastName}, {quizzer.FirstName}");
        }

        private static int GetOpponentId(MatchResult result, int teamId)
        {
            return result.TeamResults.First(x => x.TeamId != teamId).TeamId;
        }

        private static string GetOpponentName(Summary summary, MatchResult result, int teamId)
        {
            return
               summary.Result.Schedule.Teams.First(t => t.Key == result.TeamResults.First(r => r.TeamId != teamId).TeamId).Value.Name;
        }

        private static int GetOpponentScore(MatchResult result, int teamId)
        {
            return result.TeamResults.First(r => r.TeamId != teamId).Score;
        }

        private static object GetQuizzerErrors(MatchResult result, int quizzerId)
        {
            return result.QuizzerResults.First(x => x.QuizzerId == quizzerId).Errors;
        }

        private static int GetQuizzerScore(MatchResult result, int quizzerId)
        {
            return result.QuizzerResults.First(x => x.QuizzerId == quizzerId).Score;
        }

        private static int GetRoundNumber(MatchResult result)
        {
            return result.Round;
        }

        private static int GetTeamPlace(MatchResult result, int teamId)
        {
            return result.TeamResults.First(r => r.TeamId == teamId).Place;
        }

        private static int GetTeamScore(MatchResult result, int teamId)
        {
            return result.TeamResults.First(r => r.TeamId == teamId).Score;
        }

        private static Template LoadTemplate(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(name))
            {
                using (var reader = new StreamReader(stream))
                {
                    var group = new TemplateGroupString(reader.ReadToEnd());
                    group.RegisterRenderer(typeof(decimal), new DecimalAttributeRenderer());
                    return group.GetInstanceOf(RootElement);
                }
            }
        }

        private static StringBuilder WriteHeader(StringBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(HeaderTextResource))
            {
                using (var reader = new StreamReader(stream))
                {
                    builder.Append(reader.ReadToEnd());
                }
            }

            return builder;
        }

        private static void WriteIndex(Summary summary, StringBuilder builder)
        {
            var template = LoadTemplate(IndexTemplate);
            template.Add("summary", summary);
            builder.Append(template.Render());
        }

        private static void WriteQuizzerDetail(Summary summary, QuizzerSummary quizzerSummary, StringBuilder builder)
        {
            var quizzer = summary.Result.Schedule.Quizzers.First(x => x.Value.Id == quizzerSummary.QuizzerId).Value;
            var details = summary.Result.Matches
                .Where(x => x.Value.QuizzerResults.Any(r => r.QuizzerId == quizzer.Id))
                .OrderBy(x => x.Value.Round)
                .Select(x => new
                {
                    Round = GetRoundNumber(x.Value),
                    OpponentId = GetOpponentId(x.Value, quizzer.TeamId),
                    Opponent = GetOpponentName(summary, x.Value, quizzer.TeamId),
                    Score = GetQuizzerScore(x.Value, quizzer.Id),
                    Errors = GetQuizzerErrors(x.Value, quizzer.Id)
                });

            var quizzerData = new
            {
                quizzer.Id,
                quizzer.FirstName,
                quizzer.LastName,
                quizzerSummary.AverageErrors,
                quizzerSummary.AverageScore,
                quizzerSummary.Place
            };

            var template = LoadTemplate(QuizzerDetailTemplate);
            template.Add("summary", summary);
            template.Add("quizzer", quizzerData);
            template.Add("details", details);

            builder.Append(template.Render());
        }

        private static void WriteQuizzerDetails(Summary summary, StringBuilder builder)
        {
            foreach (var quizzer in summary.QuizzerSummaries)
            {
                WriteQuizzerDetail(summary, quizzer.Value, builder);
            }
        }

        private static void WriteQuizzerSummary(Summary summary, StringBuilder builder)
        {
            var quizzers = summary.QuizzerSummaries
                .OrderBy(x => x.Value.Place)
                .Select(x => new
                {
                    Id = x.Value.QuizzerId,
                    summary.Result.Schedule.Quizzers.First(q => q.Value.Id == x.Value.QuizzerId).Value.FirstName,
                    summary.Result.Schedule.Quizzers.First(q => q.Value.Id == x.Value.QuizzerId).Value.LastName,
                    x.Value.Place,
                    x.Value.AverageScore,
                    x.Value.AverageErrors
                });

            var template = LoadTemplate(QuizzerSummaryTemplate);
            template.Add("name", summary.Name);
            template.Add("quizzers", quizzers);

            builder.Append(template.Render());
        }

        private static void WriteStyleSheet(StringBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(StyleSheet))
            {
                using (var reader = new StreamReader(stream))
                {
                    builder.Append(reader.ReadToEnd());
                }
            }
        }

        private static void WriteTeamDetail(Summary summary, TeamSummary teamSummary, StringBuilder builder)
        {
            var details = summary.Result.Matches
                .Where(x => x.Value.TeamResults.Any(t => t.TeamId == teamSummary.TeamId))
                .OrderBy(x => x.Value.Round)
                .Select(x => new
                {
                    Round = GetRoundNumber(x.Value),
                    OpponentId = GetOpponentId(x.Value, teamSummary.TeamId),
                    Opponent = GetOpponentName(summary, x.Value, teamSummary.TeamId),
                    Score = GetTeamScore(x.Value, teamSummary.TeamId),
                    OpponentScore = GetOpponentScore(x.Value, teamSummary.TeamId),
                    Win = GetTeamPlace(x.Value, teamSummary.TeamId) == 1
                });

            var team = new
            {
                teamSummary.AverageErrors,
                teamSummary.AverageScore,
                teamSummary.Losses,
                teamSummary.Place,
                teamSummary.TeamId,
                teamSummary.WinPercentage,
                teamSummary.Wins,
                summary.Result.Schedule.Teams.First(x => x.Value.Id == teamSummary.TeamId).Value.Name
            };

            var quizzers = summary.Result.Schedule.Quizzers.Where(x => x.Value.TeamId == teamSummary.TeamId).Select(x => x.Value);
            var quizzerSummaries = summary.QuizzerSummaries
                .Where(x => quizzers.Any(q => q.Id == x.Value.QuizzerId))
                .OrderBy(x => x.Value.Place)
                .Select(x => new
                {
                    Id = x.Value.QuizzerId,
                    Name = FormatQuizzerName(quizzers.First(q => q.Id == x.Value.QuizzerId)),
                    x.Value.Place,
                    x.Value.AverageErrors,
                    x.Value.AverageScore
                });

            var template = LoadTemplate(TeamDetailTemplate);
            template.Add("summary", summary);
            template.Add("team", team);
            template.Add("details", details);
            template.Add("quizzers", quizzerSummaries);

            builder.Append(template.Render());
        }

        private static void WriteTeamDetails(Summary summary, StringBuilder builder)
        {
            foreach (var team in summary.TeamSummaries)
            {
                WriteTeamDetail(summary, team.Value, builder);
            }
        }

        private static void WriteTeamSummary(Summary summary, StringBuilder builder)
        {
            var teams = summary.TeamSummaries
                .OrderBy(kvp => kvp.Value.Place)
                .Select(kvp => new
                {
                    kvp.Value.Place,
                    kvp.Value.TeamId,
                    kvp.Value.Wins,
                    kvp.Value.Losses,
                    kvp.Value.WinPercentage,
                    kvp.Value.AverageScore,
                    kvp.Value.AverageErrors,
                    summary.Result.Schedule.Teams.Select(x => x.Value).FirstOrDefault(x => x.Id == kvp.Value.TeamId).Name
                });

            var template = LoadTemplate(TeamSummaryTemplate);
            template.Add("name", summary.Name);
            template.Add("teams", teams);

            builder.Append(template.Render());
        }

        private class DecimalAttributeRenderer : IAttributeRenderer
        {
            public string ToString(object obj, string formatString, CultureInfo culture)
            {
                return ((decimal)obj).ToString(formatString, culture);
            }
        }
    }
}