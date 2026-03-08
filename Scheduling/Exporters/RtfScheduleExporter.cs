namespace MatchMaker.Scheduling.Exporters;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

using Antlr4.StringTemplate;

using MatchMaker.Models;

/// <summary>
/// Exports the schedule in Rich Text Format (RTF).
/// </summary>
/// <seealso cref="IScheduleExporter" />
public class RtfScheduleExporter : IScheduleExporter
{
    private const string RtfTemplate = "MatchMaker.Scheduling.Templates.Rtf.Document.stg";
    private const string RootElement = "file";
    private const int PageWidth = 9000;
    private const int LabelColumnWidth = 1200;

    /// <summary>
    /// Exports the specified schedule in Rich Text Format (RTF).
    /// </summary>
    /// <param name="schedule">The schedule to export.</param>
    /// <param name="folder">The target folder.</param>
    public void Export(Schedule schedule, string folder)
    {
        var rooms = GetRooms(schedule);
        var rows = GetRows(schedule, rooms);
        var teamPairs = GetTeamPairs(schedule);
        var date = GetDate(schedule);
        var cellDefs = GetCellDefinitions(rooms.Count);
        var teamCellDefs = GetTeamCellDefinitions();

        var template = LoadTemplate()
            .Add("title", schedule.Name)
            .Add("rooms", rooms)
            .Add("rows", rows)
            .Add("teamPairs", teamPairs)
            .Add("cellDefs", cellDefs)
            .Add("teamCellDefs", teamCellDefs);

        if (date != null)
        {
            template.Add("date", date);
        }

        var path = Path.Combine(folder, FormattableString.Invariant($"{schedule.Name}.rtf"));
        File.WriteAllText(path, template.Render(CultureInfo.InvariantCulture));
    }

    private static List<int> GetRooms(Schedule schedule)
    {
        return schedule.Rounds.Values
            .SelectMany(r => r.Matches.Values)
            .Select(m => m.Room)
            .Distinct()
            .OrderBy(r => r)
            .ToList();
    }

    private static List<RoundRow> GetRows(Schedule schedule, List<int> rooms)
    {
        return schedule.Rounds.Values
            .OrderBy(r => r.Id)
            .Select(round =>
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

                return new RoundRow($"Round {round.Id}", cells);
            })
            .ToList();
    }

    private static List<TeamPair> GetTeamPairs(Schedule schedule)
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

    private static string? GetDate(Schedule schedule)
    {
        var firstRound = schedule.Rounds.Values.OrderBy(r => r.Id).FirstOrDefault();
        return firstRound != null
            ? firstRound.Date.ToString("MMMM d, yyyy", CultureInfo.InvariantCulture)
            : null;
    }

    private static List<string> GetCellDefinitions(int roomCount)
    {
        var cellWidth = roomCount > 0 ? (PageWidth - LabelColumnWidth) / roomCount : 0;
        var defs = new List<string> { $"\\cellx{LabelColumnWidth}" };
        for (var i = 1; i <= roomCount; i++)
        {
            defs.Add($"\\cellx{LabelColumnWidth + (i * cellWidth)}");
        }

        return defs;
    }

    private static List<string> GetTeamCellDefinitions()
    {
        return [$"\\cellx{PageWidth / 2}", $"\\cellx{PageWidth}"];
    }

    private static Template LoadTemplate()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(RtfTemplate)
            ?? throw new InvalidOperationException(FormattableString.Invariant($"The template {RtfTemplate} was not found."));
        using var reader = new StreamReader(stream);
        var group = new TemplateGroupString(reader.ReadToEnd());
        return group.GetInstanceOf(RootElement);
    }
}

internal sealed record RoundRow(string Label, IList<string> Cells);

internal sealed record TeamEntry(string Abbreviation, string Name);

internal sealed record TeamPair(TeamEntry? Left, TeamEntry? Right);
