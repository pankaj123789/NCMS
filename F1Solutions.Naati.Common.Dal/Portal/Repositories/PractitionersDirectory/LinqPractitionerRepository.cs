//using System.Collections.Generic;
//using System.Linq;
//using F1Solutions.Naati.Common.Contracts.Dal.Enum;
//using F1Solutions.Naati.Common.Contracts.Dal.Portal;
//using F1Solutions.NAATI.ePortal.SAMIntegration.Data.Repositories.PractitionersDirectory;
//using SortDirection  = F1Solutions.Naati.Common.Contracts.Dal.Portal.Common.SortDirection;
//namespace F1Solutions.Naati.Common.Dal.Portal.Repositories.PractitionersDirectory
//{
//    public class LinqPractitionerRepository : IPractitionersRepository
//    {
//        private PractitionerRepositoryDataContext mDataContext;
     

//        public LinqPractitionerRepository(PractitionerRepositoryDataContext mDataContext)
//        {
//            this.mDataContext = mDataContext;
         
//        }

//        public IEnumerable<tluAccreditationLevel> GetAccreditationLevels()
//        {
//            var query = from al in mDataContext.tluAccreditationLevels
//                        select al;
//            return query.ToList();
//        }

//        public IEnumerable<tluAccreditationDescription> GetAccreditationDescriptions()
//        {
//            var query = from ad in mDataContext.tluAccreditationDescriptions
//                        select ad;
//            return query.ToList();
//        }

//        public IEnumerable <vwPractitioner> SearchPractitioners(PractitionerSearchCriteria criteria)
//        {
//            var query = from practitioner in mDataContext.vwPractitioners
//                        select practitioner;

//            //if (criteria.AccreditationCategoryId.HasValue)
//            //{
//            //    query = query.Where(x => x.AccreditationCategoryId == criteria.AccreditationCategoryId);
//            //}
//            //else
//            //{
//            //    // Remove Language Aides from the query, as we currently don't want to display them.
//            //    query = query.Where(x => x.AccreditationCategoryId != mSystemValues.LanguageAideCategoryId);
//            //}

//            if (criteria.AccreditationLevelId.HasValue)
//            {
//                query = query.Where(x => x.AccreditationLevelId == criteria.AccreditationLevelId);
//            }

//            if (criteria.FirstLanguageId.HasValue)
//            {
//                query = query.Where(x => x.LanguageId == criteria.FirstLanguageId);
//            }

//            if (criteria.SecondLanguageId.HasValue)
//            {
//                query = query.Where(x => x.ToLanguageId == criteria.SecondLanguageId);
//            }

//            if (criteria.StateId.HasValue)
//            {
//                query = query.Where(x => x.StateId == criteria.StateId);
//            }
//            if (criteria.CountryId.HasValue)
//            {
//                query = query.Where(x => x.CountryId == criteria.CountryId);
//            }

//            if (!string.IsNullOrEmpty(criteria.Postcode))
//            {
//                query = query.Where(x => x.Postcode == criteria.Postcode);
//            }

//            //if (!string.IsNullOrEmpty(criteria.Suburb))
//            //{
//            //    query = query.Where(x => x.Suburb == criteria.Suburb);
//            //}

//            if (!string.IsNullOrEmpty(criteria.Surname))
//            {
//                query = query.Where(x => x.Surname == criteria.Surname);
//            }

//            if(criteria.SortOrder != SortDirection.None)
//            {
//                if(criteria.SortOrder == SortDirection.Ascending)
//                {
//                    query = query.OrderBy(x => x.Surname).ThenBy(x => x.OtherNames).ThenBy(x => x.GivenName);
//                }
//                else
//                {
//                    query = query.OrderByDescending(x => x.Surname).ThenBy(x => x.OtherNames).ThenBy(x => x.GivenName);
//                }
//            }

//            return query;
//        }

//        public vwPractitionerAddressDetail GetPractitionerAddressDetails(int identifier)
//        {
//            var allAddressDetails = mDataContext.vwPractitionerAddressDetails.Where(a => a.NAATINumber == identifier).ToList();

//            //This should either contain the explicitly-included address, or whatever they had as their "primary/preferred" address
//            return allAddressDetails.FirstOrDefault(a => a.ODAddressVisibilityTypeId != (int)OdAddressVisibilityTypeName.DoNotShow);
//        }

//        public string GetPractitionerWorkAreas(int identifier)
//        {
//            var allWorkAres =
//                mDataContext.vwPractitioners.Where(b => b.NAATINumber == identifier).Select(a => a.Area).ToList();

//            allWorkAres = allWorkAres.Distinct().ToList();

//            if (allWorkAres.Count <= 0)
//            {
//                return null;
//            }

//            var ResultString = allWorkAres.FirstOrDefault();

//	        if (ResultString == null)
//	        {
//		        return "";
//	        }

//            allWorkAres.Remove(allWorkAres.FirstOrDefault());

//            allWorkAres.ForEach(s =>
//            {
//                ResultString += ", " + s;
//            });

//            //This should either contain the explicitly-included address, or whatever they had as their "primary/preferred" address
//            return ResultString;
//        }

//        public IEnumerable<vwPractitionerAddressDetail> GetAllPractitionerAddressDetails()
//        {
//            return mDataContext.vwPractitionerAddressDetails.ToList();
//        }

//        public IEnumerable<vwPractitionerContactDetail> GetPractitionerContactDetails(int identifier)
//        {
//            return mDataContext.vwPractitionerContactDetails.Where(x => x.NAATINumber == identifier);
//        }

//        public IEnumerable<vwPractitionerContactDetail> GetAllPractitionerContactDetails()
//        {
//            return mDataContext.vwPractitionerContactDetails.ToList();
//        }
//    }
//}
