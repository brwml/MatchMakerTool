namespace MatchMaker.Reporting.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml.Linq;
    using System.Xml.XPath;

    using Ardalis.GuardClauses;

    /// <summary>
    /// Defines the <see cref="Schedule" />
    /// </summary>
    [DataContract]
    public class Schedule
    {
        /// <summary>
        /// Gets or sets the Churches
        /// </summary>
        [DataMember]
        public IDictionary<int, Church> Churches { get; set; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Quizzers
        /// </summary>
        [DataMember]
        public IDictionary<int, Quizzer> Quizzers { get; set; }

        /// <summary>
        /// Gets or sets the Rounds
        /// </summary>
        [DataMember]
        public IDictionary<int, Round> Rounds { get; set; }

        /// <summary>
        /// Gets or sets the Teams
        /// </summary>
        [DataMember]
        public IDictionary<int, Team> Teams { get; set; }

        /// <summary>
        /// Creates a <see cref="Schedule"/> from an <see cref="XDocument"/> and a <paramref name="name"/>.
        /// </summary>
        /// <param name="document">The <see cref="XDocument"/></param>
        /// <param name="name">The tournament name</param>
        /// <returns>The <see cref="Schedule"/></returns>
        public static Schedule FromXml(XDocument document, string name)
        {
            Guard.Against.Null(document, nameof(document));
            Guard.Against.NullOrWhiteSpace(name, nameof(name));

            var schedule = PopulateSchedule(new Schedule(), document);
            schedule.Name = name;
            return schedule;
        }

        /// <summary>
        /// Loads the churches from the <see cref="XDocument"/>
        /// </summary>
        /// <param name="document">The <see cref="XDocument"/></param>
        /// <returns>The <see cref="IDictionary{int, Church}"/></returns>
        private static IDictionary<int, Church> LoadChurches(XDocument document)
        {
            return document.XPathSelectElements("/members/churches/church")
                .Select(x => Church.FromXml(x))
                .ToDictionary(k => k.Id, v => v);
        }

        /// <summary>
        /// Loads the quizzers from the <see cref="XDocument"/>
        /// </summary>
        /// <param name="document">The <see cref="XDocument"/></param>
        /// <returns>The <see cref="IDictionary{int, Quizzer}"/></returns>
        private static IDictionary<int, Quizzer> LoadQuizzers(XDocument document)
        {
            return document.XPathSelectElements("/members/quizzers/quizzer")
                .Select(x => Quizzer.FromXml(x))
                .ToDictionary(k => k.Id, v => v);
        }

        /// <summary>
        /// Loads the rounds from the <see cref="XDocument"/>
        /// </summary>
        /// <param name="document">The <see cref="XDocument"/></param>
        /// <returns>The <see cref="IDictionary{int, Round}"/></returns>
        private static IDictionary<int, Round> LoadRounds(XDocument document)
        {
            return document.XPathSelectElements("/members/schedule/round")
                .Select(x => Round.FromXml(x))
                .ToDictionary(k => k.Id, v => v);
        }

        /// <summary>
        /// Loads the teams from the <see cref="XDocument"/>
        /// </summary>
        /// <param name="document">The <see cref="XDocument"/></param>
        /// <returns>The <see cref="IDictionary{int, Team}"/></returns>
        private static IDictionary<int, Team> LoadTeams(XDocument document)
        {
            return document.XPathSelectElements("/members/teams/team")
                .Select(x => Team.FromXml(x))
                .ToDictionary(k => k.Id, v => v);
        }

        /// <summary>
        /// Populates the schedule.
        /// </summary>
        /// <param name="schedule">The <see cref="Schedule"/></param>
        /// <param name="document">The <see cref="XDocument"/></param>
        /// <returns>The <see cref="Schedule"/></returns>
        private static Schedule PopulateSchedule(Schedule schedule, XDocument document)
        {
            schedule.Churches = LoadChurches(document);
            schedule.Teams = LoadTeams(document);
            schedule.Quizzers = LoadQuizzers(document);
            schedule.Rounds = LoadRounds(document);
            return schedule;
        }
    }

    /// <summary>
    /// Extension methods for <see cref="Schedule"/> objects
    /// </summary>
    public static class ScheduleExtensions
    {
        /// <summary>
        /// Yields the schedule object with the provided name.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="name">The name.</param>
        /// <returns>The schedule</returns>
        public static Schedule WithName(this Schedule schedule, string name)
        {
            if (schedule is not null && !string.IsNullOrWhiteSpace(name))
            {
                schedule.Name = name;
            }

            return schedule;
        }
    }
}
