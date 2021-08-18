﻿namespace MatchMaker.Reporting.Exporters
{
    using System.Collections;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Antlr4.StringTemplate;

    using MatchMaker.Reporting.Models;

    /// <summary>
    /// Exports the <see cref="Summary"/> results as an RTF file.
    /// </summary>
    /// <seealso cref="MatchMaker.Reporting.Exporters.IExporter" />
    public class RtfExporter : IExporter
    {
        /// <summary>
        /// The RTF template
        /// </summary>
        private const string RtfTemplate = "MatchMaker.Reporting.Templates.Rtf.Document.stg";

        /// <summary>
        /// The root element
        /// </summary>
        private const string RootElement = "file";

        /// <summary>
        /// Exports the tournament <see cref="Summary" /> to a file or collection of files.
        /// </summary>
        /// <param name="summary">The <see cref="Summary" /> instance</param>
        /// <param name="folder">The output folder</param>
        public void Export(Summary summary, string folder)
        {
            var template = LoadTemplate();
            template.Add("summary", summary);
            template.Add("teams", GetTeams(summary));
            template.Add("quizzers", GetQuizzers(summary));
            File.WriteAllText(Path.Combine(folder, $"{summary.Name}.rtf"), template.Render());
        }

        /// <summary>
        /// Gets the quizzers for the summary instance.
        /// </summary>
        /// <param name="summary">The summary.</param>
        /// <returns>The quizzers</returns>
        private static IEnumerable GetQuizzers(Summary summary)
        {
            return summary.QuizzerSummaries.Values
                .Select(x => new
                {
                    x.AverageErrors,
                    x.AverageScore,
                    x.Place,
                    Quizzer = GetQuizzer(x.QuizzerId, summary),
                    x.TotalErrors,
                    x.TotalRounds,
                    x.TotalScore
                })
                .OrderBy(x => x.Place)
                .ToArray();
        }

        /// <summary>
        /// Gets the quizzer information.
        /// </summary>
        /// <param name="id">The quizzer identifier.</param>
        /// <param name="summary">The summary.</param>
        /// <returns>The quizzer information</returns>
        private static object GetQuizzer(int id, Summary summary)
        {
            var quizzer = summary.Result.Schedule.Quizzers[id];
            var church = summary.Result.Schedule.Churches[quizzer.ChurchId];

            return new
            {
                quizzer.FirstName,
                quizzer.Gender,
                quizzer.Id,
                quizzer.LastName,
                quizzer.RookieYear,
                FullName = $"{quizzer.LastName}, {quizzer.FirstName}",
                Church = church
            };
        }

        /// <summary>
        /// Gets the teams for the summary instance.
        /// </summary>
        /// <param name="summary">The summary.</param>
        /// <returns>The teams</returns>
        private static IEnumerable GetTeams(Summary summary)
        {
            return summary.TeamSummaries.Values
                .Select(x => new
                {
                    x.AverageErrors,
                    x.AverageScore,
                    x.Losses,
                    x.Place,
                    Team = GetTeam(x.TeamId, summary),
                    x.TieBreak,
                    x.TotalErrors,
                    x.TotalRounds,
                    x.TotalScore,
                    x.WinPercentage,
                    x.Wins
                })
                .OrderBy(x => x.Place)
                .ToArray();
        }

        /// <summary>
        /// Gets the team information from the summary instance.
        /// </summary>
        /// <param name="id">The team identifier.</param>
        /// <param name="summary">The summary.</param>
        /// <returns>The team information</returns>
        private static Team GetTeam(int id, Summary summary)
        {
            return summary.Result.Schedule.Teams[id];
        }

        /// <summary>
        /// Loads the template.
        /// </summary>
        /// <returns>The <see cref="Template"/> instance</returns>
        private static Template LoadTemplate()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(RtfTemplate))
            {
                using (var reader = new StreamReader(stream))
                {
                    var group = new TemplateGroupString(reader.ReadToEnd());
                    group.RegisterRenderer(typeof(decimal), new DecimalAttributeRenderer());
                    return group.GetInstanceOf(RootElement);
                }
            }
        }
    }
}
