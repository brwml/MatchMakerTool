namespace MatchMaker.Reporting.Exporters;

using System.Globalization;

using Antlr4.StringTemplate;

/// <summary>
/// StringTemplate attribute renderer that HTML-encodes <see cref="SafeMarkdown"/>-wrapped text
/// before it is written into an HTML template, protecting against markup injection and broken
/// pages when team, church, or quizzer names contain HTML-significant characters.
/// </summary>
public class HtmlEscapeRenderer : IAttributeRenderer
{
    /// <summary>
    /// HTML-encodes the given <see cref="SafeMarkdown"/>-wrapped string.
    /// </summary>
    /// <param name="obj">The <see cref="SafeMarkdown"/> object to render</param>
    /// <param name="formatString">Optional format string (not used)</param>
    /// <param name="culture">The culture for rendering (not used)</param>
    /// <returns>The HTML-encoded string</returns>
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

        return System.Net.WebUtility.HtmlEncode(safeMarkdown.Value) ?? string.Empty;
    }
}
