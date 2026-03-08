namespace MatchMaker.Tool.Controllers;

using System.Xml.Linq;

using MatchMaker.Models;
using MatchMaker.Scheduling.Exporters;
using MatchMaker.Scheduling.Tournaments;

/// <summary>
/// Defines the <see cref="SchedulingController" />
/// </summary>
internal class SchedulingController : IProcessController<ScheduleOptions>
{
    /// <summary>
    /// Processes the scheduling options
    /// </summary>
    /// <param name="options">The <see cref="ScheduleOptions"/></param>
    /// <returns>The <see cref="bool"/></returns>
    public bool Process(ScheduleOptions options)
    {
        if (options.ScheduleType == ScheduleType.RoundRobin)
        {
            var schedule = RoundRobinTournament
                .Create(LoadInputSchedule(options.InputSchedulePath), options.Rooms)
                .WithName(GetScheduleName(options.InputSchedulePath));

            var folder = options.OutputFolderPath;
            Directory.CreateDirectory(folder);

            var formats = options.OutputFormat | OutputFormat.Xml;

            ExportSchedule(schedule, folder, formats);
        }

        return true;
    }

    private static void ExportSchedule(Schedule schedule, string folder, OutputFormat formats)
    {
        if ((formats & OutputFormat.Xml) != 0)
        {
            new XmlScheduleExporter().Export(schedule, folder);
        }

        if ((formats & OutputFormat.Html) != 0)
        {
            new HtmlScheduleExporter().Export(schedule, folder);
        }

        if ((formats & OutputFormat.Pdf) != 0)
        {
            new PdfScheduleExporter().Export(schedule, folder);
        }

        if ((formats & OutputFormat.Rtf) != 0)
        {
            new RtfScheduleExporter().Export(schedule, folder);
        }

        if ((formats & OutputFormat.Markdown) != 0)
        {
            new MarkdownScheduleExporter().Export(schedule, folder);
        }
    }

    /// <summary>
    /// Loads the input schedule.
    /// </summary>
    /// <param name="schedulePath">The schedule path.</param>
    /// <returns>The schedule instance</returns>
    private static Schedule LoadInputSchedule(string schedulePath)
    {
        return Schedule.FromXml(XDocument.Load(schedulePath), GetScheduleName(schedulePath));
    }

    /// <summary>
    /// Gets the name of the schedule.
    /// </summary>
    /// <param name="schedulePath">The schedule path.</param>
    /// <returns></returns>
    private static string GetScheduleName(string schedulePath)
    {
        return Path.GetFileNameWithoutExtension(schedulePath);
    }
}
