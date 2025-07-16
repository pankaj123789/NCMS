using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Institution;

namespace Ncms.Contracts
{
    public interface IInstitutionService
    {
        GenericResponse<InstitutionModel> GetInstitution(int naatiNumber);
        GenericResponse<bool> UpdateInstitution(InstitutionModel model);
        GenericResponse<bool> AddName(InstitutionModel model);
        GenericResponse<InstitutionInsertResponse> InsertInstitution(InstitutionModel model);
        GenericResponse<InstitutionInsertResponse> CheckDuplicatedInstitution(InstitutionModel model);
        GenericResponse<IEnumerable<InstitutionResultModel>> Search(InstitutionSearchRequest request);

        GenericResponse<IEnumerable<EndorsedQualificationSearchResultModel>> SearchQualifications(EndorsedQualificationSearchRequest request);

        CreateOrUpdateResponse CreateOrUpdateQualification(EndorsedQualificationRequest model);

        GenericResponse<EndorsedQualificationModel> GetEndorsedQualification(int endorsedQualificationId);
    }

}
