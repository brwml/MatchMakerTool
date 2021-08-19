namespace MatchMaker.Reporting.Models
{
    using System.Runtime.Serialization;
    using System.Xml.Linq;

    using MatchMaker.Utilities;

    /// <summary>
    /// Defines the <see cref="Quizzer" />
    /// </summary>
    [DataContract]
    public class Quizzer
    {
        /// <summary>
        /// Gets or sets the church identifier
        /// </summary>
        [DataMember]
        public int ChurchId { get; set; }

        /// <summary>
        /// Gets or sets the first name
        /// </summary>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the Gender
        /// </summary>
        [DataMember]
        public Gender Gender { get; set; }

        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the last name
        /// </summary>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the rookie year
        /// </summary>
        [DataMember]
        public int RookieYear { get; set; }

        /// <summary>
        /// Gets or sets the team identifier
        /// </summary>
        [DataMember]
        public int TeamId { get; set; }

        /// <summary>
        /// Creates a new <see cref="Quizzer"/> instance from an XML element.
        /// </summary>
        /// <param name="xml">The xml<see cref="XElement"/> instance</param>
        /// <returns>The <see cref="Quizzer"/> instance</returns>
        public static Quizzer FromXml(XElement xml)
        {
            Arg.NotNull(xml, nameof(xml));

            return new Quizzer
            {
                Id = xml.GetAttribute<int>("id"),
                TeamId = xml.GetElement<int>("teamID"),
                ChurchId = xml.GetElement<int>("churchID"),
                FirstName = xml.Element("firstname").Value.Trim(),
                LastName = xml.Element("lastname").Value.Trim(),
                Gender = xml.Element("gender").Value == "M" ? Gender.Male : Gender.Female,
                RookieYear = xml.GetElement<int>("rookieYear")
            };
        }
    }
}
