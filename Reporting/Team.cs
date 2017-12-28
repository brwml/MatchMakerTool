using System.Runtime.Serialization;
using System.Xml.Linq;

namespace MatchMaker.Reporting
{
    [DataContract]
    public class Team
    {
        [DataMember]
        public string Abbreviation { get; set; }

        [DataMember]
        public int Division { get; set; }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
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