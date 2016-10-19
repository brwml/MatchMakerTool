using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MatchMaker.Reporting
{
    public class Result
    {
        public IDictionary<int, MatchResult> Matches { get; set; }

        public string Name
        {
            get
            {
                return this.Schedule?.Name ?? string.Empty;
            }
        }

        public Schedule Schedule { get; set; }

        public static Result FromXml(IEnumerable<XDocument> documents, Schedule schedule)
        {
            var result = new Result();
            result.Schedule = schedule;
            result.Matches = documents?.SelectMany(x => LoadMatches(x))?.ToDictionary(m => m.ScheduleId, m => m)
                ?? new Dictionary<int, MatchResult>();
            return result;
        }

        private static IEnumerable<MatchResult> LoadMatches(XDocument document)
        {
            return document.XPathSelectElements("/members/results/match").Select(x => MatchResult.FromXml(x));
        }
    }
}