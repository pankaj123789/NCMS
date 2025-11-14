using System;
using System.Collections.Generic;
using System.Web;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.CacheQuery;
using F1Solutions.Naati.Common.Dal.Portal;
using MyNaati.Contracts.BackOffice.Language;
using MyNaati.Contracts.BackOffice.Lookups;
using MyNaati.Contracts.Portal;
using ISystemValueService = F1Solutions.Naati.Common.Contracts.Dal.Portal.SystemValues.ISystemValueService;

namespace MyNaati.Ui.Common
{
    public class CachedLookupProvider : ILookupProvider
    {
        private readonly ICredentialLanguage1CacheQueryService _credentialLanguage1CacheQueryService;
        private readonly ICredentialLanguage2CacheQueryService _credentialLanguage2CacheQueryService;
        private readonly ILanguagesCacheQueryService _languagesCacheQueryService;
        private readonly ICountriesCacheQueryService _countriesCacheQueryService;
        private readonly ITestLocationsCacheQueryService _testLocationsCacheQueryService;
        private readonly IPostcodesCacheQueryService _postcodesCacheQueryService;
        private readonly IPersonTitlesCacheQueryService _personTitlesCacheQueryService;
        private readonly IOdAddressVisibilityTypesCacheQueryService _odAddressVisibilityTypesCacheQueryService;

        public CachedLookupProvider(ICredentialLanguage1CacheQueryService credentialLanguage1CacheQueryService,
            ILanguagesCacheQueryService languagesCacheQueryService,
            ICredentialLanguage2CacheQueryService credentialLanguage2CacheQueryService,
            ICountriesCacheQueryService countriesCacheQueryService,
            ITestLocationsCacheQueryService testLocationsCacheQueryService,
            IPostcodesCacheQueryService postcodesCacheQueryService,
            IPersonTitlesCacheQueryService personTitlesCacheQueryService,
            IOdAddressVisibilityTypesCacheQueryService odAddressVisibilityTypesCacheQueryService,
            ISystemValuesTranslator systemValuesTranslator)
        {
            _credentialLanguage1CacheQueryService = credentialLanguage1CacheQueryService;
            _languagesCacheQueryService = languagesCacheQueryService;
            _credentialLanguage2CacheQueryService = credentialLanguage2CacheQueryService;
            _countriesCacheQueryService = countriesCacheQueryService;
            _testLocationsCacheQueryService = testLocationsCacheQueryService;
            _postcodesCacheQueryService = postcodesCacheQueryService;
            _personTitlesCacheQueryService = personTitlesCacheQueryService;
            _odAddressVisibilityTypesCacheQueryService = odAddressVisibilityTypesCacheQueryService;
            SystemValues = systemValuesTranslator;
        }

        public IEnumerable<AccreditationCategory> AccreditationCategories => new List<AccreditationCategory>();
        public IEnumerable<Language> Languages => _languagesCacheQueryService.GetAllLanguages();
        public IEnumerable<LanguageLookup> CertificationCredentialLanguages1 => _credentialLanguage1CacheQueryService.GetAllLanguages();
        public IEnumerable<LanguageLookup> CertificationCredentialLanguages2 => _credentialLanguage2CacheQueryService.GetAllLanguages();
        public IEnumerable<EoiLanguageTransferObject> EoiLanguages => new List<EoiLanguageTransferObject>();
        public IEnumerable<CredentialTypeLookup> CertificatesCredentialTypes => new List<CredentialTypeLookup>();
        public IEnumerable<State> States => new List<State>();
        public IEnumerable<Country> Countries => _countriesCacheQueryService.GetAllCountries();
        public IEnumerable<TestLocation> TestLocations => _testLocationsCacheQueryService.GetAllTestLocations();
        public IEnumerable<Postcode> Postcodes => _postcodesCacheQueryService.GetAllPostcodes();
        public IEnumerable<PersonTitle> PersonTitles => _personTitlesCacheQueryService.GetAllPersonTitles();
        public IEnumerable<OdAddressVisibilityTypeLookup> OdAddressVisibilityTypes => _odAddressVisibilityTypesCacheQueryService.GetAllOdAddressVisibilityTypes();
        public IEnumerable<Institution> Institutions => new List<Institution>();
        public IEnumerable<Course> Courses => new List<Course>();
        public ISystemValuesTranslator SystemValues { get; }

        public void RefreshAllCache()
        {
            _credentialLanguage1CacheQueryService.RefreshAllCache();
            _credentialLanguage2CacheQueryService.RefreshAllCache();
            _languagesCacheQueryService.RefreshAllCache();
            _countriesCacheQueryService.RefreshAllCache();
            _testLocationsCacheQueryService.RefreshAllCache();
            //_postcodesCacheQueryService.RefreshAllCache();
            _personTitlesCacheQueryService.RefreshAllCache();
            _odAddressVisibilityTypesCacheQueryService.RefreshAllCache();
            SystemValues.RefreshAllCache();
        }
    }
}