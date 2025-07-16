using System.Runtime.Serialization;

namespace F1Solutions.Naati.Common.Contracts.Dal.Enum
{
    public enum NoteType
    {
        [EnumMember]
        Panel,
        [EnumMember]
        Test,
        [EnumMember]
        NaatiEntity,
        [EnumMember]
        Application,
        [EnumMember]
        MaterialRequest,
        [EnumMember]
        MaterialRequestPublic
    }
}