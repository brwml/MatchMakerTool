namespace MatchMaker.Tool.Controllers;

using System.Collections.Generic;

using MatchMaker.Reporting.Exporters;

/// <summary>
/// A factory class to create <see cref="ISummaryExporter"/> instances.
/// </summary>
internal static class ExporterFactory
{
    /// <summary>
    /// Gets the exporters defined the <paramref name="format"/> argument.
    /// </summary>
    /// <param name="format">The output format definition</param>
    /// <returns>The <see cref="IEnumerable{IExporter}"/> instance</returns>
    public static IEnumerable<ISummaryExporter> GetExporters(OutputFormat format)
    {
        var list = new List<ISummaryExporter>();

        if (format.HasFlag(OutputFormat.Excel))
        {
            list.Add(new ExcelSummaryExporter());
        }

        if (format.HasFlag(OutputFormat.Html))
        {
            list.Add(new HtmlSummaryExporter());
        }

        if (format.HasFlag(OutputFormat.Pdf))
        {
            list.Add(new PdfSummaryExporter());
        }

        if (format.HasFlag(OutputFormat.Rtf))
        {
            list.Add(new RtfSummaryExporter());
        }

        if (format.HasFlag(OutputFormat.Xml))
        {
            list.Add(new XmlSummaryExporter());
        }

        return list;
    }
}
