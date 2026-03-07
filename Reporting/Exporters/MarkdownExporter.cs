namespace MatchMaker.Reporting.Exporters;

using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Antlr4.StringTemplate;

using MatchMaker.Models;
using MatchMaker.Reporting.Models;

/// <summary>
/// Defines the <see cref="MarkdownExporter" />
/// </summary>
public partial class MarkdownExporter : BaseSummaryExporter
{
    /// <summary>
    /// Defines the index template resource key
    /// </summary>
    private const string IndexTemplate = "MatchMaker.Reporting.Templates.Markdown.Index.stg";

    /// <summary>
    /// Defines the quizzer detail template resource key
    /// </summary>
    private const string QuizzerDetailTemplate = "MatchMaker.Reporting.Templates.Markdown.QuizzerDetail.stg";

    /// <summary>
    /// Defines the quizzer summary template resource key
    /// </summary>
    private const string QuizzerSummaryTemplate = "MatchMaker.Reporting.Templates.Markdown.QuizzerSummary.stg";

    /// <summary>
    /// Defines the root element name
    /// </summary>
    private const string RootElement = "root";

    /// <summary>
    /// Defines the team detail template resource key
    /// </summary>
    private const string TeamDetailTemplate = "MatchMaker.Reporting.Templates.Markdown.TeamDetail.stg";

    /// <summary>
    /// Defines the team summary template resource key
    /// </summary>
    private const string TeamSummaryTemplate = "MatchMaker.Reporting.Templates.Markdown.TeamSummary.stg";

    /// <summary>
    /// The markdown folder name
    /// </summary>
    private const string MarkdownFolderName = "Markdown";

    /// <summary>
    /// The index file name
    /// </summary>
    private const string IndexFileName = "index.md";

    /// <summary>
    /// The quizzers folder name
    /// </summary>
    private const string QuizzersFolderName = "quizzers";

    /// <summary>
    /// The quizzers file name
    /// </summary>
    private const string QuizzersFileName = "quizzers.md";

    /// <summary>
    /// The teams folder name
    /// </summary>
    private const string TeamsFolderName = "teams";

    /// <summary>
    /// The teams file name
    /// </summary>
    private const string TeamsFileName = "teams.md";

    /// <summary>
    /// Exports the <see cref="Summary"/> to markdown files.
    /// </summary>
    /// <param name="summary">The <see cref="Summary"/> instance</param>
    /// <param name="folder">The output folder</param>
    public override void Export(Summary summary, string folder)
    {
        Trace.WriteLine($"Exporting tournament '{summary.Name}' to Markdown format");
        Trace.Indent();

        try
        {
            var markdownFolder = CreateMarkdownFolder(folder, summary);

            CreateResults(summary, markdownFolder);

            Trace.WriteLine("Markdown export completed successfully");
        }
        catch (Exception ex)
        {
            Trace.TraceError($"Error during Markdown export: {ex.Message}");
            throw;
        }
        finally
        {
            Trace.Unindent();
        }
    }

    /// <summary>
    /// Creates the markdown output folder with path traversal protection.
    /// </summary>
    /// <param name="folder">The root folder.</param>
    /// <param name="summary">The summary.</param>
    /// <returns>The markdown folder path</returns>
    private static string CreateMarkdownFolder(string folder, Summary summary)
    {
        var basePath = Path.Combine(folder, MarkdownFolderName);
        var sanitizedName = SanitizePathName(summary.Name);
        var markdownFolder = Path.Combine(basePath, sanitizedName);

        var fullBasePath = Path.GetFullPath(basePath);
        var fullMarkdownPath = Path.GetFullPath(markdownFolder);

        if (!fullMarkdownPath.StartsWith(fullBasePath, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                FormattableString.Invariant($"Tournament name results in path outside base directory: {fullMarkdownPath}"));
        }

        Trace.WriteLine($"Preparing markdown folder: {markdownFolder}");

        if (Directory.Exists(markdownFolder))
        {
            Directory.Delete(markdownFolder, true);
            Trace.WriteLine("Existing markdown folder deleted");
        }

        Directory.CreateDirectory(markdownFolder);
        Trace.WriteLine("Markdown folder created");
        return markdownFolder;
    }

