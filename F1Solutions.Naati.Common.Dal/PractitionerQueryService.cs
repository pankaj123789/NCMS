using System.Linq;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Dal.QueryHelper;

namespace F1Solutions.Naati.Common.Dal
{
   public class PractitionerQueryService : IPractitionerQueryService
    {
        private readonly IAutoMapperHelper _autoMapperHelper;

        public PractitionerQueryService(IAutoMapperHelper autoMapperHelper)
        {
            _autoMapperHelper = autoMapperHelper;
        }

        public PractitionerSearchResponse SearchPractitioner(GetPractitionerSearchRequest request)
        {
            var practitioner = new PractitionerQueryHelper();
            var response = practitioner.SearchPractitioner(request);
            return new PractitionerSearchResponse
            {
                Total = response.Key,
                Results = response.Value
            };
        }
        
        public PractitionerCountResponse CountPractitioners(GetPractitionerCountRequest request)
        {
            var practitioner = new PractitionerQueryHelper();
            var response = practitioner.GetCount(request);
            return new PractitionerCountResponse
            {
                Results = response
            };
            
        }

        public LookupTypeResponse GetLookup(PractitionerLookupType lookupType)
        {
            var practitioner = new PractitionerQueryHelper();
            var response = practitioner.GetLookup(lookupType);

            return new LookupTypeResponse { Results = response };
        }

        public GetLegacyAccreditionsResponse GetLegacyAccreditions(GetLegacyAccreditionsRequest request)
        {
            var naatiNumber = NHibernateSession.Current.Get<Person>(request.PersonId)?.Entity.NaatiNumber ?? 0;
            var acreditations = NHibernateSession.Current.Query<LegacyAccreditation>()
                .Where(x => x.NAATINumber == naatiNumber).ToList();

            return new GetLegacyAccreditionsResponse { Results = acreditations.Select(_autoMapperHelper.Mapper.Map<LegacyAccreditationDto>) };
        }
        
        public ApiPublicPractitionerSearchResponse ApiSearchPractitioner(GetApiPublicPractitionerSearchRequest request)
        {
            var practitioner = new ApiPractitionerQueryHelper(_autoMapperHelper);
            var result = practitioner.SearchPractitioner(request);
            var response = new ApiPublicPractitionerSearchResponse
            {
                Results = result
            };

            return response;
        }

        public ApiPublicPractitionerCountServiceResponse ApiCountPractitioners(GetAPiPublicPractitionerCountRequest request)
        {
            var practitioner = new ApiPractitionerQueryHelper(_autoMapperHelper);
            var response = practitioner.GetCount(request);
            return new ApiPublicPractitionerCountServiceResponse
            {
                Results = response
            };
        }

        public PublicLookupResponse ApiGetLookup(ApiPublicLookupType lookupType)
        {
            var practitioner = new ApiPractitionerQueryHelper(_autoMapperHelper);
            var response = practitioner.GetLookup(lookupType);

            return new PublicLookupResponse { Results = response };
        }

        public LanguagesResponse ApiGetLanguages(LanguagesRequest request)
        {
            var practitioner = new ApiPractitionerQueryHelper(_autoMapperHelper);
            var response = practitioner.GetLanguages(request);
            
            return response;
        }

        public GetLegacyAccreditionsResponse ApiGetLegacyAccreditions(GetLegacyAccreditionsRequest request)
        {
            var practitioner = new ApiPractitionerQueryHelper(_autoMapperHelper);
            var response = practitioner.GetLegacyAccreditions(request);

            return response;
        }
    }
}
