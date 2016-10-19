using System.Xml.Linq;

namespace MatchMaker.Reporting
{
    public class QuizzerResult
    {
        public int Errors { get; set; }

        public int QuizzerId { get; set; }

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