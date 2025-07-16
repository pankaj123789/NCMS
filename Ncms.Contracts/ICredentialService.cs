using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.Person;

namespace Ncms.Contracts
{
    public interface ICredentialService
    {
        GenericResponse<CertificationPeriodModel> GetCertificationPeriod(int certificationPeriodId);
        GenericResponse<IEnumerable<CredentialResultModel>> Search(CredentialSearchRequest request);
        GenericResponse<string> GetPhotosAndExcel(CredentialSearchRequest request);
    }
}