namespace MatchMaker.Reporting.Models
{
    using System.Runtime.Serialization;
    using System.Xml.Linq;

    using MatchMaker.Utilities;

    /// <summary>
    /// Defines the <see cref="Church" /> class
    /// </summary>
    [DataContract]
    public class Church
    {
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
        /// Gets a <see cref="Church"/> instance from an XML element
        /// </summary>
        /// <param name="xml">The xml element</param>
        /// <returns>The <see cref="Church"/></returns>
        public static Church FromXml(XElement xml)
        {
            Arg.NotNull(xml, nameof(xml));

            return new Church
            {
                Id = xml.GetAttribute<int>("id"),
                Name = xml.Value
            };
        }
    }
}
