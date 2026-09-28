namespace MatchMaker.Reporting.Exporters;

using System;
using System.Globalization;

using Antlr4.StringTemplate;

/// <summary>
/// StringTemplate attribute renderer that escapes <see cref="SafeMarkdown"/>-wrapped text for
/// safe inclusion in an RTF document, protecting against corrupted output when team, church, or
/// quizzer names contain RTF-significant characters (backslash and curly braces).
/// </summary>
public class RtfEscapeRenderer : IAttributeRenderer
{
    /// <summary>
    /// Escapes RTF special characters in the given <see cref="SafeMarkdown"/>-wrapped string.
    /// </summary>
    /// <param name="obj">The <see cref="SafeMarkdown"/> object to render</param>
    /// <param name="formatString">Optional format string (not used)</param>
    /// <param name="culture">The culture for rendering (not used)</param>
    /// <returns>The RTF-escaped string</returns>
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

        return string.IsNullOrEmpty(value)
            ? value
            : value.Replace("\\", "\\\\", StringComparison.Ordinal)
                .Replace("{", "\\{", StringComparison.Ordinal)
                .Replace("}", "\\}", StringComparison.Ordinal);
    }
}
