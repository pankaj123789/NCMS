using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
    public interface ICredentialQueryService : IQueryService
    {
        GetCertificationPeriodResponse GetCertificationPeriod(int certificationPeriodId);
        
        GetCredentialTypeResponse GetCredentialType(int credentialTypeId);

        CredentialSearchResponse SearchCredential(GetCredentialSearchRequest request);

        /// <summary>
        /// verify a passed in document number to return the name credential and date of credential
        /// </summary>
        /// <param name="documentNumber"></param>
        /// <returns></returns>
        GenericResponse<VerifyDocumentResponse> VerifyCredentialDocument(VerifyDocumentRequest verifyDocumentRequest);
    }
}