namespace Reporting.Test.Models;

using System.Xml.Linq;

using MatchMaker.Models;

using Xunit;

/// <summary>
/// Defines the <see cref="XmlExtensionsTests" />
/// </summary>
public class XmlExtensionsTests
{
    /// <summary>
    /// The VerifyConvertXmlAttributeToInt32
    /// </summary>
    [Fact]
    public void VerifyConvertXmlAttributeToInt32()
    {
        var expected = 1;
        var xml = new XElement("node", new XAttribute("id", expected));
        Assert.Equal(expected, xml.GetAttribute<int>("id"));
    }
}
