using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;

namespace F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery
{
    public interface ILookupTypeConverterHelper : ICacheQueryService
    {
        string GetDisplayName(LookupType lookupType, int value);
    }
}
