using System.Runtime.Serialization;

namespace MatchMaker.Reporting
{
    [DataContract]
    public enum TieBreakReason
    {
        [EnumMember]
        None,

        [EnumMember]
        HeadToHead,

        [EnumMember]
        AverageScore,

        [EnumMember]
        AverageErrors
    }
}