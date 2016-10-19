using System.Linq;
using System.Xml.Linq;

namespace MatchMaker.Reporting
{
    public class MatchResult
    {
        public int Id { get; set; }

        public QuizzerResult[] QuizzerResults { get; set; }

        public int Room { get; set; }
        public int Round { get; set; }

        public int ScheduleId
        {
            get
            {
                return (this.Round * 100) + this.Room;
            }
        }

        public TeamResult[] TeamResults { get; set; }

        public static MatchResult FromXml(XElement xml)
        {
            return new MatchResult
            {
                Id = xml.GetAttribute<int>("id"),
                Round = xml.GetAttribute<int>("round"),
                Room = xml.GetAttribute<int>("room"),
                TeamResults = xml.Elements("team").Select(x => TeamResult.FromXml(x)).ToArray(),
                QuizzerResults = xml.Elements("quizzer").Select(x => QuizzerResult.FromXml(x)).ToArray()
            };
        }
    }
}