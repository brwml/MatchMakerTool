using System.Xml.Linq;

namespace MatchMaker.Reporting
{
    public class Quizzer
    {
        public int ChurchId { get; set; }
        public string FirstName { get; set; }
        public Gender Gender { get; set; }
        public int Id { get; set; }

        public string LastName { get; set; }
        public int RookieYear { get; set; }
        public int TeamId { get; set; }

        public static Quizzer FromXml(XElement xml)
        {
            return new Quizzer
            {
                Id = xml.GetAttribute<int>("id"),
                TeamId = xml.GetElement<int>("teamID"),
                ChurchId = xml.GetElement<int>("churchID"),
                FirstName = xml.Element("firstname").Value,
                LastName = xml.Element("lastname").Value,
                Gender = xml.Element("gender").Value == "M" ? Gender.Male : Gender.Female,
                RookieYear = xml.GetElement<int>("rookieYear")
            };
        }
    }
}