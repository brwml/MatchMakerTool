namespace MatchMaker.Reporting.Models
{
    using System.Runtime.Serialization;
    using System.Xml.Linq;

    /// <summary>
    /// Defines the <see cref="QuizzerResult" />
    /// </summary>
    [DataContract]
    public class QuizzerResult
    {
        /// <summary>
        /// Gets or sets the Errors
        /// </summary>
        [DataMember]
        public int Errors { get; set; }

        /// <summary>
        /// Gets or sets the quizzer identifier
        /// </summary>
        [DataMember]
        public int QuizzerId { get; set; }

        /// <summary>
        /// Gets or sets the Score
        /// </summary>
        [DataMember]
        public int Score { get; set; }

        /// <summary>
        /// Creates a <see cref="QuizzerResult"/> instance from an XML element.
        /// </summary>
        /// <param name="xml">The xml<see cref="XElement"/> instance</param>
        /// <returns>The <see cref="QuizzerResult"/> instance</returns>
        public static QuizzerResult FromXml(XElement xml)
        {
            return new QuizzerResult
            {
                QuizzerId = xml.GetAttribute<int>("id"),
                Score = xml.GetAttribute<int>("score"),
                Errors = xml.GetAttribute<int>("errors")
            };
        }
    }
}
