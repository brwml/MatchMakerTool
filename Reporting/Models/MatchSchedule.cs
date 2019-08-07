namespace MatchMaker.Reporting.Models
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Xml.Linq;

    /// <summary>
    /// Defines the <see cref="MatchSchedule" />
    /// </summary>
    [DataContract]
    public class MatchSchedule
    {
        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the room identifier
        /// </summary>
        [DataMember]
        public int Room { get; set; }

        /// <summary>
        /// Gets or sets the Teams
        /// </summary>
        [DataMember]
        public IList<int> Teams { get; set; }

        /// <summary>
        /// Creates a <see cref="MatchSchedule"/> instance from an XML element.
        /// </summary>
        /// <param name="x">The <see cref="XElement"/> instance</param>
        /// <returns>The <see cref="MatchSchedule"/> instance</returns>
        internal static MatchSchedule FromXml(XElement x)
        {
            return new MatchSchedule
            {
                Id = x.GetAttribute<int>("id"),
                Room = x.GetAttribute<int>("room"),
                Teams = new[] { x.GetAttribute<int>("team1"), x.GetAttribute<int>("team2") }
            };
        }
    }
}
