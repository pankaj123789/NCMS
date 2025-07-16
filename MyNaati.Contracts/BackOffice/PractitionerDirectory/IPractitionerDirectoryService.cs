//using System.Collections.Generic;
//using F1Solutions.Naati.Common.Contracts.Bl;
//using F1Solutions.Naati.Common.Contracts.Dal.DTO;
//using F1Solutions.Naati.Common.Contracts.Dal.Portal;
//using F1Solutions.Naati.Common.Contracts.Dal.Portal.Common;

//namespace MyNaati.Contracts.BackOffice.PractitionerDirectory
//{
    
//    public interface IPractitionerDirectoryService : IInterceptableservice
//    {
        
//        SearchResults<Practitioner> SearchPractitioners(PractitionerSearchCriteria criteria);

        
//        byte[] GetProfilePicture(int naatiNumber);

        
//        SearchResults<Practitioner> ExportPractitioners(PractitionerSearchCriteria criteria);

	    
//        CountResults<ValCount> CountPractitioners(PractitionerSearchCriteria criteria);

        
//        PractitionerDirectoryGetContactDetailsResponse GetPractitionerContactDetails(PractitionerDirectoryGetContactDetailsRequest request);

        
//        string GetPractitionerWorkAreas(PractitionerDirectoryGetContactDetailsRequest request);

        
//        IEnumerable<CredentialsDetailsDto> GetPractionerCredentials(int personId);
//    }
//}