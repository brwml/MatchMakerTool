namespace MatchMaker.Reporting.Exporters;

using System.Buffers;
using System.Globalization;
using System.Text;

using Antlr4.StringTemplate;

/// <summary>
/// StringTemplate attribute renderer for escaping markdown special characters.
/// </summary>
public class MarkdownEscapeRenderer : IAttributeRenderer
{
    /// <summary>
    /// The markdown special characters that must be escaped with a leading backslash.
    /// </summary>
    private static readonly SearchValues<char> SpecialCharacters =
        SearchValues.Create("|[]()\\*_#+-.!`{}");

    /// <summary>
    /// Escapes markdown special characters in the given string.
    /// </summary>
    /// <param name="obj">The SafeMarkdown object to render</param>
    /// <param name="formatString">Optional format string (not used)</param>
    /// <param name="culture">The culture for rendering</param>
    /// <returns>The escaped string</returns>
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

        return string.IsNullOrEmpty(value) || value.AsSpan().IndexOfAny(SpecialCharacters) < 0
            ? value
            : EscapeSpecialCharacters(value);
    }

    /// <summary>
    /// Prefixes every markdown special character in the given string with a backslash.
    /// </summary>
    /// <param name="value">The string to escape</param>
    /// <returns>The escaped string</returns>
    private static string EscapeSpecialCharacters(string value)
    {
        var escaped = new StringBuilder(value.Length + 8);

        foreach (var c in value)
        {
            if (SpecialCharacters.Contains(c))
            {
                escaped.Append('\\');
            }

            escaped.Append(c);
        }

        return escaped.ToString();
    }
}
