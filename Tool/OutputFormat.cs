namespace MatchMaker.Tool;

using System;

/// <summary>
/// Defines the <see cref="OutputFormat"/>
/// </summary>
[Flags]
internal enum OutputFormat
{
    /// <summary>
    /// Defines no output format
    /// </summary>
    None = 0,
    /// <summary>
    /// Defines the Excel output format
    /// </summary>
    Excel = 1,
    /// <summary>
    /// Defines the HTML output format
    /// </summary>
    Html = 2,
    /// <summary>
    /// Defines the PDF output format
    /// </summary>
    Pdf = 4,
    /// <summary>
    /// Defines the RTF output format
    /// </summary>
    Rtf = 8,
    /// <summary>
    /// Defines the XML output format
    /// </summary>
    Xml = 16,
    /// <summary>
    /// Defines all output formats
    /// </summary>
    All = Excel | Html | Pdf | Rtf | Xml
}
