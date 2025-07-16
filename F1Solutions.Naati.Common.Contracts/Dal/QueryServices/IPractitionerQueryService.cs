using System;
using System.ServiceModel;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
    public interface IPractitionerQueryService : IQueryService
    {
        
        PractitionerSearchResponse SearchPractitioner(GetPractitionerSearchRequest request);

        
        PractitionerCountResponse CountPractitioners(GetPractitionerCountRequest request);

    
        ApiPublicPractitionerSearchResponse ApiSearchPractitioner(GetApiPublicPractitionerSearchRequest request);

       
        ApiPublicPractitionerCountServiceResponse ApiCountPractitioners(GetAPiPublicPractitionerCountRequest request);

        
        LookupTypeResponse GetLookup(PractitionerLookupType lookupType);

        
        GetLegacyAccreditionsResponse GetLegacyAccreditions(GetLegacyAccreditionsRequest request);

  
        PublicLookupResponse ApiGetLookup(ApiPublicLookupType lookupType);

      
        GetLegacyAccreditionsResponse ApiGetLegacyAccreditions(GetLegacyAccreditionsRequest request);

   
        LanguagesResponse ApiGetLanguages(LanguagesRequest request);
    }
}
