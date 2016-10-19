using System.Xml.Linq;

namespace MatchMaker.Reporting
{
    public class Team
    {
        public string Abbreviation { get; set; }
        public int Division { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }

        public static Team FromXml(XElement xml)
        {
            return new Team
            {
                Id = xml.GetAttribute<int>("id"),
                Division = xml.GetAttribute<int>("div"),
                Abbreviation = xml.Attribute("abbrev").Value,
                Name = xml.Value
            };
        }
    }
}