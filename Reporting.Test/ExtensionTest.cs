using System.Xml.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MatchMaker.Reporting.Test
{
    [TestClass]
    public class ExtensionTest
    {
        [TestMethod]
        public void VerifyConvertXmlAttributeToInt32()
        {
            var expected = 1;
            var xml = new XElement("node", new XAttribute("id", expected));
            Assert.AreEqual(expected, xml.GetAttribute<int>("id"));
        }
    }
}