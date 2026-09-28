namespace MatchMaker.Reporting.Exporters;

using System.Buffers;
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

        if (!IsWithinBaseDirectory(fullMarkdownPath, fullBasePath))
        {
            throw new InvalidOperationException(
                FormattableString.Invariant($"Tournament name results in path outside base directory: {fullMarkdownPath}"));
        }

        Trace.WriteLine($"Preparing markdown folder: {markdownFolder}");

        if (Directory.Exists(markdownFolder))
        {
            EnsureFolderOnlyContainsPriorExportArtifacts(markdownFolder);
            Directory.Delete(markdownFolder, true);
            Trace.WriteLine("Existing markdown folder deleted");
        }

        Directory.CreateDirectory(markdownFolder);
        Trace.WriteLine("Markdown folder created");
        return markdownFolder;
    }

    /// <summary>
    /// Determines whether the given path is the base directory itself or a true descendant of
    /// it, using a directory-boundary-aware comparison rather than a raw string prefix check
    /// (which would incorrectly accept sibling directories whose name happens to start with the
    /// same characters, for example "Markdown" and "MarkdownEvil").
    /// </summary>
    /// <param name="path">The fully-qualified candidate path.</param>
    /// <param name="baseDirectory">The fully-qualified base directory.</param>
    /// <returns><see langword="true"/> if <paramref name="path"/> is contained within <paramref name="baseDirectory"/>.</returns>
    private static bool IsWithinBaseDirectory(string path, string baseDirectory)
    {
        var normalizedBase = baseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        return path.Equals(normalizedBase, StringComparison.OrdinalIgnoreCase)
            || path.StartsWith(normalizedBase + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Guards against deleting a folder that was not created by a previous Markdown export, for
    /// example when the output folder is mistakenly set to the same folder as the source result
    /// files. Only files and subfolders recognized as artifacts produced by this exporter are
    /// permitted; anything else causes an exception instead of a silent recursive delete.
    /// </summary>
    /// <param name="markdownFolder">The markdown folder to validate.</param>
    private static void EnsureFolderOnlyContainsPriorExportArtifacts(string markdownFolder)
    {
        var knownFiles = new[] { IndexFileName, TeamsFileName, QuizzersFileName };
        var knownFolders = new[] { TeamsFolderName, QuizzersFolderName };

        var unexpectedFile = Directory.EnumerateFiles(markdownFolder)
            .Select(Path.GetFileName)
            .FirstOrDefault(name => !knownFiles.Contains(name, StringComparer.OrdinalIgnoreCase));

        var unexpectedFolder = Directory.EnumerateDirectories(markdownFolder)
            .Select(Path.GetFileName)
            .FirstOrDefault(name => !knownFolders.Contains(name, StringComparer.OrdinalIgnoreCase));

        if (unexpectedFile is not null || unexpectedFolder is not null)
        {
            throw new InvalidOperationException(
                FormattableString.Invariant(
                    $"The folder '{markdownFolder}' contains files or subfolders that were not created by a previous Markdown export (for example, the source folder may be the same as the output folder). Remove or rename it and try again."));
        }
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
        var template = LoadTemplate(IndexTemplate)
            .Add("summary", summary)
            .Add("name", new SafeMarkdown(summary.Name));
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
                .Add("name", new SafeMarkdown(summary.Name))
                .Add("teams", teams)
                .Add("hasEliminationTeams", teams.Any(x => x.IsElimination));

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
                Opponent = new SafeMarkdown(GetOpponentName(summary, x.Value, teamId)),
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
                .Add("name", new SafeMarkdown(summary.Name))
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
                Opponent = new SafeMarkdown(GetOpponentName(summary, x.Value, teamId)),
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
public class SafeMarkdown(string value)
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
public class MarkdownEscapeRenderer : IAttributeRenderer
{
    /// <summary>
    /// The markdown special characters that must be escaped with a leading backslash.
    /// </summary>
    private static readonly SearchValues<char> SpecialCharacters =
        SearchValues.Create("|[]()\\*_#+-.!`{}");

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

        var value = safeMarkdown.Value;

        return string.IsNullOrEmpty(value) || value.AsSpan().IndexOfAny(SpecialCharacters) < 0
            ? value
            : EscapeSpecialCharacters(value);
    }

    /// <summary>
    /// Prefixes every markdown special character in the given string with a backslash.
    /// </summary>
    /// <param name="value">The string to escape</param>
    /// <returns>The escaped string</returns>
    private static string EscapeSpecialCharacters(string value)
    {
        var escaped = new StringBuilder(value.Length + 8);

        foreach (var c in value)
        {
            if (SpecialCharacters.Contains(c))
            {
                escaped.Append('\\');
            }

            escaped.Append(c);
        }

        return escaped.ToString();
    }
}
