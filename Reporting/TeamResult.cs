using System.Runtime.Serialization;
using System.Xml.Linq;

namespace MatchMaker.Reporting
{
    [DataContract]
    public class TeamResult
    {
        [DataMember]
        public int Errors { get; set; }

        [DataMember]
        public int Place { get; set; }

        [DataMember]
        public int Score { get; set; }

        [DataMember]
        public int TeamId { get; set; }

        public static TeamResult FromXml(XElement xml)
        {
            return new TeamResult
            {
                TeamId = xml.GetAttribute<int>("id"),
                Score = xml.GetAttribute<int>("score"),
                Errors = xml.GetAttribute<int>("errors"),
                Place = xml.GetAttribute<int>("place")
            };
        }
    }
}