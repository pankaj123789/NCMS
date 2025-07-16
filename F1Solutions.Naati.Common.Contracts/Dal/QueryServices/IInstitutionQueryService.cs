using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
    public interface IInstitutionQueryService : IQueryService
    {
        
        InstitutionDto GetInstitution(int naatiNumber);

        
        void UpdateInstitution(InstitutionDto model);

        
        AddNameResponse AddName(InstitutionDto model);

        
        InstitutionInsertResponse InsertInstitution(InstitutionDto model);

        
        InstitutionInsertResponse CheckDuplicatedInstitution(InstitutionDto model);

        
        InstituteSearchResponse SearchInstitute(GetInstituteSearchRequest request);

        
        ServiceResponse<IEnumerable<EndorsedQualificationSearchResultDto>> SearchEndorsedQualification(GetEndorsedQualificationSearchRequest request);

        
        CreateOrUpdateResponse CreateOrUpdateQualification(EndorsedQualificationDto request);

        
        ServiceResponse<EndorsedQualificationDto> GetEndorsedQualification(int endorsedQualificationId);
    }
}
