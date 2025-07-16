using System.Runtime.Serialization;

namespace F1Solutions.Naati.Common.Contracts.Dal.Enum
{
    public enum ObjectStatusTypeName
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        Created =1,
        [EnumMember]
        Updated = 2,
        [EnumMember]
        Deleted = 3
    }
}