namespace MatchMaker.Scheduling.Exporters;

using System;
using System.Globalization;
using System.IO;

using MatchMaker.Models;

/// <summary>
/// Exports the schedule as a Markdown file.
/// </summary>
/// <seealso cref="IScheduleExporter" />
public class MarkdownScheduleExporter : BaseScheduleExporter
{
    private const string MarkdownTemplate = "MatchMaker.Scheduling.Templates.Markdown.Document.stg";

    /// <summary>
    /// Exports the specified schedule as a Markdown file.
    /// </summary>
    /// <param name="schedule">The schedule to export.</param>
    /// <param name="folder">The target folder.</param>
    public override void Export(Schedule schedule, string folder)
    {
        var rooms = GetRooms(schedule);
        var template = LoadTemplate(MarkdownTemplate)
            .Add("title", schedule.Name)
            .Add("rooms", GetRoomHeaders(rooms))
            .Add("rows", GetRows(schedule, rooms))
            .Add("teamPairs", GetTeamPairs(schedule));

        var date = GetDate(schedule);
        if (date != null)
        {
            template.Add("date", date);
        }

        var path = Path.Combine(folder, FormattableString.Invariant($"{schedule.Name}.md"));
        File.WriteAllText(path, template.Render(CultureInfo.InvariantCulture));
    }
}
