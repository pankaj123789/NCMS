namespace F1Solutions.Naati.Common.Dal.Portal.Repositories.PractitionersDirectory
{
    //public interface IPractitionerSearchRepository
    //{
    //    IEnumerable<Practitioner> SearchPractitioners(PractitionerSearchCriteria criteria);
    //    Practitioner SearchPractitioner(int naatiNumber);
    //    IEnumerable<Practitioner> ExportPractitioners(PractitionerSearchCriteria criteria);
    //    IEnumerable<ValCount[]> CountPractitioners(PractitionerSearchCriteria criteria);
    //    DateTime LastRefresh { get; }
    //    DateTime LastCompletedRefresh { get; }
    //}

    //internal class PractitionerSearchItem
    //{
    //    public Practitioner TranslatedPractitioner { get; set; }
    //    public IEnumerable<vwPractitioner> SourceRows { get; set; }
    //    public IList<PractitionerLanguageSkill> LanguageSkills { get; set; }
    //}

    /// <summary>
    /// This is just to help searching. So it doesn't have names or anything in it.
    /// </summary>
    //internal class PractitionerLanguageSkill
    //{
    //    public int AccreditationLevelId { get; set; }
    //    public int AccreditationCategoryId { get; set; }
    //    public int AccreditationMethodId { get; set; }

    //    public List<int> MatchingLanguageIds { get; set; }

    //    //Usually English
    //    public List<int> MatchingToLanguageIds { get; set; }

    //    public bool? ToEnglish { get; set; }
    //}

    //public class PractitionerSearchRepository : IPractitionerSearchRepository
    //{
    //    private object mRepositoryLock = new object();
    //    private ISystemValuesTranslator mSystemValuesTranslator;
    //    private IPractitionersRepository mSourceRepository;
    //    private ILanguageRepository mLanguageRepository;        

    //    public PractitionerSearchRepository(ISystemValuesTranslator translator, ILanguageRepository languageRepository)
    //    {
    //        mSystemValuesTranslator = translator;
    //        mLanguageRepository = languageRepository;
    //    }

    //    private ISystemValuesTranslator SystemValuesTranslator
    //    {
    //        get
    //        {
    //            return mSystemValuesTranslator;
    //        }
    //    }

    //    private IPractitionersRepository SourceRepository
    //    {
    //        get
    //        {
    //            lock (mRepositoryLock)
    //            {
    //                if (mSourceRepository == null)
    //                    //Get this instance directly from container so that it doesn't get disposed too early
    //                    mSourceRepository = ServiceLocator.Resolve<IPractitionersRepository>();

    //                return mSourceRepository;
    //            }
    //        }
    //    }

    //    private ILanguageRepository LanguageRepository
    //    {
    //        get
    //        {
    //            return mLanguageRepository;
    //        }
    //    }

    //    #region Cached Results Stuff

    //    private static IList<PractitionerSearchItem> mCachedResults;
    //    private static DateTime mLastRefresh = DateTime.Now;
    //    private static DateTime mLastCompletedRefresh = DateTime.Now;
    //    private static object mOnlyOneRefreshAtATime = new object();        
    //    private static Thread mThread;
        
    //    public DateTime LastRefresh
    //    {
    //        get
    //        {
    //            return mLastRefresh;
    //        }
    //    }

    //    public DateTime LastCompletedRefresh
    //    {
    //        get
    //        {
    //            return mLastCompletedRefresh;
    //        }
    //    }

    //    private IList<PractitionerSearchItem> CachedResults
    //    {
    //        get
    //        {
    //            //The action of this lock and the mLastRefresh update below should prevent 
    //            //simultaneous refreshes... assuming the refresh interval isn't too short
    //            lock (mOnlyOneRefreshAtATime)
    //            {
    //                if (mCachedResults == null)
    //                {
    //                    mLastRefresh = DateTime.Now;
    //                    mCachedResults = GetFreshPractitionerResults();
    //                }
    //                else if (mLastRefresh.AddSeconds(MinimumCacheRefreshInterval) < DateTime.Now)
    //                {
    //                    mLastRefresh = DateTime.Now;

    //                    //This refresh is kicked off in response to searches so:
    //                    // * We don't bother refreshing while nobody is searching
    //                    // * We don't need to worry about starting/restarting some long-running thread that may encounter exceptions from time to time.                        
    //                    if (mThread == null || mThread.ThreadState == ThreadState.Stopped)
    //                    {
    //                        mThread = new Thread(RefreshPractitionerResultsInBackground);                            
    //                        mThread.Start();                            
    //                    }                                                                        
    //                }
    //            }

    //            return mCachedResults;
    //        }
    //    }

    //    //Cache initialisation takes 20sec+. Call this at startup so the search is always cracking fast.
    //    public static void PrimeCacheAsync()
    //    {
    //        var repository = new PractitionerSearchRepository(
    //            ServiceLocator.Resolve<ISystemValuesTranslator>(),
    //            ServiceLocator.Resolve<ILanguageRepository>());

    //        mLastRefresh = DateTime.Now;
    //        if (mThread == null || mThread.ThreadState == ThreadState.Stopped)
    //        {
    //            mThread = new Thread(repository.RefreshPractitionerResultsInBackgroundWithLocking);
    //            mThread.Start();
    //        }            
    //    }

    //    private int MinimumCacheRefreshInterval
    //    {
    //        get
    //        {
    //            var intervalString = ConfigurationManager.AppSettings["PDSearchResults_MinimumCacheRefreshInterval"];
    //            int testVal;
    //            if (int.TryParse(intervalString, out testVal))
    //                return testVal;

    //            return 300;
    //        }
    //    }

    //    private void RefreshPractitionerResultsInBackgroundWithLocking()
    //    {
    //        lock (mOnlyOneRefreshAtATime)
    //        {
    //            RefreshPractitionerResultsInBackground();
    //        }
    //    }

    //    private void RefreshPractitionerResultsInBackground()
    //    {
    //        try
    //        {
    //            var freshResults = GetFreshPractitionerResults();
    //            mCachedResults = freshResults;
    //            //last completed date time
    //            mLastCompletedRefresh = DateTime.Now;                
    //        }
    //        catch (Exception e)
    //        {
    //            LoggingHelper.LogException(e, "Could not refresh PD Search Results cache");
    //        }
    //    }

    //    private IList<PractitionerSearchItem> GetFreshPractitionerResults()
    //    {
    //        var openCriteria = new PractitionerSearchCriteria()
    //        {
    //            PageNumber = 0,
    //            PageSize = 1000000,
    //            SortOrder = SortDirection.None,
    //        };

    //        var rawResults = SourceRepository.SearchPractitioners(openCriteria)
    //            .Where(p => p.CountryId != null)
    //            // Remove Language Aides from the query, as we currently don't want to display them.
    //            .Where(p => p.AccreditationCategoryId != SystemValuesTranslator.LanguageAideCategoryId).ToList();
    //        var rawAddresses = SourceRepository.GetAllPractitionerAddressDetails().ToList();
    //        var rawContacts = SourceRepository.GetAllPractitionerContactDetails().ToList();

    //        return TransformResults(rawResults, rawAddresses, rawContacts).ToList();
    //    }

    //    #endregion Cached Results Stuff

    //    public IEnumerable<Practitioner> SearchPractitioners(PractitionerSearchCriteria criteria)
    //    {
    //        var query = CachedResults.AsEnumerable();
    //        query = query.Where(p => IsMatch(p, criteria));
    //        query = SortPractitioners(query, criteria.SortOrder, criteria.SortMember, criteria.RandomSearchSeed);
    //        return query.Select(p => p.TranslatedPractitioner);
    //    }

    //    public Practitioner SearchPractitioner(int naatiNumber)
    //    {
    //        var query = CachedResults.AsEnumerable();
    //        query = query.Where(p => p.TranslatedPractitioner.Id == naatiNumber);
    //        return query.Select(p => p.TranslatedPractitioner).FirstOrDefault();
    //    }

    //    public IEnumerable<Practitioner> ExportPractitioners(PractitionerSearchCriteria criteria)
    //    {
    //        var query = CachedResults.AsEnumerable();
    //        query = query.Where(p => IsMatch(p, criteria));
    //        query = SortPractitioners(query, criteria.SortOrder, criteria.SortMember, criteria.RandomSearchSeed);
    //        return query.Select(p => p.TranslatedPractitioner);
    //    }

    //    public IEnumerable<ValCount[]> CountPractitioners(PractitionerSearchCriteria criteria)
    //    {
    //        var data = CachedResults.AsEnumerable();
	   //     return CriteriaCounts(data, criteria);
    //    }

    //    private bool IsMatch(PractitionerSearchItem input, PractitionerSearchCriteria criteria)
    //    {
    //        const int LANGUAGE_ID_CHINESE = 24;
    //        const int LANGUAGE_ID_MANDARIN = 116;
    //        var languageQuery = input.LanguageSkills.AsEnumerable();

    //        //if (criteria.AccreditationCategoryId.HasValue)
    //        //    languageQuery = languageQuery.Where(x => x.AccreditationCategoryId == criteria.AccreditationCategoryId);

    //        if (criteria.ToEnglish.HasValue)
    //            languageQuery = languageQuery.Where(x => x.ToEnglish == criteria.ToEnglish || x.ToEnglish == null);

    //        if (criteria.AccreditationLevelId.HasValue)
    //            languageQuery = languageQuery.Where(x => x.AccreditationLevelId == criteria.AccreditationLevelId);

    //        if (criteria.FirstLanguageId.HasValue)
    //        {
    //            if (criteria.FirstLanguageId == LANGUAGE_ID_CHINESE)
    //            {
    //                languageQuery = languageQuery.Where(x => x.MatchingLanguageIds.Any(l => (l == criteria.FirstLanguageId) || (l == LANGUAGE_ID_MANDARIN)));
    //            }
    //            else
    //            {
    //                languageQuery = languageQuery.Where(x => x.MatchingLanguageIds.Any(l => l == criteria.FirstLanguageId));
    //            }
    //        }

    //        if (criteria.SecondLanguageId.HasValue)
    //            languageQuery = languageQuery.Where(x => x.MatchingToLanguageIds.Any(l => l == criteria.SecondLanguageId));

    //        var practitionerSourceRecord = input.SourceRows.First();

    //        return languageQuery.Any() &&
    //            (!criteria.StateId.HasValue || practitionerSourceRecord.StateId == criteria.StateId) &&
    //            (!criteria.CountryId.HasValue || practitionerSourceRecord.CountryId == criteria.CountryId) &&
    //            (string.IsNullOrEmpty(criteria.Postcode) || practitionerSourceRecord.Postcode == criteria.Postcode) &&
    //            //(string.IsNullOrEmpty(criteria.Suburb) || practitionerSourceRecord.Suburb.ToLower().Contains(criteria.Suburb.ToLower())) &&
    //            (string.IsNullOrEmpty(criteria.Surname) || practitionerSourceRecord.Surname.ToLower().Contains(criteria.Surname.ToLower()));
    //    }

    //    private IEnumerable<ValCount[]> CriteriaCounts(IEnumerable<PractitionerSearchItem> data, PractitionerSearchCriteria criteria)
    //    {
    //        const int LANGUAGE_ID_CHINESE = 24;
    //        const int LANGUAGE_ID_MANDARIN = 116;

    //        var list = new List<ValCount[]>();
	   //     int listCount = 0;
	   //     int i = 0;
	   //     List<PractitionerSearchItem> refinedItems = new List<PractitionerSearchItem>();
	   //     foreach (var row in data)
	   //     {
		  //      refinedItems.Add(row);
	   //     }

    //        // first add the total count
    //        list.Add(new ValCount[] { new ValCount(){ Count = data.Count(), Val = 0 }});
	   //     var uniqueValues = new List<int>();
    //        // then add the counts for the from language option
    //        refinedItems.ForEach(rf =>
    //        {
    //            var vals = rf.LanguageSkills.Select(ls => ls.MatchingLanguageIds.Select(mI => mI));
    //            foreach (var val in vals)
    //            {
    //                foreach (var current in val)
    //                {
    //                    if (!uniqueValues.Contains(current))
    //                    {
    //                        uniqueValues.Add(current);
    //                    }
    //                }
    //            }
    //        });
    //        list.Add(new ValCount[uniqueValues.Count + 1]);
    //        listCount = list.Count - 1;
    //        for (int j = 0; j < uniqueValues.Count + 1; j++)
    //        {
    //            list[listCount][j] = new ValCount();
    //        }
    //        list[listCount][0].Val = 0;
    //        list[listCount][0].Count = refinedItems.Count;
    //        i = 1;
    //        foreach (var fromLanguage in uniqueValues.OrderBy(v => v))
    //        {
    //            list[listCount][i].Val = fromLanguage;
    //            if (fromLanguage == LANGUAGE_ID_CHINESE)
    //            {
    //                list[listCount][i].Count = refinedItems.Count(rf => rf.LanguageSkills.Any(ls => ls.MatchingLanguageIds.Any(mI => (mI == fromLanguage || mI == LANGUAGE_ID_MANDARIN))));
    //            }
    //            else
    //            {
    //                list[listCount][i].Count = refinedItems.Count(rf => rf.LanguageSkills.Any(ls => ls.MatchingLanguageIds.Any(mI => mI == fromLanguage)));
    //            }
    //            i++;
    //        }
    //        if (criteria.FirstLanguageId != 0 && criteria.FirstLanguageId != null)
    //        {
    //            if (criteria.FirstLanguageId == LANGUAGE_ID_CHINESE)
    //            {
    //                refinedItems =
    //                refinedItems.Where(
    //                    p => p.LanguageSkills.Any(ls => ls.MatchingLanguageIds.Any(mI => mI == criteria.FirstLanguageId || mI == LANGUAGE_ID_MANDARIN))).ToList();
    //            }
    //            else
    //            {
    //                refinedItems =
    //                refinedItems.Where(
    //                    p => p.LanguageSkills.Any(ls => ls.MatchingLanguageIds.Any(mI => mI == criteria.FirstLanguageId))).ToList();
    //            }
    //        }
    //        uniqueValues.Clear();
    //        // then add the counts for the toLanguage
    //        refinedItems.ForEach(rf =>
    //        {
    //            var vals = rf.LanguageSkills.Select(ls => ls.MatchingToLanguageIds.Select(mI => mI));
    //            foreach (var val in vals)
    //            {
    //                foreach (var current in val)
    //                {
    //                    if (!uniqueValues.Contains(current))
    //                    {
    //                        uniqueValues.Add(current);
    //                    }
    //                }
    //            }
    //        });
    //        list.Add(new ValCount[uniqueValues.Count + 1]);
    //        listCount = list.Count - 1;
    //        for (int j = 0; j < uniqueValues.Count + 1; j++)
    //        {
    //            list[listCount][j] = new ValCount();
    //        }
    //        list[listCount][0].Val = 0;
    //        list[listCount][0].Count = refinedItems.Count;
    //        i = 1;
    //        foreach (var fromLanguage in uniqueValues.OrderBy(v => v))
    //        {
    //            list[listCount][i].Val = fromLanguage;
    //            list[listCount][i].Count = refinedItems.Count(rf => rf.LanguageSkills.Any(ls => ls.MatchingToLanguageIds.Any(mI => mI == fromLanguage)));
    //            i++;
    //        }
    //        if (criteria.SecondLanguageId != 0 && criteria.SecondLanguageId != null)
    //        {
    //            refinedItems =
    //                refinedItems.Where(
    //                    p => p.LanguageSkills.Any(ls => ls.MatchingToLanguageIds.Any(mI => mI == criteria.SecondLanguageId))).ToList();
    //        }
    //        uniqueValues.Clear();
    //        // then add the counts for the Accreditation Category option
    //        refinedItems.ForEach(rf =>
    //        {
	   //         var vals = rf.LanguageSkills.Select(ls => ls.AccreditationCategoryId);
	   //         foreach (var val in vals)
	   //         {
		  //          if (!uniqueValues.Contains(val))
		  //          {
			 //           uniqueValues.Add(val);
		  //          }
	   //         }
    //        });
    //        list.Add(new ValCount[uniqueValues.Count + 1]);
    //        listCount = list.Count - 1;
    //        for (int j = 0; j < uniqueValues.Count + 1; j++)
	   //     {
		  //      list[listCount][j] = new ValCount();
	   //     }
    //        list[listCount][0].Val = 0;
    //        list[listCount][0].Count = refinedItems.Count;
	   //     i = 1;
    //        foreach (var AccreditationCategory in uniqueValues.OrderBy(v => v))
    //        {
    //            list[listCount][i].Val = AccreditationCategory;
    //            list[listCount][i].Count = refinedItems.Count(rf => rf.LanguageSkills.Any(ls => ls.AccreditationCategoryId == AccreditationCategory &&
    //                ls.MatchingLanguageIds.Any(mLi => mLi == criteria.FirstLanguageId)));
	   //         i++;
    //        }
    //     //   if (criteria.AccreditationCategoryId != 0 && criteria.AccreditationCategoryId != null)
	   //     //{
		  //     // refinedItems =
			 //     //  refinedItems.Where(
    //     //               p => p.LanguageSkills.Any(ls => ls.AccreditationCategoryId == criteria.AccreditationCategoryId &&
    //     //           ls.MatchingLanguageIds.Any(mLi => mLi == criteria.FirstLanguageId))).ToList();
	   //     //}
	   //     uniqueValues.Clear();
    //        // then add the counts for the AccreditationLevelId option
    //        refinedItems.ForEach(rf =>
    //        {
    //            var vals = rf.LanguageSkills.Select(ls => ls.AccreditationLevelId);
    //            foreach (var val in vals)
    //            {
    //                if (!uniqueValues.Contains(val))
    //                {
    //                    uniqueValues.Add(val);
    //                }
    //            }
    //        });
    //        list.Add(new ValCount[uniqueValues.Count + 1]);
    //        listCount = list.Count - 1;
    //        for (int j = 0; j < uniqueValues.Count + 1; j++)
    //        {
    //            list[listCount][j] = new ValCount();
    //        }
    //        list[listCount][0].Val = 0;
    //        list[listCount][0].Count = refinedItems.Count;
	   //     i = 1;
    //        foreach (var AccreditationLevel in uniqueValues.OrderBy(v => v))
    //        {
    //            list[listCount][i].Val = AccreditationLevel;
    //            list[listCount][i].Count = refinedItems.Count(rf => rf.LanguageSkills.Any(ls => ls.AccreditationLevelId == AccreditationLevel &&
    //                ls.MatchingLanguageIds.Any(mLi => mLi == criteria.FirstLanguageId)));
	   //         i++;
    //        }
    //        if (criteria.AccreditationLevelId != 0 && criteria.AccreditationLevelId != null)
	   //     {
		  //      refinedItems =
			 //       refinedItems.Where(
    //                    p => p.LanguageSkills.Any(ls => ls.AccreditationLevelId == criteria.AccreditationLevelId &&
    //                ls.MatchingLanguageIds.Any(mLi => mLi == criteria.FirstLanguageId))).ToList();
	   //     }
	   //     uniqueValues.Clear();
    //        // then add the counts for the CountryId option
    //        refinedItems.ForEach(rf =>
    //        {
    //            if (rf.SourceRows.First() != null && rf.SourceRows.First().CountryId != null)
    //            {
    //                var val = (int)rf.SourceRows.First().CountryId;
    //                if (!uniqueValues.Contains(val))
    //                {
    //                    uniqueValues.Add(val);
    //                }
    //            }
    //        });
    //        list.Add(new ValCount[uniqueValues.Count + 1]);
    //        listCount = list.Count - 1;
    //        for (int j = 0; j < uniqueValues.Count + 1; j++)
    //        {
    //            list[listCount][j] = new ValCount();
    //        }
    //        list[listCount][0].Val = 0;
    //        list[listCount][0].Count = refinedItems.Count;
    //        i = 1;
    //        foreach (var CountryId in uniqueValues.OrderBy(v => v))
    //        {
    //            list[listCount][i].Val = CountryId;
    //            var temp = refinedItems.Where(rf => rf.SourceRows.First().CountryId == CountryId).ToList();
    //            var nonMatch = temp.Where(t => !IsMatch(t, criteria)).ToList();
    //            nonMatch.ForEach(t =>
    //            {
    //                var temp1 = !IsMatch(t, criteria);
    //            });
    //            list[listCount][i].Count = refinedItems.Count(rf => rf.SourceRows.First().CountryId == CountryId);
    //            i++;
    //        }
    //        if (criteria.CountryId != 0 && criteria.CountryId != null)
    //        {
    //            refinedItems =
    //                refinedItems.Where(
    //                    p => p.SourceRows.First().CountryId == criteria.CountryId).ToList();
    //        }
    //        uniqueValues.Clear();
    //        // then if necessary add state counts
    //        refinedItems.ForEach(rf =>
    //        {
    //            if (rf.SourceRows.First() != null && rf.SourceRows.First().StateId != null)
    //            {
    //                var val = (int)rf.SourceRows.First().StateId;
    //                if (!uniqueValues.Contains(val))
    //                {
    //                    uniqueValues.Add(val);
    //                }
    //            }
    //        });
    //        if ((criteria.CountryId == 0 || criteria.CountryId == 13))
    //        {
    //            list.Add(new ValCount[uniqueValues.Count + 1]);
    //            listCount = list.Count - 1;
    //            for (int j = 0; j < uniqueValues.Count + 1; j++)
    //            {
    //                list[listCount][j] = new ValCount();
    //            }
    //        }
    //        else
    //        {
    //            list.Add(new ValCount[1]);
    //            listCount = list.Count - 1;
    //            list[listCount][0] = new ValCount();
    //        }
            
    //        list[listCount][0].Val = 0;
    //        list[listCount][0].Count = refinedItems.Count;
    //        i = 1;
    //        if ((criteria.CountryId == 0 || criteria.CountryId == 13))
    //        {
    //            foreach (var StateId in uniqueValues.OrderBy(v => v))
    //            {
    //                list[listCount][i].Val = StateId;
    //                list[listCount][i].Count = refinedItems.Count(rf => rf.SourceRows.First().StateId == StateId);
    //                i++;
    //            }
    //            if (criteria.StateId != 0 && criteria.StateId != null)
    //            {
    //                refinedItems =
    //                    refinedItems.Where(
    //                        p => p.SourceRows.First().StateId == criteria.StateId).ToList();
    //            }
    //        }
    //        uniqueValues.Clear();

	   //     return list.AsEnumerable();
    //    }

    //    private int getStateId(string name)
    //    {
    //        if (name.Contains("ACT"))
    //        {
    //            return 1;
    //        }
    //        else if (name.Contains("NSW"))
    //        {
    //            return 2;
    //        }
    //        else if (name.Contains("NT"))
    //        {
    //            return 3;
    //        }
    //        else if (name.Contains("QLD"))
    //        {
    //            return 4;
    //        }
    //        else if (name.Contains("SA"))
    //        {
    //            return 5;
    //        }
    //        else if (name.Contains("TAS"))
    //        {
    //            return 6;
    //        }
    //        else if (name.Contains("VIC"))
    //        {
    //            return 7;
    //        }
    //        else if (name.Contains("WA"))
    //        {
    //            return 8;
    //        }
    //        else
    //        {
    //            return 0;
    //        }
    //    }

    //    private IEnumerable<PractitionerSearchItem> TransformResults(IEnumerable<vwPractitioner> input, 
    //        IEnumerable<vwPractitionerAddressDetail> addresses, IEnumerable<vwPractitionerContactDetail> contacts)
    //    {
    //        var allLanguages = LanguageRepository.FindLanguagesForWebUsage();

    //        var addressLookup = addresses.GroupBy(a => a.NAATINumber.Value).ToDictionary(g => g.Key);
    //        var contactLookup = contacts.GroupBy(c => c.NAATINumber.Value).ToDictionary(g => g.Key);

    //        var groupedQuery = input.GroupBy(
    //            x =>
    //            new
    //            {
    //                x.PersonId,
    //                x.NAATINumber,
    //                x.Title,
    //                x.GivenName,
    //                x.OtherNames,
    //                x.Surname,
    //                x.ExpertiseFreeText,
    //                x.Suburb,
    //                x.StateName,
    //                x.CountryName,
    //                CountryId = x.CountryId.Value,
    //                x.Postcode,
    //                x.StreetDetails
    //            });

    //        foreach (var practitionerGroup in groupedQuery)
    //        {
    //            var naatiNumber = practitionerGroup.Key.NAATINumber.Value;
    //            //Work areas
    //            var uniqueExpertise = practitionerGroup
    //                .Where(p => !string.IsNullOrEmpty(p.Area))
    //                .Select(p => p.Area)
    //                .OrderBy(p => p)
    //                .Distinct();

    //            if (uniqueExpertise.Count() == 0)
    //                uniqueExpertise = uniqueExpertise.Union(new[] { "Any Area" });

    //            var expertiseString = string.Join(", ", uniqueExpertise.ToArray());

    //            //Skills
    //            var skillGroup = practitionerGroup
    //                .Distinct(new PractitionerSkillDistinctComparer())
    //                .OrderBy(p => p, new PractitionerSkillComparer());
    //            var skillStrings = skillGroup
    //                .Select(s => string.Format("{0} ({1})",
    //                GetDescription(s),
    //                GetDirection(s)));
    //            var accreditationString = string.Join(", ", skillStrings.ToArray());

    //            AddressDetail address = null;
    //            if (addressLookup.ContainsKey(naatiNumber))
    //            {
    //                var practitionerAndAddressDetails = addressLookup[practitionerGroup.Key.NAATINumber.Value];
    //                var explicitlyIncludedAddress = practitionerAndAddressDetails.FirstOrDefault(a => a.ODAddressVisibilityTypeId != (int)OdAddressVisibilityTypeName.DoNotShow);
    //                //rely on the queried view to only give back primary/preferred address if no PdIncluded address
    //                var preferredAddress = practitionerAndAddressDetails.FirstOrDefault();
    //                var addressToDisplay = explicitlyIncludedAddress ?? preferredAddress;
    //                address = TransformAddress(addressToDisplay);
    //            }
                
    //            List<ContactDetail> contactDetails;
    //            if (contactLookup.ContainsKey(naatiNumber))
    //            {
    //                contactDetails = contactLookup[practitionerGroup.Key.NAATINumber.Value]
    //                    .Select(Mapper.Map<vwPractitionerContactDetail, ContactDetail>)
    //                    .ToList();
    //            }
    //            else
    //            {
    //                contactDetails = new List<ContactDetail>();
    //            }

    //            yield return new PractitionerSearchItem()
    //            {
    //                TranslatedPractitioner = new Practitioner
    //                {
    //                    Id = practitionerGroup.Key.NAATINumber.Value,
    //                    GivenName = practitionerGroup.Key.GivenName,
    //                    OtherNames = practitionerGroup.Key.OtherNames,
    //                    Surname = practitionerGroup.Key.Surname,
    //                    Title = practitionerGroup.Key.Title,
    //                    Location = BuildLocation(practitionerGroup.Key.StreetDetails, practitionerGroup.Key.Suburb, practitionerGroup.Key.StateName, practitionerGroup.Key.Postcode, practitionerGroup.Key.CountryName, practitionerGroup.Key.CountryId),
    //                    Skills = accreditationString,
    //                    Address = address,
    //                    ContactDetails = contactDetails
    //                },
    //                SourceRows = practitionerGroup,
    //                LanguageSkills = TransformSkills(practitionerGroup, allLanguages).ToList()
    //            };
    //        }
    //    }

    //    private AddressDetail TransformAddress(vwPractitionerAddressDetail a)
    //    {
    //        if (a == null)
    //            return null;

    //        AddressDetail result = new AddressDetail();
    //        result.OdAddressVisibilityTypeId = (int)a.ODAddressVisibilityTypeId;
    //        result.Country = a.CountryName;
    //        result.CountryId = (int)a.CountryId;
    //        result.DefaultContryId = (int)a.CountryId;
    //        result.GivenName = a.GivenName;
    //        result.OtherNames = a.OtherNames;
    //        result.Surname = a.Surname;
    //        result.Title = a.Title;

    //        result.Postcode = a.Postcode;
    //        result.State = a.State;
    //        result.StreetDetails = a.StreetDetails;
    //        result.Suburb = a.Suburb;

    //        return result;
    //    }

    //    private IEnumerable<PractitionerLanguageSkill> TransformSkills(IEnumerable<vwPractitioner> input, IEnumerable<Language> allLanguages)
    //    {
    //        return input.Select(l => new PractitionerLanguageSkill()
    //        {
    //            AccreditationCategoryId = l.AccreditationCategoryId.Value,
    //            AccreditationLevelId = l.AccreditationLevelId.Value,
    //            AccreditationMethodId = l.AccreditationMethodId,
    //            MatchingLanguageIds = GetMatchingLanguageIds(l.LanguageId, allLanguages).ToList(),
    //            MatchingToLanguageIds = GetMatchingLanguageIds(l.ToLanguageId ?? SystemValuesTranslator.EnglishLanguageId, allLanguages).ToList(),
    //            ToEnglish = l.ToEnglish
    //        });
    //    }

    //    private IEnumerable<int> GetMatchingLanguageIds(int languageId, IEnumerable<Language> allLanguages)
    //    {
    //        yield return languageId;

    //        var language = allLanguages.Single(l => l.Id == languageId);
            
    //        // this might need to be updated to support the new kind of grouping structure
    //        //if (language.GroupLanguage != null)
    //        //    yield return language.GroupLanguage.Id;

    //        foreach (var matchedByCode in allLanguages.Where(l => l.Code == language.Code && l.Id != languageId))
    //            yield return matchedByCode.Id;
    //    }

    //    private IEnumerable<PractitionerSearchItem> SortPractitioners(IEnumerable<PractitionerSearchItem> practitioners, SortDirection direction, PDSortMember sortMember, int randomSeed)
    //    {
    //        var query = practitioners.Select(kv => kv);
    //        IOrderedEnumerable<PractitionerSearchItem> orderedQuery;
    //        if (direction == SortDirection.None)
    //        {
    //            query = query.OrderBy(p => GetRandomOrderForPractitioner(randomSeed, p.TranslatedPractitioner.Id));
    //        }
    //        else if (direction == SortDirection.Ascending)
    //        {
    //            // sort by the "sort member" first
    //            switch (sortMember)
    //            {
    //                case PDSortMember.State:
    //                    orderedQuery = query.OrderBy(x => x.TranslatedPractitioner.Address.State);
    //                    break;
    //                case PDSortMember.Suburb:
    //                    orderedQuery = query.OrderBy(x => x.TranslatedPractitioner.Address.Suburb);
    //                    break;
    //                case PDSortMember.Level:
    //                default:
    //                    orderedQuery = query.OrderBy(x => GetSenioritySortForAccreditationLevels(x.SourceRows.Select(vp => vp.AccreditationLevelId)));
    //                    break;
    //            }

    //            query = orderedQuery.ThenBy(p => GetRandomOrderForPractitioner(randomSeed, p.TranslatedPractitioner.Id));
    //        }
    //        else if (direction == SortDirection.Descending)
    //        {
    //            switch (sortMember)
    //            {
    //                case PDSortMember.State:
    //                    orderedQuery = query.OrderByDescending(x => x.TranslatedPractitioner.Address.State);
    //                    break;
    //                case PDSortMember.Suburb:
    //                    orderedQuery = query.OrderByDescending(x => x.TranslatedPractitioner.Address.Suburb);
    //                    break;
    //                case PDSortMember.Level:
    //                default:
    //                    orderedQuery = query.OrderByDescending(x => GetSenioritySortForAccreditationLevels(x.SourceRows.Select(vp => vp.AccreditationLevelId)));
    //                    break;
    //            }

    //            query = orderedQuery.ThenByDescending(p => GetSenioritySortForAccreditationLevels(p.SourceRows.Select(vp => vp.AccreditationLevelId)));
    //        }

    //        return query;
    //    }

    //    private class PractitionerSkillDistinctComparer : EqualityComparer<vwPractitioner>
    //    {
    //        public override bool Equals(vwPractitioner x, vwPractitioner y)
    //        {
    //            if (x.AccreditationCategoryId != y.AccreditationCategoryId)
    //                return false;

    //            if (x.AccreditationLevelId != y.AccreditationLevelId)
    //                return false;

    //            if (x.LanguageId != y.LanguageId)
    //                return false;

    //            if (x.ToLanguageId != y.ToLanguageId)
    //                return false;

    //            if (x.ToEnglish != y.ToEnglish)
    //                return false;

    //            if (x.NAATINumber != y.NAATINumber)
    //                return false;

    //            if (x.SubstituteLanguage != y.SubstituteLanguage)
    //                return false;

    //            //They're equal enough for our purposes
    //            return true;
    //        }

    //        public override int GetHashCode(vwPractitioner obj)
    //        {
    //            //Very dodgy implementation, but it's OK because this thing is only used for determining 
    //            //Distinct(), not for putting these in a dictionary.
    //            return obj.NAATINumber.Value;
    //        }
    //    }

    //    private class PractitionerSkillComparer : Comparer<vwPractitioner>
    //    {
    //        public override int Compare(vwPractitioner x, vwPractitioner y)
    //        {
    //            if (x.AccreditationLevelId != y.AccreditationLevelId)
    //                return GetSenioritySortForAccreditationLevel(x.AccreditationLevelId).CompareTo(GetSenioritySortForAccreditationLevel(y.AccreditationLevelId));

    //            if (x.AccreditationCategoryId != y.AccreditationCategoryId)
    //                //assume not null
    //                return x.AccreditationCategoryId.Value.CompareTo(y.AccreditationCategoryId.Value);

    //            return x.Name.CompareTo(y.Name);
    //        }
    //    }

    //    private int GetRandomOrderForPractitioner(int seed, int practitionerId)
    //    {
    //        var random = new Random(seed + practitionerId);
    //        return random.Next();
    //    }

    //    /// <returns>A ranking for the given accreditation level IDs. Lower == more senior.</returns>
    //    private int GetSenioritySortForAccreditationLevels(IEnumerable<int?> levelIds)
    //    {
    //        var maxId = levelIds.Max();
    //        return GetSenioritySortForAccreditationLevel(maxId);
    //    }

    //    internal static int GetSenioritySortForAccreditationLevel(int? levelId)
    //    {
    //        if (levelId == null)
    //            return int.MaxValue;

    //        //TODO: This is pretty horrible. It assumes that seniority of accreditation level increases with ID.
    //        //SAM system values don't currently contain the advanced levels. Timeframes too cramped to add them now.
    //        return -(levelId.Value);
    //    }

    //    private string GetDirection(vwPractitioner s)
    //    {
    //        bool? toLanguageB = s.ToEnglish.HasValue ? s.ToEnglish.Value || !string.IsNullOrEmpty(s.SubstituteLanguage) : (bool?)null;
    //        string languageA = s.Name;
    //        string languageB = s.SubstituteLanguage ?? "English";

    //        if (!toLanguageB.HasValue)
    //        {
    //            return string.Format("{0} to/from {1}", languageA, languageB);
    //        }

    //        if (toLanguageB.Value)
    //        {
    //            return string.Format("{0} to {1}", languageA, languageB);
    //        }

    //        return string.Format("{0} to {1}", languageB, languageA);
    //    }

    //    private string GetDescription(vwPractitioner practitioner)
    //    {
    //        var description = string.Empty;

    //        description = GetCustomAccreditationString(practitioner);

    //        if (string.IsNullOrEmpty(description))
    //        {
    //            description = GetDefaultAccreditationString(practitioner);
    //        }

    //        return description;
    //    }

    //    private string GetDefaultAccreditationString(vwPractitioner practitioner)
    //    {
    //        var accreditationLevelForPractitioner =
    //            SourceRepository.GetAccreditationLevels().SingleOrDefault(x => x.AccreditationLevelId == practitioner.AccreditationLevelId);

    //        if (accreditationLevelForPractitioner != null)
    //        {
    //            if (!string.IsNullOrEmpty(accreditationLevelForPractitioner.StandardLetterDisplay))
    //            {
    //                return string.Format("{0} {1}", accreditationLevelForPractitioner.StandardLetterDisplay,
    //                                     practitioner.Description);
    //            }
    //        }

    //        return string.Empty;
    //    }

    //    private string GetCustomAccreditationString(vwPractitioner practitioner)
    //    {
    //        var accreditationDescriptionForPractitioner = from ad in SourceRepository.GetAccreditationDescriptions()
    //                                                      where
    //                                                          (((!ad.AccreditationLevelId.HasValue ||
    //                                                             ad.AccreditationLevelId ==
    //                                                             practitioner.AccreditationLevelId) &&
    //                                                            (!ad.AccreditationMethodId.HasValue ||
    //                                                             ad.AccreditationMethodId ==
    //                                                             practitioner.AccreditationMethodId) &&
    //                                                            (!ad.AccreditationCategoryId.HasValue ||
    //                                                             ad.AccreditationCategoryId ==
    //                                                             practitioner.AccreditationCategoryId)) &&
    //                                                           (ad.AccreditationLevelId.HasValue ||
    //                                                            ad.AccreditationMethodId.HasValue ||
    //                                                            ad.AccreditationCategoryId.HasValue))
    //                                                      select ad;

    //        if (!accreditationDescriptionForPractitioner.Any())
    //        {
    //            return string.Empty;
    //        }


    //        if (accreditationDescriptionForPractitioner.First().Description == "All" && accreditationDescriptionForPractitioner.Count() == 1)
    //        {
    //            return string.Empty;
    //        }

    //        if (accreditationDescriptionForPractitioner.Count() == 1)
    //        {
    //            return accreditationDescriptionForPractitioner.First().Description;
    //        }

    //        return GetBestMatch(accreditationDescriptionForPractitioner, practitioner);
    //    }

    //    private string GetBestMatch(IEnumerable<tluAccreditationDescription> accreditationDescriptionForPractitioner, vwPractitioner practitioner)
    //    {
    //        var bestMatches = 0;
    //        var description = string.Empty;

    //        foreach (var accreditationDescription in accreditationDescriptionForPractitioner)
    //        {
    //            var matches = GetMatchWeighting(accreditationDescription, practitioner);
    //            if (matches > bestMatches)
    //            {
    //                bestMatches = matches;
    //                description = accreditationDescription.Description;
    //            }
    //        }

    //        return description;
    //    }

    //    private static int GetMatchWeighting(tluAccreditationDescription accreditationDescription, vwPractitioner practitioner)
    //    {
    //        var weighting = 0;

    //        if (accreditationDescription.AccreditationLevelId == practitioner.AccreditationLevelId)
    //        {
    //            weighting += 100;
    //        }

    //        if (accreditationDescription.AccreditationMethodId == practitioner.AccreditationMethodId)
    //        {
    //            weighting += 10;
    //        }

    //        if (accreditationDescription.AccreditationCategoryId == practitioner.AccreditationCategoryId)
    //        {
    //            weighting += 1;
    //        }

    //        return weighting;
    //    }

    //    //NOTE: This should probably be an interface thing and we return all of these to the gui...
    //    public string BuildLocation(string streetDetails, string suburb, string state, string postCode, string country, int countryId)
    //    {
    //        if (countryId == SystemValuesTranslator.AustraliaCountryId)
    //        {
    //            return $"{suburb ?? string.Empty} {state ?? string.Empty} {postCode ?? string.Empty}";
    //        }

    //        return country;
    //    }

    //    public ValCount Val { get; set; }
    //}
}
