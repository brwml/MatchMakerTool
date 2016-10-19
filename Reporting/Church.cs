using System.Xml.Linq;

namespace MatchMaker.Reporting
{
    public class Church
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public static Church FromXml(XElement xml)
        {
            return new Church
            {
                Id = xml.GetAttribute<int>("id"),
                Name = xml.Value
            };
        }
    }
}