using System.Runtime.Serialization;
using System.Xml.Linq;

namespace MatchMaker.Reporting
{
    [DataContract]
    public class MatchSchedule
    {
        [DataMember]
        public int Id
        {
            get; set;
        }

        [DataMember]
        public int Room
        {
            get; set;
        }

        [DataMember]
        public int[] Teams
        {
            get; set;
        }

        internal static MatchSchedule FromXml(XElement x)
        {
            return new MatchSchedule
            {
                Id = x.GetAttribute<int>("id"),
                Room = x.GetAttribute<int>("room"),
                Teams = new[] { x.GetAttribute<int>("team1"), x.GetAttribute<int>("team2") }
            };
        }
    }
}