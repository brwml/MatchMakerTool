using System.Xml.Linq;

namespace MatchMaker.Reporting
{
    public class TeamResult
    {
        public int Errors { get; set; }

        public int Place { get; set; }

        public int Score { get; set; }

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