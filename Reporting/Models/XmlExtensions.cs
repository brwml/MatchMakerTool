namespace MatchMaker.Reporting.Models;

using System;
using System.Globalization;
using System.Xml.Linq;

using Ardalis.GuardClauses;

/// <summary>
/// Defines the <see cref="XmlExtensions" />
/// </summary>
public static class XmlExtensions
{
    /// <summary>
    /// Gets the attribute value by type
    /// </summary>
    /// <typeparam name="T">The output type of the value</typeparam>
    /// <param name="xml">The <see cref="XElement"/></param>
    /// <param name="name">The name of the attribute</param>
    /// <returns>The <see cref="T"/> value instance</returns>
    public static T GetAttribute<T>(this XElement xml, string name)
    {
        Guard.Against.Null(xml, nameof(xml));
        Guard.Against.NullOrWhiteSpace(name, nameof(name));

        return (T)Convert.ChangeType(xml.Attribute(name)?.Value ?? string.Empty, typeof(T), CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Gets the value of an element within an XML container.
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="xml">The <see cref="XContainer"/></param>
    /// <param name="name">The name of the element</param>
    /// <returns>The <see cref="T"/> value instance</returns>
    public static T GetElement<T>(this XContainer xml, string name)
    {
        Guard.Against.Null(xml, nameof(xml));
        Guard.Against.NullOrWhiteSpace(name, nameof(name));

        return (T)Convert.ChangeType(xml.Element(name)?.Value ?? string.Empty, typeof(T), CultureInfo.InvariantCulture);
    }
}
