using System;
using System.Xml.Linq;

namespace MatchMaker.Reporting
{
    public static class Extensions
    {
        public static T GetAttribute<T>(this XElement xml, string name)
        {
            return (T)Convert.ChangeType(xml.Attribute(name).Value, typeof(T));
        }

        public static T GetElement<T>(this XElement xml, string name)
        {
            return (T)Convert.ChangeType(xml.Element(name).Value, typeof(T));
        }
    }
}