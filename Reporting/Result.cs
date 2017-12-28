using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MatchMaker.Reporting
{
    [DataContract]
    public class Result
    {
        [DataMember]
        public IDictionary<int, MatchResult> Matches { get; set; }

        [IgnoreDataMember]
        public string Name => this.Schedule?.Name ?? string.Empty;

        [DataMember]
        public Schedule Schedule { get; set; }

        public static Result FromXml(IEnumerable<XDocument> documents, Schedule schedule)
        {
            return new Result
            {
                Schedule = schedule,
                Matches = documents?.SelectMany(x => LoadMatches(x))?.ToDictionary(m => m.ScheduleId, m => m) ?? new Dictionary<int, MatchResult>()
            };
        }

        private static IEnumerable<MatchResult> LoadMatches(XDocument document)
        {
            return document.XPathSelectElements("/members/results/match").Select(x => MatchResult.FromXml(x));
        }
    }
}