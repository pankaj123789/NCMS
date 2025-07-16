using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
    public interface ISystemQueryService : IQueryService
    {
        
        GetSystemValueResponse GetSystemValue(GetSystemValueRequest request);

        
        void SetSystemValue(SetSystemValueRequest request);
        
        
        void SetSystemValues(SetSystemValuesRequest request);
        
        
        GetAllSystemValuesResponse GetAllSystemValues();
        
        
        GetConfigDetailsResponse GetConfigDetails();

        
        void ObtainGraphAccessToken(string accessCode);

        void ObtainWiiseAccessToken(string accessCode);
        		
		LanguageSearchResponse LanguageSearch(LanguageSearchRequest request);

        
        VenueSearchResponse VenueSearch(VenueSearchRequest request);

        ApiAdminSearchResponse ApiAdminSearch(int ApiAccessId = 0);

        SaveResponse SaveApiAdmin(SaveApiAdminRequest request);


        ServiceResponse<int> SaveLanguage(SaveLanguageRequest request);

        
        SaveResponse SaveVenue(SaveVenueRequest request);

        
		SkillSearchResponse SkillSearch(SkillSearchRequest getRequest);

		
		ServiceResponse<int> SaveSkill(SaveSkillRequest request);
        
        
        void ThrowException();

        /// <summary>
        /// Purpose built call to retrieve file delete report
        /// which is SP [GetRemainingStoredFilesForDeletionCount]
        /// </summary>
        /// <returns></returns>
        GenericResponse<string> GetFileDeleteReport();
    }
}