    /// <summary>
    /// Sanitizes a tournament/file name to prevent path traversal attacks.
    /// Removes invalid file system characters and path separators.
    /// </summary>
    /// <param name="name">The name to sanitize</param>
    /// <returns>A sanitized name safe for use in file paths</returns>
    private static string SanitizePathName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "Tournament";
        }

        var invalidChars = Path.GetInvalidPathChars();
        var sanitized = new StringBuilder();

        foreach (var c in name)
        {
            if (!invalidChars.Contains(c) && c != '.' && c != '/')
            {
                sanitized.Append(c);
            }
        }

        var result = sanitized.ToString().Trim();
        return string.IsNullOrWhiteSpace(result) ? "Tournament" : result;
    }

    /// <summary>
    /// Create the markdown output from the given <see cref="Summary"/> instance
    /// </summary>
    /// <param name="summary">The <see cref="Summary"/> instance</param>
    /// <param name="folder">The target folder</param>
    private static void CreateResults(Summary summary, string folder)
    {
        Trace.WriteLine("Generating markdown report files");
        Trace.Indent();

        try
        {
            WriteIndex(summary, folder);

            WriteTeamSummary(summary, folder);
            WriteTeamDetails(summary, folder);

            WriteQuizzerSummary(summary, folder);
            WriteQuizzerDetails(summary, folder);

            Trace.WriteLine("Markdown report files generated successfully");
        }
        finally
        {
            Trace.Unindent();
        }
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
    /// Loads the specified template with markdown escaping renderer registered.
    /// </summary>
    /// <param name="name">The name of the template resource</param>
    /// <returns>The <see cref="Template"/> instance</returns>
    private static Template LoadTemplate(string name)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(name) ?? throw new InvalidOperationException(FormattableString.Invariant($"The manifest resource stream {name} was not found."));
        using var reader = new StreamReader(stream);
        var group = new TemplateGroupString(reader.ReadToEnd());
        group.RegisterRenderer(typeof(decimal), new DecimalAttributeRenderer());
        group.RegisterRenderer(typeof(SafeMarkdown), new MarkdownEscapeRenderer());
        return group.GetInstanceOf(RootElement);
    }

    /// <summary>
    /// Writes the tournament <see cref="Summary"/> index.
    /// </summary>
    /// <param name="summary">The <see cref="Summary"/> instance</param>
    /// <param name="folder">The target folder</param>
    private static void WriteIndex(Summary summary, string folder)
    {
        Trace.WriteLine("Writing tournament index page");
        var template = LoadTemplate(IndexTemplate).Add("summary", summary);
        File.WriteAllText(Path.Combine(folder, IndexFileName), template.Render(CultureInfo.CurrentCulture));
        Trace.WriteLine("Index page written successfully");
    }

    /// <summary>
    /// Writes the team summary page
    /// </summary>
    /// <param name="summary">The <see cref="Summary"/> instance</param>
    /// <param name="folder">The target folder</param>
    private static void WriteTeamSummary(Summary summary, string folder)
    {
        Trace.WriteLine("Writing team summary page");
        var teams = GetTeamInfo(summary);

        var template =
            LoadTemplate(TeamSummaryTemplate)
                .Add("name", summary.Name)
                .Add("teams", teams);

        File.WriteAllText(Path.Combine(folder, TeamsFileName), template.Render(CultureInfo.CurrentCulture));
        Trace.WriteLine($"Team summary page written with {teams.Count()} teams");
    }

    /// <summary>
    /// Writes all team details.
    /// </summary>
    /// <param name="summary">The <see cref="Summary"/> instance</param>
    /// <param name="folder">The target folder</param>
    private static void WriteTeamDetails(Summary summary, string folder)
    {
        Trace.WriteLine("Writing team detail pages");
        folder = Path.Combine(folder, TeamsFolderName);
        Directory.CreateDirectory(folder);

        var teamCount = 0;
        foreach (var team in summary.TeamSummaries)
        {
            WriteTeamDetail(summary, team.Value, folder);
            teamCount++;
        }

        Trace.WriteLine($"Team detail pages written for {teamCount} teams");
    }

    /// <summary>
    /// Writes the team detail page
    /// </summary>
    /// <param name="summary">The <see cref="Summary"/> instance</param>
    /// <param name="teamSummary">The <see cref="TeamSummary"/> instance</param>
    /// <param name="folder">The target folder</param>
    private static void WriteTeamDetail(Summary summary, TeamSummary teamSummary, string folder)
    {
        var teamId = teamSummary.TeamId;
        var details = summary.Result.Matches
            .Where(x => x.Value.TeamResults.Any(t => t.TeamId == teamId))
            .OrderBy(x => x.Value.Round)
            .Select(x => new
            {
                Round = GetRoundNumber(x.Value),
                OpponentId = GetOpponentId(x.Value, teamId),
                Opponent = GetOpponentName(summary, x.Value, teamId),
                Score = GetTeamScore(x.Value, teamId),
                OpponentScore = GetOpponentScore(x.Value, teamId),
                Win = GetTeamPlace(x.Value, teamId) == 1
            });

        var team = summary.Result.Schedule.Teams[teamId];
        var teamInfo = new TeamInfo(team, teamSummary);

        var quizzers = GetQuizzerInfo(summary).Where(x => x.Team.Id == teamId);

        var template =
            LoadTemplate(TeamDetailTemplate)
                .Add("summary", summary)
                .Add("team", teamInfo)
                .Add("details", details)
                .Add("quizzers", quizzers);

        var path = Path.Combine(folder, FormattableString.Invariant($"{team.Id}.md"));
        File.WriteAllText(path, template.Render(CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Writes the quizzer summary page
    /// </summary>
    /// <param name="summary">The <see cref="Summary"/> instance</param>
    /// <param name="folder">The target folder</param>
    private static void WriteQuizzerSummary(Summary summary, string folder)
    {
        Trace.WriteLine("Writing quizzer summary page");
        var quizzers = GetQuizzerInfo(summary);

        var template =
            LoadTemplate(QuizzerSummaryTemplate)
                .Add("name", summary.Name)
                .Add("quizzers", quizzers);

        File.WriteAllText(Path.Combine(folder, QuizzersFileName), template.Render(CultureInfo.CurrentCulture));
        Trace.WriteLine($"Quizzer summary page written with {quizzers.Count()} quizzers");
    }

    /// <summary>
    /// Writes all quizzer details to the output.
    /// </summary>
    /// <param name="summary">The <see cref="Summary"/> instance</param>
    /// <param name="folder">The target folder</param>
    private static void WriteQuizzerDetails(Summary summary, string folder)
    {
        Trace.WriteLine("Writing quizzer detail pages");
        folder = Path.Combine(folder, QuizzersFolderName);
        Directory.CreateDirectory(folder);

        var quizzerCount = 0;
        foreach (var quizzer in summary.QuizzerSummaries)
        {
            WriteQuizzerDetail(summary, quizzer.Value, folder);
            quizzerCount++;
        }

        Trace.WriteLine($"Quizzer detail pages written for {quizzerCount} quizzers");
    }

    /// <summary>
    /// Writes the quizzer detail page.
    /// </summary>
    /// <param name="summary">The <see cref="Summary"/> instance</param>
    /// <param name="quizzerSummary">The <see cref="QuizzerSummary"/> instance</param>
    /// <param name="folder">The target folder</param>
    private static void WriteQuizzerDetail(Summary summary, QuizzerSummary quizzerSummary, string folder)
    {
        var quizzerId = quizzerSummary.QuizzerId;
        var quizzer = summary.Result.Schedule.Quizzers[quizzerId];
        var teamId = quizzer.TeamId;

        var details = summary.Result.Matches
            .Where(x => x.Value.QuizzerResults.Any(r => r.QuizzerId == quizzerId))
            .OrderBy(x => x.Value.Round)
            .Select(x => new
            {
                Round = GetRoundNumber(x.Value),
                OpponentId = GetOpponentId(x.Value, teamId),
                Opponent = GetOpponentName(summary, x.Value, teamId),
                Score = GetQuizzerScore(x.Value, quizzerId),
                Errors = GetQuizzerErrors(x.Value, quizzerId)
            });

        var quizzerInfo = new QuizzerInfo(quizzer, quizzerSummary, GetChurch(summary, quizzer), GetTeam(summary, quizzer));

        var template =
            LoadTemplate(QuizzerDetailTemplate)
                .Add("summary", summary)
                .Add("quizzer", quizzerInfo)
                .Add("details", details);

        var path = Path.Combine(folder, FormattableString.Invariant($"{quizzerId}.md"));
        File.WriteAllText(path, template.Render(CultureInfo.CurrentCulture));
    }
}

/// <summary>
/// Wrapper class for markdown text that needs escaping of special characters.
/// </summary>
/// <remarks>
/// Initializes a new instance of the SafeMarkdown class.
/// </remarks>
/// <param name="value">The text to be escaped when rendered</param>
internal class SafeMarkdown(string value)
{
    /// <summary>
    /// Gets the raw value without escaping.
    /// </summary>
    public string Value { get; } = value ?? string.Empty;

    /// <summary>
    /// Returns the raw value.
    /// </summary>
    public override string ToString()
    {
        return this.Value;
    }
}

/// <summary>
/// StringTemplate attribute renderer for escaping markdown special characters.
/// </summary>
internal class MarkdownEscapeRenderer : IAttributeRenderer
{
    /// <summary>
    /// Escapes markdown special characters in the given string.
    /// </summary>
    /// <param name="obj">The SafeMarkdown object to render</param>
    /// <param name="formatString">Optional format string (not used)</param>
    /// <param name="culture">The culture for rendering</param>
    /// <returns>The escaped string</returns>
    public string ToString(object obj, string formatString, CultureInfo culture)
    {
        if (obj is null)
        {
            return string.Empty;
        }

        if (obj is not SafeMarkdown safeMarkdown)
        {
            return obj.ToString() ?? string.Empty;
        }

        var str = safeMarkdown.Value;

        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        var escaped = new StringBuilder();
        foreach (var c in str)
        {
            switch (c)
            {
                case '|':
                case '[':
                case ']':
                case '(':
                case ')':
                case '\\':
                case '*':
                case '_':
                case '#':
                case '+':
                case '-':
                case '.':
                case '!':
                case '`':
                case '{':
                case '}':
                    escaped.Append('\\').Append(c);
                    break;
                default:
                    escaped.Append(c);
                    break;
            }
        }

        return escaped.ToString();
    }
}
