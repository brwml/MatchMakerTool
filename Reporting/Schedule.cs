using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MatchMaker.Reporting
{
    public class Schedule
    {
        public IDictionary<int, Church> Churches { get; set; }
        public string Name { get; set; }
        public IDictionary<int, Quizzer> Quizzers { get; set; }
        public IDictionary<int, Round> Rounds { get; set; }
        public IDictionary<int, Team> Teams { get; set; }

        public static Schedule FromXml(XDocument document, string name)
        {
            var schedule = PopulateSchedule(new Schedule(), document);
            schedule.Name = name;
            return schedule;
        }

        private static IDictionary<int, Church> LoadChurches(XDocument document)
        {
            return document.XPathSelectElements("/members/churches/church")
                .Select(x => Church.FromXml(x))
                .ToDictionary(k => k.Id, v => v);
        }

        private static IDictionary<int, Quizzer> LoadQuizzers(XDocument document)
        {
            return document.XPathSelectElements("/members/quizzers/quizzer")
                .Select(x => Quizzer.FromXml(x))
                .ToDictionary(k => k.Id, v => v);
        }

        private static IDictionary<int, Round> LoadRounds(XDocument document)
        {
            return document.XPathSelectElements("/members/schedule/round")
                .Select(x => Round.FromXml(x))
                .ToDictionary(k => k.Id, v => v);
        }

        private static IDictionary<int, Team> LoadTeams(XDocument document)
        {
            return document.XPathSelectElements("/members/teams/team")
                .Select(x => Team.FromXml(x))
                .ToDictionary(k => k.Id, v => v);
        }

        private static Schedule PopulateSchedule(Schedule schedule, XDocument document)
        {
            schedule.Churches = LoadChurches(document);
            schedule.Teams = LoadTeams(document);
            schedule.Quizzers = LoadQuizzers(document);
            schedule.Rounds = LoadRounds(document);
            return schedule;
        }
    }
}