using System;
using System.Globalization;
using System.Xml.Linq;

namespace MatchMaker.Reporting
{
    public static class Extensions
    {
        public static T GetAttribute<T>(this XElement xml, string name)
        {
            if (xml == null)
            {
                return default(T);
            }

            return (T)Convert.ChangeType(xml.Attribute(name).Value, typeof(T), CultureInfo.InvariantCulture);
        }

        public static T GetElement<T>(this XContainer xml, string name)
        {
            if (xml == null)
            {
                return default(T);
            }

            return (T)Convert.ChangeType(xml.Element(name).Value, typeof(T), CultureInfo.InvariantCulture);
        }
    }
}