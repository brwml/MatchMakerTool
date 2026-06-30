namespace Reporting.Test.Models;

using System.Xml.Linq;

using MatchMaker.Models;

using Xunit;

/// <summary>
/// Defines the <see cref="XmlExtensionsTests" />
/// </summary>
public class XmlExtensionsTests
{
    [Fact]
    public void VerifyConvertXmlAttributeToInt32()
    {
        var expected = 1;
        var xml = new XElement("node", new XAttribute("id", expected));
        Assert.Equal(expected, xml.GetAttribute<int>("id"));
    }

    [Fact]
    public void GetAttribute_String_ReturnsValue()
    {
        var xml = new XElement("node", new XAttribute("name", "hello"));
        Assert.Equal("hello", xml.GetAttribute<string>("name"));
    }

    [Fact]
    public void GetAttribute_MissingAttribute_ThrowsForNumericType()
    {
        var xml = new XElement("node");
        Assert.Throws<FormatException>(() => xml.GetAttribute<int>("missing"));
    }

    [Fact]
    public void GetElement_Int_ReturnsValue()
    {
        var xml = new XElement("root", new XElement("count", 42));
        Assert.Equal(42, xml.GetElement<int>("count"));
    }

    [Fact]
    public void GetElement_String_ReturnsValue()
    {
        var xml = new XElement("root", new XElement("name", "world"));
        Assert.Equal("world", xml.GetElement<string>("name"));
    }

    [Fact]
    public void GetElement_MissingElement_ThrowsForNumericType()
    {
        var xml = new XElement("root");
        Assert.Throws<FormatException>(() => xml.GetElement<int>("missing"));
    }

    [Theory]
    [InlineData("10", 10)]
    [InlineData("0", 0)]
    [InlineData("-5", -5)]
    public void GetAttribute_VariousIntegers_RoundTrip(string raw, int expected)
    {
        var xml = new XElement("node", new XAttribute("val", raw));
        Assert.Equal(expected, xml.GetAttribute<int>("val"));
    }
}
