using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MatchMaker.Reporting
{
    public class Round
    {
        public int Id { get; set; }

        public IDictionary<int, MatchSchedule> Matches { get; set; }
        public DateTime StartTime { get; set; }

        public static Round FromXml(XElement xml)
        {
            return new Round
            {
                Id = xml.GetAttribute<int>("id"),
                StartTime = Convert.ToDateTime(xml.Attribute("date").Value + " " + xml.Attribute("time").Value),
                Matches = xml.Elements("match").Select(x => MatchSchedule.FromXml(x)).ToDictionary(k => k.Id, v => v)
            };
        }
    }
}