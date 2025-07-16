//using System;
//using System.Linq;
//using F1Solutions.Naati.Common.Contracts.Dal.Portal;
//using F1Solutions.Naati.Common.Contracts.Dal.Portal.Common;
//using F1Solutions.NAATI.ePortal.SAMIntegration.Data.Repositories;

//namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
//{
//    public interface IEPortalSamUserRepository
//    {
//        SearchResults<vwEPortalUser> Search(EPortalSamUserSearchCriteria criteria);
//    }

//    public class EPortalSamUserRepository : IEPortalSamUserRepository
//    {
//        private readonly SamLinqRepositoryDataContext mDataContext;

//        private const string NaatiNumber = "NaatiNumber";
//        private const string Fullname = "FullName";
//        private const string DateCreated = "WebAccountCreateDate";
//        private const string EmailAddress = "Email";

//        public EPortalSamUserRepository(SamLinqRepositoryDataContext dataContext)
//        {
//            mDataContext = dataContext;
//        }

//        #region IEPortalUserRepository Members

//        public SearchResults<vwEPortalUser> Search(EPortalSamUserSearchCriteria criteria)
//        {
//            var query = from u in mDataContext.vwEPortalUsers
//                        select u;

//            if (criteria.NaatiNumber.HasValue)
//            {
//                query = query.Where(x => x.NAATINumber == criteria.NaatiNumber);
//            }

//            if (!string.IsNullOrEmpty(criteria.FamilyName))
//            {
//                query = query.Where(x => x.Surname.StartsWith(criteria.FamilyName));
//            }

//            if (!string.IsNullOrEmpty(criteria.GivenName))
//            {
//                query = query.Where(x => x.GivenName.StartsWith(criteria.GivenName));
//            }

//            if (!string.IsNullOrEmpty(criteria.Email))
//            {
//                query = query.Where(x => x.Email.StartsWith(criteria.Email));
//            }

//            if (criteria.CreationDateFrom != DateTime.MinValue)
//            {
//                query = query.Where(x => x.WebAccountCreateDate >= criteria.CreationDateFrom);
//            }

//            if (criteria.CreationDateTo != DateTime.MinValue)
//            {
//                //WebAccountCreateDate has a time component, so can't just search by the provided criteria date
//                var modifiedToDate = criteria.CreationDateTo.Date.AddDays(1);
//                query = query.Where(x => x.WebAccountCreateDate == null || x.WebAccountCreateDate < modifiedToDate);
//            }

//            foreach (var sort in criteria.Sort)
//            {
//                switch (sort.Key)
//                {
//                    case Fullname:
//                        query = sort.Value == SortDirection.Ascending
//                            ? query.OrderBy(p => p.Surname).ThenBy(p => p.GivenName).ThenBy(p => p.OtherNames)
//                            : query.OrderByDescending(p => p.Surname).ThenByDescending(p => p.GivenName).ThenByDescending(p => p.OtherNames);
//                        break;

//                    case EmailAddress:
//                        query = sort.Value == SortDirection.Ascending
//                            ? query.OrderBy(p => p.Email)
//                            : query.OrderByDescending(p => p.Email);
//                        break;

//                    case DateCreated:
//                        query = sort.Value == SortDirection.Ascending
//                            ? query.OrderBy(p => p.WebAccountCreateDate)
//                            : query.OrderByDescending(p => p.WebAccountCreateDate);
//                        break;

//                    default:
//                        query = sort.Value == SortDirection.Ascending
//                            ? query.OrderBy(p => p.NAATINumber)
//                            : query.OrderByDescending(p => p.NAATINumber);
//                        break;
//                }
//            }

//            var totalCount = query.Count();

//            query = query.Skip(criteria.Start);

//            if (criteria.Length != -1)
//            {
//                query = query.Take(criteria.Length);
//            }

//            var results = query.ToList();

//            return new SearchResults<vwEPortalUser>
//            {
//                Results = results,
//                TotalResultsCount = totalCount
//            };
//        }

//        #endregion
//    }
//}
