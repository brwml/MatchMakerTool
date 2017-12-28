using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace MatchMaker.Reporting
{
    [DataContract]
    public class Round
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public IDictionary<int, MatchSchedule> Matches { get; set; }

        [DataMember]
        public DateTime StartTime { get; set; }

        public static Round FromXml(XElement xml)
        {
            return new Round
            {
                Id = xml.GetAttribute<int>("id"),
                StartTime = ConvertDateTime(xml.Attribute("date").Value + " " + xml.Attribute("time").Value),
                Matches = xml.Elements("match").Select(x => MatchSchedule.FromXml(x)).ToDictionary(k => k.Id, v => v)
            };
        }

        private static DateTime ConvertDateTime(string dateTime)
        {
            return DateTime.TryParse(dateTime, out var result) ? result : DateTime.Today;
        }
    }
}