using System.Runtime.Serialization;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    
    public enum ApplicationFeeType
    {
        [EnumMember]
        FirstApplication,

        [EnumMember]
        PerApplication,

        [EnumMember]
        PerCredential
    }
}
