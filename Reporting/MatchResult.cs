using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace MatchMaker.Reporting
{
    [DataContract]
    public class MatchResult
    {
        [DataMember]
        public int Id
        {
            get; set;
        }

        [DataMember]
        public QuizzerResult[] QuizzerResults
        {
            get; set;
        }

        [DataMember]
        public int Room
        {
            get; set;
        }

        [DataMember]
        public int Round
        {
            get; set;
        }

        [IgnoreDataMember]
        public int ScheduleId => (this.Round * 100) + this.Room;

        [DataMember]
        public TeamResult[] TeamResults
        {
            get; set;
        }

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