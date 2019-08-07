namespace MatchMaker.Reporting.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines the <see cref="TieBreakReason"/>
    /// </summary>
    [DataContract]
    public enum TieBreakReason
    {
        /// <summary>
        /// Defines the no tie breaker
        /// </summary>
        [EnumMember]
        None,
        /// <summary>
        /// Defines the head-to-head tie breaker
        /// </summary>
        [EnumMember]
        HeadToHead,
        /// <summary>
        /// Defines the average score tie breaker
        /// </summary>
        [EnumMember]
        AverageScore,
        /// <summary>
        /// Defines the average errors tie breaker
        /// </summary>
        [EnumMember]
        AverageErrors
    }
}
