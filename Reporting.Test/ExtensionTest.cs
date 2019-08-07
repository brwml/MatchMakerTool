﻿namespace MatchMaker.Reporting.Test
{
    using MatchMaker.Reporting.Models;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System.Xml.Linq;

    /// <summary>
    /// Defines the <see cref="ExtensionTest" />
    /// </summary>
    [TestClass]
    public class ExtensionTest
    {
        /// <summary>
        /// The VerifyConvertXmlAttributeToInt32
        /// </summary>
        [TestMethod]
        public void VerifyConvertXmlAttributeToInt32()
        {
            var expected = 1;
            var xml = new XElement("node", new XAttribute("id", expected));
            Assert.AreEqual(expected, xml.GetAttribute<int>("id"));
        }
    }
}
