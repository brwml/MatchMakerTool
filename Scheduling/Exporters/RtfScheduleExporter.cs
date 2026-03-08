namespace MatchMaker.Scheduling.Exporters;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using MatchMaker.Models;

/// <summary>
/// Exports the schedule in Rich Text Format (RTF).
/// </summary>
/// <seealso cref="IScheduleExporter" />
public class RtfScheduleExporter : BaseScheduleExporter
{
    private const string RtfTemplate = "MatchMaker.Scheduling.Templates.Rtf.Document.stg";
    private const int PageWidth = 9000;
    private const int LabelColumnWidth = 1200;
    private const string CellBorders = @"\clbrdrl\brdrs\brdrw15\clbrdrt\brdrs\brdrw15\clbrdrr\brdrs\brdrw15\clbrdrb\brdrs\brdrw15";
    private const string HeaderBackground = @"\clcbpat1";

    /// <summary>
    /// Exports the specified schedule in Rich Text Format (RTF).
    /// </summary>
    /// <param name="schedule">The schedule to export.</param>
    /// <param name="folder">The target folder.</param>
    public override void Export(Schedule schedule, string folder)
    {
        var rooms = GetRooms(schedule);
        var roomHeaders = GetRoomHeaders(rooms);
        var rows = GetRows(schedule, rooms);
        var teamPairs = GetTeamPairs(schedule);
        var date = GetDate(schedule);
        var widths = GetColumnWidths(rooms.Count);
        var headerCellDefs = GetHeaderCellDefs(widths);
        var dataCellDefs = GetDataCellDefs(widths);
        var teamCellDefs = GetTeamCellDefs();

        var template = LoadTemplate(RtfTemplate)
            .Add("title", schedule.Name)
            .Add("rooms", roomHeaders)
            .Add("rows", rows)
            .Add("teamPairs", teamPairs)
            .Add("headerCellDefs", headerCellDefs)
            .Add("dataCellDefs", dataCellDefs)
            .Add("teamCellDefs", teamCellDefs);

        if (date != null)
        {
            template.Add("date", date);
        }

        var path = Path.Combine(folder, FormattableString.Invariant($"{schedule.Name}.rtf"));
        File.WriteAllText(path, template.Render(CultureInfo.InvariantCulture));
    }

    private static List<int> GetColumnWidths(int roomCount)
    {
        var cellWidth = roomCount > 0 ? (PageWidth - LabelColumnWidth) / roomCount : 0;
        var widths = new List<int> { LabelColumnWidth };
        for (var i = 1; i <= roomCount; i++)
        {
            widths.Add(LabelColumnWidth + (i * cellWidth));
        }

        return widths;
    }

    private static List<string> GetHeaderCellDefs(IList<int> widths) =>
        widths.Select(w => $"{HeaderBackground}{CellBorders}\\cellx{w}").ToList();

    private static List<string> GetDataCellDefs(IList<int> widths) =>
        widths.Select(w => $"{CellBorders}\\cellx{w}").ToList();

    private static List<string> GetTeamCellDefs() =>
    [
        $"{CellBorders}\\cellx{PageWidth / 2}",
        $"{CellBorders}\\cellx{PageWidth}"
    ];
}
