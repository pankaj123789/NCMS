using System.Runtime.Serialization;

namespace F1Solutions.Naati.Common.Contracts.Dal.Enum
{
    public enum PointsLimitTypeName
    {
        [EnumMember]
        MaxPointsPerApplication = 1,
        [EnumMember]
        MaxPointsPerYear = 2
    }
}