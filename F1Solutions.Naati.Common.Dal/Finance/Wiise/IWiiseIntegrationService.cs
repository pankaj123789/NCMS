using F1Solutions.Naati.Common.Wiise;

namespace F1Solutions.Naati.Common.Dal.Finance.Wiise
{
    public interface IWiiseIntegrationService
    {
        IWiiseAccountingApi Api { get; }
    }
}
