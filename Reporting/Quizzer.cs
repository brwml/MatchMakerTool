using System.Runtime.Serialization;
using System.Xml.Linq;

namespace MatchMaker.Reporting
{
    [DataContract]
    public class Quizzer
    {
        [DataMember]
        public int ChurchId
        {
            get; set;
        }

        [DataMember]
        public string FirstName
        {
            get; set;
        }

        [DataMember]
        public Gender Gender
        {
            get; set;
        }

        [DataMember]
        public int Id
        {
            get; set;
        }

        [DataMember]
        public string LastName
        {
            get; set;
        }

        [DataMember]
        public int RookieYear
        {
            get; set;
        }

        [DataMember]
        public int TeamId
        {
            get; set;
        }

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