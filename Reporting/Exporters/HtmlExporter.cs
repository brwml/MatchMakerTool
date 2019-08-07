namespace MatchMaker.Reporting.Exporters
{
    using Antlr4.StringTemplate;

    using MatchMaker.Reporting.Models;
    using MatchMaker.Utilities;

    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Defines the <see cref="HtmlExporter" />
    /// </summary>
    public class HtmlExporter : IExporter
    {
        /// <summary>
        /// Defines the end of file marker
        /// </summary>
        private const string EndOfFileMarker = "--NextPart--";

        /// <summary>
        /// Defines the header text resource key
        /// </summary>
        private const string HeaderTextResource = "MatchMaker.Reporting.Templates.Html.Header.txt";

        /// <summary>
        /// Defines the index template resource key
        /// </summary>
        private const string IndexTemplate = "MatchMaker.Reporting.Templates.Html.Index.stg";

        /// <summary>
        /// Defines the quizzer detail template resource key
        /// </summary>
        private const string QuizzerDetailTemplate = "MatchMaker.Reporting.Templates.Html.QuizzerDetail.stg";

        /// <summary>
        /// Defines the quizzer summary template resource key
        /// </summary>
        private const string QuizzerSummaryTemplate = "MatchMaker.Reporting.Templates.Html.QuizzerSummary.stg";

        /// <summary>
        /// Defines the root element name
        /// </summary>
        private const string RootElement = "root";

        /// <summary>
        /// Defines the style sheet resource key
        /// </summary>
        private const string StyleSheet = "MatchMaker.Reporting.Templates.Html.Style.txt";

        /// <summary>
        /// Defines the team detail template resource key
        /// </summary>
        private const string TeamDetailTemplate = "MatchMaker.Reporting.Templates.Html.TeamDetail.stg";

        /// <summary>
        /// Defines the team summary template resource key
        /// </summary>
        private const string TeamSummaryTemplate = "MatchMaker.Reporting.Templates.Html.TeamSummary.stg";

        /// <summary>
        /// Exports the <see cref="Summary"/> to a single HTML archive.
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="folder">The output folder</param>
        public void Export(Summary summary, string folder)
        {
            Arg.NotNull(summary, nameof(summary));

            var results = CreateResults(summary);
            File.WriteAllText(Path.Combine(folder, FormattableString.Invariant($"{summary.Name}.mhtml")), results);
        }

        /// <summary>
        /// Create the HTML output from the given <see cref="Summary"/> instance
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <returns>The HTML <see cref="string"/> output</returns>
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

        /// <summary>
        /// Formats the quizzer name
        /// </summary>
        /// <param name="quizzer">The <see cref="Quizzer"/> instance</param>
        /// <returns>The formatter quizzer name <see cref="string"/></returns>
        private static string FormatQuizzerName(Quizzer quizzer)
        {
            return FormattableString.Invariant($"{quizzer.LastName}, {quizzer.FirstName}");
        }

        /// <summary>
        /// Gets the identifier of the opponent for the given team in the specified <see cref="MatchResult"/>.
        /// </summary>
        /// <param name="result">The <see cref="MatchResult"/> instance</param>
        /// <param name="teamId">The team identifier</param>
        /// <returns>The opposing team identifier</returns>
        private static int GetOpponentId(MatchResult result, int teamId)
        {
            return result.TeamResults.First(x => x.TeamId != teamId).TeamId;
        }

        /// <summary>
        /// Gets the opponent name in the specified <see cref="MatchResult"/>.
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="result">The <see cref="MatchResult"/> instance</param>
        /// <param name="teamId">The team identifier</param>
        /// <returns>The name of the opponent of the given team</returns>
        private static string GetOpponentName(Summary summary, MatchResult result, int teamId)
        {
            return
               summary.Result.Schedule.Teams.First(t => t.Key == result.TeamResults.First(r => r.TeamId != teamId).TeamId).Value.Name;
        }

        /// <summary>
        /// Gets the opponent score
        /// </summary>
        /// <param name="result">The <see cref="MatchResult"/> instance</param>
        /// <param name="teamId">The team identifier</param>
        /// <returns>The score of the opponent</returns>
        private static int GetOpponentScore(MatchResult result, int teamId)
        {
            return result.TeamResults.First(r => r.TeamId != teamId).Score;
        }

        /// <summary>
        /// Gets the quizzer's errors in the specified <see cref="MatchResult"/>.
        /// </summary>
        /// <param name="result">The <see cref="MatchResult"/> instance</param>
        /// <param name="quizzerId">The quizzer identifier</param>
        /// <returns>The quizzer's errors</returns>
        private static int GetQuizzerErrors(MatchResult result, int quizzerId)
        {
            return result.QuizzerResults.First(x => x.QuizzerId == quizzerId).Errors;
        }

        /// <summary>
        /// Gets the quizzer's score for the specified <see cref="MatchResult"/>.
        /// </summary>
        /// <param name="result">The <see cref="MatchResult"/> instance</param>
        /// <param name="quizzerId">The quizzer identifier</param>
        /// <returns>The quizzers score</returns>
        private static int GetQuizzerScore(MatchResult result, int quizzerId)
        {
            return result.QuizzerResults.First(x => x.QuizzerId == quizzerId).Score;
        }

        /// <summary>
        /// Gets the round number for the specified <see cref="MatchResult"/>.
        /// </summary>
        /// <param name="result">The <see cref="MatchResult"/> instance</param>
        /// <returns>The round number</returns>
        private static int GetRoundNumber(MatchResult result)
        {
            return result.Round;
        }

        /// <summary>
        /// Get the team place in the specified <see cref="MatchResult"/>.
        /// </summary>
        /// <param name="result">The <see cref="MatchResult"/> instance</param>
        /// <param name="teamId">The team identifier</param>
        /// <returns>The team's placement</returns>
        private static int GetTeamPlace(MatchResult result, int teamId)
        {
            return result.TeamResults.First(r => r.TeamId == teamId).Place;
        }

        /// <summary>
        /// Gets the team's score for the specified <see cref="MatchResult"/>.
        /// </summary>
        /// <param name="result">The <see cref="MatchResult"/> instance</param>
        /// <param name="teamId">The team identifier</param>
        /// <returns>The team's score</returns>
        private static int GetTeamScore(MatchResult result, int teamId)
        {
            return result.TeamResults.First(r => r.TeamId == teamId).Score;
        }

        /// <summary>
        /// Loads the specified template
        /// </summary>
        /// <param name="name">The name of the template resource</param>
        /// <returns>The <see cref="Template"/> instance</returns>
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

        /// <summary>
        /// Writes the header text.
        /// </summary>
        /// <param name="builder">The input <see cref="StringBuilder"/></param>
        /// <returns>The <see cref="StringBuilder"/> with the header text</returns>
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

        /// <summary>
        /// Writes the tournament <see cref="Summary"/> index.
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="builder">The input <see cref="StringBuilder"/></param>
        private static void WriteIndex(Summary summary, StringBuilder builder)
        {
            var template = LoadTemplate(IndexTemplate);
            template.Add("summary", summary);
            builder.Append(template.Render(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Writes the quizzer detail page.
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="quizzerSummary">The <see cref="QuizzerSummary"/> instance</param>
        /// <param name="builder">The <see cref="StringBuilder"/> instance</param>
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

            builder.Append(template.Render(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Writes all quizzer details to the output.
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="builder">The <see cref="StringBuilder"/> instance</param>
        private static void WriteQuizzerDetails(Summary summary, StringBuilder builder)
        {
            foreach (var quizzer in summary.QuizzerSummaries)
            {
                WriteQuizzerDetail(summary, quizzer.Value, builder);
            }
        }

        /// <summary>
        /// Writes the quizzer summary page
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="builder">The <see cref="StringBuilder"/> instance</param>
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

            builder.Append(template.Render(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Writes the CSS style sheet
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> instance</param>
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

        /// <summary>
        /// Writes the team detail page
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="teamSummary">The <see cref="TeamSummary"/> instance</param>
        /// <param name="builder">The <see cref="StringBuilder"/> instance</param>
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

            builder.Append(template.Render(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Writes all team details.
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="builder">The <see cref="StringBuilder"/> instance</param>
        private static void WriteTeamDetails(Summary summary, StringBuilder builder)
        {
            foreach (var team in summary.TeamSummaries)
            {
                WriteTeamDetail(summary, team.Value, builder);
            }
        }

        /// <summary>
        /// Writes the team summary page
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="builder">The <see cref="StringBuilder"/> instance</param>
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

            builder.Append(template.Render(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Defines the <see cref="DecimalAttributeRenderer" />
        /// </summary>
        private class DecimalAttributeRenderer : IAttributeRenderer
        {
            /// <summary>
            /// Converts the given object to a string assuming it is a <see cref="decimal"/>.
            /// </summary>
            /// <param name="obj">The <see cref="decimal"/> <see cref="object"/> instance</param>
            /// <param name="formatString">The format <see cref="string"/></param>
            /// <param name="culture">The <see cref="CultureInfo"/> instance</param>
            /// <returns>The formatted <see cref="string"/></returns>
            public string ToString(object obj, string formatString, CultureInfo culture)
            {
                return ((decimal)obj).ToString(formatString, culture);
            }
        }
    }
}
