namespace MatchMaker.Reporting.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines the <see cref="Gender"/>
    /// </summary>
    [DataContract]
    public enum Gender
    {
        /// <summary>
        /// Defines the Unknown
        /// </summary>
        [EnumMember] Unknown,

        /// <summary>
        /// Defines the Male
        /// </summary>
        [EnumMember] Male,

        /// <summary>
        /// Defines the Female
        /// </summary>
        [EnumMember] Female
    }
}
