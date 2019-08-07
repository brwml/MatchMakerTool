namespace MatchMaker.Reporting.Models
{
    using MatchMaker.Utilities;

    using System.Runtime.Serialization;
    using System.Xml.Linq;

    /// <summary>
    /// Defines the <see cref="Team" />
    /// </summary>
    [DataContract]
    public class Team
    {
        /// <summary>
        /// Gets or sets the Abbreviation
        /// </summary>
        [DataMember]
        public string Abbreviation { get; set; }

        /// <summary>
        /// Gets or sets the Division
        /// </summary>
        [DataMember]
        public int Division { get; set; }

        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Creates a <see cref="Team"/> from an <see cref="XElement"/>
        /// </summary>
        /// <param name="xml">The <see cref="XElement"/></param>
        /// <returns>The <see cref="Team"/></returns>
        public static Team FromXml(XElement xml)
        {
            Arg.NotNull(xml, nameof(xml));

            return new Team
            {
                Id = xml.GetAttribute<int>("id"),
                Division = xml.GetAttribute<int>("div"),
                Abbreviation = xml.Attribute("abbrev").Value,
                Name = xml.Value
            };
        }
    }
}
