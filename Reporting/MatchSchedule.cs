using System.Xml.Linq;

namespace MatchMaker.Reporting
{
    public class MatchSchedule
    {
        public int Id { get; set; }

        public int Room { get; set; }

        public int[] Teams { get; set; }

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