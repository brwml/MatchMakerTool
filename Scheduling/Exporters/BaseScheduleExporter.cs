namespace MatchMaker.Scheduling.Exporters;

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

using Antlr4.StringTemplate;

using MatchMaker.Models;

/// <summary>
/// Base class for schedule exporters that provides shared data-preparation helpers.
/// </summary>
public abstract class BaseScheduleExporter : IScheduleExporter
{
    /// <inheritdoc/>
    public abstract void Export(Schedule schedule, string folder);

    /// <summary>
    /// Returns the distinct, sorted room numbers used across all rounds.
    /// </summary>
    private protected static List<int> GetRooms(Schedule schedule) =>
        schedule.Rounds.Values
            .SelectMany(r => r.Matches.Values)
            .Select(m => m.Room)
            .Distinct()
            .OrderBy(r => r)
            .ToList();

    /// <summary>
    /// Converts raw room numbers to display strings ("Room 1", "Room 2", …).
    /// </summary>
    private protected static List<string> GetRoomHeaders(IList<int> rooms) =>
        rooms.Select(r => $"Room {r}").ToList();

    /// <summary>
    /// Builds one <see cref="RoundRow"/> per round containing the match cell text for each room.
    /// </summary>
    private protected static List<RoundRow> GetRows(Schedule schedule, IList<int> rooms)
    {
        return schedule.Rounds.Values
            .OrderBy(r => r.Id)
            .Select((round, index) =>
            {
                var matchByRoom = round.Matches.Values.ToDictionary(m => m.Room);
                var cells = rooms.Select(room =>
                {
                    if (matchByRoom.TryGetValue(room, out var match) && match.Teams.Count >= 2)
                    {
                        var team1 = schedule.Teams.TryGetValue(match.Teams[0], out var t1) ? t1.Abbreviation : "?";
                        var team2 = schedule.Teams.TryGetValue(match.Teams[1], out var t2) ? t2.Abbreviation : "?";
                        return $"{team1} - {team2}";
                    }

                    return string.Empty;
                }).ToList();

                return new RoundRow($"Round {round.Id}", cells, index % 2 != 0);
            })
            .ToList();
    }

    /// <summary>
    /// Returns teams sorted by abbreviation, paired for a two-column layout.
    /// </summary>
    private protected static List<TeamPair> GetTeamPairs(Schedule schedule)
    {
        var teams = schedule.Teams.Values
            .OrderBy(t => t.Abbreviation)
            .Select(t => new TeamEntry(t.Abbreviation, t.Name))
            .ToList();

        var pairs = new List<TeamPair>();
        for (var i = 0; i < teams.Count; i += 2)
        {
            pairs.Add(new TeamPair(
                teams[i],
                i + 1 < teams.Count ? teams[i + 1] : null));
        }

        return pairs;
    }

    /// <summary>
    /// Returns the date of the first round formatted for display, or <c>null</c> if there are no rounds.
    /// </summary>
    private protected static string? GetDate(Schedule schedule)
    {
        var firstRound = schedule.Rounds.Values.OrderBy(r => r.Id).FirstOrDefault();
        return firstRound != null
            ? firstRound.Date.ToString("MMMM d, yyyy", CultureInfo.InvariantCulture)
            : null;
    }

    /// <summary>
    /// Loads a StringTemplate group from an embedded resource and returns the named root template.
    /// </summary>
    private protected static Template LoadTemplate(string resourceName, string rootElement = "file")
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                FormattableString.Invariant($"The template '{resourceName}' was not found as an embedded resource."));
        using var reader = new StreamReader(stream);
        var group = new TemplateGroupString(reader.ReadToEnd());
        return group.GetInstanceOf(rootElement);
    }
}

/// <summary>Represents one row of the schedule table (one round).</summary>
internal sealed record RoundRow(string Label, IList<string> Cells, bool IsOdd);

/// <summary>A team abbreviation + full name pair used in the team list.</summary>
internal sealed record TeamEntry(string Abbreviation, string Name);

/// <summary>A left/right pair of <see cref="TeamEntry"/> instances for two-column layout.</summary>
internal sealed record TeamPair(TeamEntry? Left, TeamEntry? Right);
