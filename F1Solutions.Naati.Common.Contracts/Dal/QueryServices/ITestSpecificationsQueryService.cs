using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Dal.Domain;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    public interface ITestSpecificationsQueryService
    {
        IList<CredentialType> GetData();
        GenericResponse<string> WriteData(IList<CredentialType> credentialTypes);
    }
}
