namespace MatchMaker.Reporting.Models;

using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Linq;

using Ardalis.GuardClauses;

/// <summary>
/// Defines the <see cref="Church" /> class
/// </summary>
[DataContract]
[DebuggerDisplay("Church {Name} ({Id})")]
public class Church
{
    /// <summary>
    /// Initializes an instance of the <see cref="Church"/> class.
    /// </summary>
    /// <param name="id">The identifier</param>
    /// <param name="name">The name</param>
    public Church(int id, string name)
    {
        this.Id = id;
        this.Name = name;
    }

    /// <summary>
    /// Gets or sets the Id
    /// </summary>
    [DataMember]
    public int Id { get; }

    /// <summary>
    /// Gets or sets the Name
    /// </summary>
    [DataMember]
    public string Name { get; }

    /// <summary>
    /// Gets a <see cref="Church"/> instance from an XML element
    /// </summary>
    /// <param name="xml">The xml element</param>
    /// <returns>The <see cref="Church"/></returns>
    public static Church FromXml(XElement xml)
    {
        Guard.Against.Null(xml, nameof(xml));

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
