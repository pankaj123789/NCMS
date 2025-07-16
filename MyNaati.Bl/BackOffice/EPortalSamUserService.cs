//using System.Linq;
//using F1Solutions.Naati.Common.Contracts.Dal.Portal;
//using F1Solutions.Naati.Common.Contracts.Dal.Portal.Common;
//using F1Solutions.Naati.Common.Dal.Portal.Repositories;
//using MyNaati.Contracts.BackOffice.PortalUser;

//namespace MyNaati.Bl.BackOffice
//{
    
//    public class EPortalSamUserService : IEPortalSamUserService
//    {
//        private readonly IEPortalSamUserRepository mUserRepository;

//        public EPortalSamUserService(IEPortalSamUserRepository userRepository)
//        {
//            mUserRepository = userRepository;
//        }

//        public SearchResults<ePortalSamUser> Search(EPortalSamUserSearchCriteria criteria)
//        {
//            var registrationResults = mUserRepository.Search(criteria);

//            var results = new SearchResults<ePortalSamUser>
//            {
//                Results = registrationResults.Results.Select(e => new ePortalSamUser
//                {
//                    NaatiNumber = e.NAATINumber.Value,
//                    FullName = string.Format("{0}; {1} {2}", e.Surname, e.GivenName, e.OtherNames),
//                    Email = e.Email,
//                    Deceased = e.Deceased,
//                    WebAccountCreateDate = e.WebAccountCreateDate
//                }).ToList(),
//                TotalResultsCount = registrationResults.TotalResultsCount
//            };

//            return results;
//        }
//    }
//}
