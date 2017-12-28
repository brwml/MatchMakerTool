using System.Runtime.Serialization;
using System.Xml.Linq;

namespace MatchMaker.Reporting
{
    [DataContract]
    public class QuizzerResult
    {
        [DataMember]
        public int Errors { get; set; }

        [DataMember]
        public int QuizzerId { get; set; }

        [DataMember]
        public int Score { get; set; }

        public static QuizzerResult FromXml(XElement xml)
        {
            return new QuizzerResult
            {
                QuizzerId = xml.GetAttribute<int>("id"),
                Score = xml.GetAttribute<int>("score"),
                Errors = xml.GetAttribute<int>("errors")
            };
        }
    }
}