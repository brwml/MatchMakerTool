namespace MatchMaker.Tool.Controllers;

using System.Collections.Generic;

using MatchMaker.Reporting.Exporters;

/// <summary>
/// A factory class to create <see cref="IExporter"/> instances.
/// </summary>
internal static class ExporterFactory
{
    /// <summary>
    /// Gets the exporters defined the <paramref name="format"/> argument.
    /// </summary>
    /// <param name="format">The output format definition</param>
    /// <returns>The <see cref="IEnumerable{IExporter}"/> instance</returns>
    public static IEnumerable<IExporter> GetExporters(OutputFormat format)
    {
        var list = new List<IExporter> { new DefaultExporter() };

        if (format.HasFlag(OutputFormat.Excel))
        {
            list.Add(new ExcelExporter());
        }

        if (format.HasFlag(OutputFormat.Html))
        {
            list.Add(new HtmlExporter());
        }

        if (format.HasFlag(OutputFormat.Pdf))
        {
            list.Add(new PdfExporter());
        }

        if (format.HasFlag(OutputFormat.Rtf))
        {
            list.Add(new RtfExporter());
        }

        if (format.HasFlag(OutputFormat.Xml))
        {
            list.Add(new XmlExporter());
        }

        return list;
    }
}
