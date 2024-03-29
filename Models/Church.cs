﻿namespace MatchMaker.Models;

using System.Diagnostics;
using System.Xml.Linq;

/// <summary>
/// Defines the <see cref="Church" /> class
/// </summary>
/// <remarks>
/// Initializes an instance of the <see cref="Church"/> class.
/// </remarks>
/// <param name="id">The identifier</param>
/// <param name="name">The name</param>
[DebuggerDisplay("Church {Name} ({Id})")]
public class Church(int id, string name)
{
    /// <summary>
    /// Gets or sets the Id
    /// </summary>
    public int Id { get; } = id;

    /// <summary>
    /// Gets or sets the Name
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets a <see cref="Church"/> instance from an XML element
    /// </summary>
    /// <param name="xml">The xml element</param>
    /// <returns>The <see cref="Church"/></returns>
    public static Church FromXml(XElement xml)
    {
        var id = xml.GetAttribute<int>("id");
        var name = xml.Value;

        return new Church(id, name);
    }

    /// <summary>
    /// Converts the <see cref="Church"/> instance to XML.
    /// </summary>
    /// <returns>The <see cref="XElement"/> instance</returns>
    public XElement ToXml()
    {
        return new XElement(
            "church",
            this.Name,
            new XAttribute("id", this.Id));
    }
}
