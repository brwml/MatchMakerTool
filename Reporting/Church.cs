using System.Runtime.Serialization;
using System.Xml.Linq;

namespace MatchMaker.Reporting
{
    [DataContract]
    public class Church
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
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