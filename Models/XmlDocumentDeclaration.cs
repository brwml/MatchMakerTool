namespace MatchMaker.Models;

using System.Xml.Linq;

/// <summary>
/// Represents the required XML declaration for the MatchMaker program.
/// </summary>
internal class XmlDocumentDeclaration : XDeclaration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XmlDocumentDeclaration"/> class.
    /// </summary>
    public XmlDocumentDeclaration()
        : base("1.0", "iso-8859-1", "yes")
    {
    }
}
