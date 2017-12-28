using System.Runtime.Serialization;

namespace MatchMaker.Reporting
{
    [DataContract]
    public enum Gender
    {
        [EnumMember]
        Unknown,

        [EnumMember]
        Male,

        [EnumMember]
        Female
    }
}