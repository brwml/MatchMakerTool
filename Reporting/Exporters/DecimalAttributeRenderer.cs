namespace MatchMaker.Reporting.Exporters;

using System.Globalization;

using Antlr4.StringTemplate;

/// <summary>
/// Defines the <see cref="DecimalAttributeRenderer" />
/// </summary>
internal class DecimalAttributeRenderer : IAttributeRenderer
{
    /// <summary>
    /// Converts the given object to a string assuming it is a <see cref="decimal"/>.
    /// </summary>
    /// <param name="obj">The <see cref="decimal"/> <see cref="object"/> instance</param>
    /// <param name="formatString">The format <see cref="string"/></param>
    /// <param name="culture">The <see cref="CultureInfo"/> instance</param>
    /// <returns>The formatted <see cref="string"/></returns>
    public string ToString(object obj, string formatString, CultureInfo culture)
    {
        return ((decimal)obj).ToString(formatString, culture);
    }
}
