namespace MatchMaker.Reporting.Exporters
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;

    using Antlr4.StringTemplate;

    using MatchMaker.Reporting.Models;
    using MatchMaker.Utilities;

    /// <summary>
    /// Defines the <see cref="HtmlExporter" />
    /// </summary>
    public partial class HtmlExporter : IExporter
    {

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
        private const string StyleSheet = "MatchMaker.Reporting.Templates.Html.Style.css";

        /// <summary>
        /// Defines the team detail template resource key
        /// </summary>
        private const string TeamDetailTemplate = "MatchMaker.Reporting.Templates.Html.TeamDetail.stg";

        /// <summary>
        /// Defines the team summary template resource key
        /// </summary>
        private const string TeamSummaryTemplate = "MatchMaker.Reporting.Templates.Html.TeamSummary.stg";

        /// <summary>
        /// The results folder
        /// </summary>
        private const string ResultsFolder = "Results";

        /// <summary>
        /// The index file name
        /// </summary>
        private const string IndexFileName = "index.html";

        /// <summary>
        /// The quizzers folder name
        /// </summary>
        private const string QuizzersFolderName = "quizzers";

        /// <summary>
        /// The quizzers file name
        /// </summary>
        private const string QuizzersFileName = "quizzers.html";

        /// <summary>
        /// The style sheet file name
        /// </summary>
        private const string StyleSheetFileName = "style.css";

        /// <summary>
        /// The teams folder name
        /// </summary>
        private const string TeamsFolderName = "teams";

        /// <summary>
        /// The teams file name
        /// </summary>
        private const string TeamsFileName = "teams.html";

        /// <summary>
        /// Exports the <see cref="Summary"/> to a single HTML archive.
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="folder">The output folder</param>
        public void Export(Summary summary, string folder)
        {
            Arg.NotNull(summary, nameof(summary));
            Arg.NotNullOrWhiteSpace(folder, nameof(folder));

            var resultsFolder = CreateResultsFolder(folder);

            CreateResults(summary, resultsFolder);

            CreateZipFile(summary, folder, resultsFolder);
        }

        /// <summary>
        /// Creates the ZIP file.
        /// </summary>
        /// <param name="summary">The summary.</param>
        /// <param name="folder">The folder.</param>
        /// <param name="resultsFolder">The results folder.</param>
        private static void CreateZipFile(Summary summary, string folder, string resultsFolder)
        {
            var zipPath = Path.Combine(folder, $"{summary.Name} (Results).zip");

            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }

            ZipFile.CreateFromDirectory(resultsFolder, zipPath);
        }

        /// <summary>
        /// Creates the results folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns>The results folder path</returns>
        private static string CreateResultsFolder(string folder)
        {
            var resultsFolder = Path.Combine(folder, ResultsFolder);

            if (Directory.Exists(resultsFolder))
            {
                Directory.Delete(resultsFolder, true);
            }

            Directory.CreateDirectory(resultsFolder);
            return resultsFolder;
        }

        /// <summary>
        /// Create the HTML output from the given <see cref="Summary"/> instance
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="folder">The target folder</param>
        private static void CreateResults(Summary summary, string folder)
        {
            WriteStyleSheet(folder);

            WriteIndex(summary, folder);

            WriteTeamSummary(summary, folder);
            WriteTeamDetails(summary, folder);

            WriteQuizzerSummary(summary, folder);
            WriteQuizzerDetails(summary, folder);
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
            using var stream = assembly.GetManifestResourceStream(name);
            using var reader = new StreamReader(stream);
            var group = new TemplateGroupString(reader.ReadToEnd());
            group.RegisterRenderer(typeof(decimal), new DecimalAttributeRenderer());
            return group.GetInstanceOf(RootElement);
        }

        /// <summary>
        /// Writes the tournament <see cref="Summary"/> index.
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="folder">The target folder</param>
        private static void WriteIndex(Summary summary, string folder)
        {
            var template = LoadTemplate(IndexTemplate);
            template.Add("summary", summary);
            File.WriteAllText(Path.Combine(folder, IndexFileName), template.Render(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Writes the quizzer detail page.
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="quizzerSummary">The <see cref="QuizzerSummary"/> instance</param>
        /// <param name="folder">The target folder</param>
        private static void WriteQuizzerDetail(Summary summary, QuizzerSummary quizzerSummary, string folder)
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

            var template = LoadTemplate(QuizzerDetailTemplate);
            template.Add("summary", summary);
            template.Add("quizzer", new QuizzerData(quizzerSummary, quizzer));
            template.Add("details", details);

            File.WriteAllText(Path.Combine(folder, $"{quizzer.Id}.html"), template.Render(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Writes all quizzer details to the output.
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="folder">The target folder</param>
        private static void WriteQuizzerDetails(Summary summary, string folder)
        {
            folder = Path.Combine(folder, QuizzersFolderName);
            Directory.CreateDirectory(folder);

            foreach (var quizzer in summary.QuizzerSummaries)
            {
                WriteQuizzerDetail(summary, quizzer.Value, folder);
            }
        }

        /// <summary>
        /// Writes the quizzer summary page
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="folder">The target folder</param>
        private static void WriteQuizzerSummary(Summary summary, string folder)
        {
            var quizzers = summary.Result.Schedule.Quizzers;

            var quizzerData = summary.QuizzerSummaries
                                     .Join(quizzers,
                                           s => s.Key,
                                           s => s.Key,
                                           (s, q) => new QuizzerData(s.Value, q.Value))
                                     .OrderBy(x => (x.Place, x.LastName, x.FirstName));

            var template = LoadTemplate(QuizzerSummaryTemplate);
            template.Add("name", summary.Name);
            template.Add("quizzers", quizzerData);

            File.WriteAllText(Path.Combine(folder, QuizzersFileName), template.Render(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Writes the CSS style sheet
        /// </summary>
        /// <param name="folder">The target folder</param>
        private static void WriteStyleSheet(string folder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(StyleSheet);
            using var destination = File.OpenWrite(Path.Combine(folder, StyleSheetFileName));
            stream.CopyTo(destination);
        }

        /// <summary>
        /// Writes the team detail page
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="teamSummary">The <see cref="TeamSummary"/> instance</param>
        /// <param name="folder">The target folder</param>
        private static void WriteTeamDetail(Summary summary, TeamSummary teamSummary, string folder)
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

            var team = summary.Result.Schedule.Teams[teamSummary.TeamId];
            var teamData = new TeamData(team, teamSummary);

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
            template.Add("team", teamData);
            template.Add("details", details);
            template.Add("quizzers", quizzerSummaries);

            File.WriteAllText(Path.Combine(folder, $"{team.Id}.html"), template.Render(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Writes all team details.
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="folder">The target folder</param>
        private static void WriteTeamDetails(Summary summary, string folder)
        {
            folder = Path.Combine(folder, TeamsFolderName);
            Directory.CreateDirectory(folder);

            foreach (var team in summary.TeamSummaries)
            {
                WriteTeamDetail(summary, team.Value, folder);
            }
        }

        /// <summary>
        /// Writes the team summary page
        /// </summary>
        /// <param name="summary">The <see cref="Summary"/> instance</param>
        /// <param name="folder">The target folder</param>
        private static void WriteTeamSummary(Summary summary, string folder)
        {
            var teams = summary.TeamSummaries
                               .Join(summary.Result.Schedule.Teams,
                                     s => s.Key,
                                     t => t.Key,
                                     (s, t) => new TeamData(t.Value, s.Value))
                               .OrderBy(x => (x.Place, x.Name));

            var template = LoadTemplate(TeamSummaryTemplate);
            template.Add("name", summary.Name);
            template.Add("teams", teams);

            File.WriteAllText(Path.Combine(folder, TeamsFileName), template.Render(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Complete quizzer information for the tournament.
        /// </summary>
        private class QuizzerData
        {
            private readonly QuizzerSummary quizzerSummary;
            private readonly Quizzer quizzer;

            public QuizzerData(QuizzerSummary summary, Quizzer quizzer)
            {
                this.quizzerSummary = summary;
                this.quizzer = quizzer;
            }

            public int Id => this.quizzer.Id;

            public string FirstName => this.quizzer.FirstName;

            public string LastName => this.quizzer.LastName;

            public decimal AverageErrors => this.quizzerSummary.AverageErrors;

            public decimal AverageScore => this.quizzerSummary.AverageScore;

            public int Place => this.quizzerSummary.Place;

            public int TotalErrors => this.quizzerSummary.TotalErrors;

            public int TotalRounds => this.quizzerSummary.TotalRounds;

            public int TotalScore => this.quizzerSummary.TotalScore;
        }

        /// <summary>
        /// Complete team information for the tournament
        /// </summary>
        private class TeamData
        {
            private readonly Team team;
            private readonly TeamSummary teamSummary;

            public TeamData(Team team, TeamSummary summary)
            {
                this.team = team;
                this.teamSummary = summary;
            }

            public int Id => this.team.Id;

            public string Abbreviation => this.team.Abbreviation;

            public string Name => FormattableString.Invariant($"{this.team.Name} ({this.team.Abbreviation})");

            public decimal AverageErrors => this.teamSummary.AverageErrors;

            public decimal AverageScore => this.teamSummary.AverageScore;

            public int Losses => this.teamSummary.Losses;

            public int Place => this.teamSummary.Place;

            public TieBreak TieBreak => this.teamSummary.TieBreak;

            public int TotalErrors => this.teamSummary.TotalErrors;

            public int TotalRounds => this.teamSummary.TotalRounds;

            public int TotalScore => this.teamSummary.TotalScore;

            public decimal WinPercentage => this.teamSummary.WinPercentage;

            public int Wins => this.teamSummary.Wins;
        }
    }
}
