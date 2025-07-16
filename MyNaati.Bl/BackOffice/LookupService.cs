using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Portal.Repositories;
using MyNaati.Contracts.BackOffice.Lookups;
using Country = F1Solutions.Naati.Common.Contracts.Dal.Portal.Country;
using Language = F1Solutions.Naati.Common.Contracts.Dal.Portal.Language;
using Postcode = F1Solutions.Naati.Common.Contracts.Dal.Portal.Postcode;
using TestLocation = F1Solutions.Naati.Common.Contracts.Dal.Portal.TestLocation;

namespace MyNaati.Bl.BackOffice
{
  //  public class LookupService : ILookupService
  //  {
  //      private readonly ICredentialLanguage1CacheQueryService _credenetialLanguage1Cache;
  //      private ILanguageRepository mLanguageRepository;
  //      private ICountryRepository mCountryRepository;
  //      private IStateRepository mStateRepository;
  //      private IPostcodeRepository mPostcodeRepository;
  //      private SystemValuesTranslator mSystemValuesTranslator;
  //      private ITestLocationRepository mTestLocationRepository;

  //      private IPractitionerQueryService mPractitionerQueryService;

  //      //To get away from really anemic repositories for lookups, this particular
  //      //service will access NHibernate (i.e. the DB) directly.
  //      private readonly ICustomSessionManager mSessionManager;

  //      public LookupService(ICredentialLanguage1CacheQueryService credenetialLanguage1Cache )
  //      {
  //          _credenetialLanguage1Cache = credenetialLanguage1Cache;
  //      }


  //      public GetAllLookupsResponse GetAllLookups(GetAllLookupsRequest request)
  //      {
  //          var session = mSessionManager.OpenSession();
  //          var response = new GetAllLookupsResponse();

  //          response.Languages = GetLanguages().ToArray();
  //          response.CertificatesLanguages1 = GetCertificatesLanguages1().ToArray();
  //          response.CertificatesLanguages2 = GetCertificatesLanguages2().ToArray();
  //          response.CertificatesCredentialTypes = GetCertificatesCredentialTypes().ToArray();

  //          var samCountries = mCountryRepository.GetAll().OrderBy(x=>x.Name);
  //          response.Countries = Translate(samCountries).ToArray();

  //          var samStates = mStateRepository.GetAll().OrderBy(x => x.DisplayText);
  //          response.States = samStates.ToArray();

		//	var samPostcodes = mPostcodeRepository.GetAll().OrderBy(x => x.Suburb.Name);
		//	response.Postcodes = Translate(samPostcodes).ToArray();

		//	var testLocations = mTestLocationRepository.GetAll();
		//	response.TestLocations = Translate(testLocations).ToArray();

		//	response.PersonTitles = session.Query<Title>().OrderBy(x=>x.TitleName)
  //              .Select(t => new PersonTitle
  //              {
  //                  SamId = t.Id,
  //                  DisplayText = t.TitleName,
  //                  IsStandardTitle = t.StandardTitle
  //              }).ToArray();

  //          response.OdAddressVisibilityTypes = session.Query<OdAddressVisibilityType>().OrderBy(x => x.Id)
  //              .Select(t => new OdAddressVisibilityTypeLookup
  //              {
  //                  SamId = t.Id,
  //                  DisplayText = t.DisplayName,
  //              }).ToArray();

  //          return response;
  //      }

  //      private IEnumerable<Language> GetLanguages()
  //      {
  //          IList<global::F1Solutions.Naati.Common.Dal.Domain.Language> allLanguages = mLanguageRepository.FindLanguagesForWebUsage();

  //          IEnumerable<Language> availableLanguages = allLanguages.Select(language => new Language
  //          {
  //              DisplayText = language.Name,
  //              SamId = language.Id
  //          }).OrderBy(x => x.DisplayText);

  //          return availableLanguages;
  //      }

  //      public IEnumerable<LanguageLookup> GetCertificatesLanguages1()
  //      {
  //         return mPractitionerQueryService.GetLookup(PractitionerLookupType.ActiveCredentialsLanguages1)
  //              .Results.OrderBy(x=>x.DisplayName).Select(x=>new LanguageLookup { SamId = x.Id, DisplayText = x.DisplayName}).ToList();
  //      }

  //      public IEnumerable<LanguageLookup> GetCertificatesLanguages2()
  //      {
  //          return mPractitionerQueryService.GetLookup(PractitionerLookupType.ActiveCredentialsLanguages2)
  //              .Results.OrderBy(x=>x.DisplayName).Select(x => new LanguageLookup { SamId = x.Id, DisplayText = x.DisplayName }).ToList();
  //      }

  //      public IEnumerable<CredentialTypeLookup> GetCertificatesCredentialTypes()
  //      {
  //          return mPractitionerQueryService.GetLookup(PractitionerLookupType.ActiveCredentialsTypesByRequest)
  //              .Results.OrderBy(x=>x.DisplayName).Select(x => new CredentialTypeLookup { SamId = x.Id, DisplayText = x.DisplayName, SkillName = x.ExtraData }).ToList();
  //      }

  //      private IEnumerable<Country> Translate(IEnumerable<F1Solutions.Naati.Common.Dal.Domain.Country> countries)
  //      {
  //          foreach (var country in countries)
  //              yield return new Country
  //              {
  //                  DisplayText = country.Name,
  //                  //TODO: This is potentially lame
  //                  IsHomeCountry = country.Id == mSystemValuesTranslator.AustraliaCountryId,
  //                  SamId = country.Id
  //              };
		//}

		//private IEnumerable<Postcode> Translate(IEnumerable<F1Solutions.Naati.Common.Dal.Domain.Postcode> postcodes)
		//{
		//	foreach (var postcode in postcodes)
		//	{
		//		yield return new Postcode
		//		{
		//			DisplayText = string.Format("{0} {1} {2}", postcode.Suburb.Name, postcode.Suburb.State.Abbreviation, postcode.PostCode),
		//			SamId = postcode.Id,
		//			SuburbId = postcode.Suburb.Id,
		//			Suburb = postcode.Suburb.Name,
		//			State = postcode.Suburb.State.Abbreviation,
		//			Code = postcode.PostCode
		//		};
		//	}
		//}

		//private IEnumerable<TestLocation> Translate(IEnumerable<F1Solutions.Naati.Common.Dal.Domain.TestLocation> testLocations)
		//{
		//	foreach (var testLocation in testLocations)
		//	{
		//		yield return new TestLocation
		//		{
		//			DisplayText = testLocation.Name,
		//			SamId = testLocation.Id
		//		};
		//	}
		//}

		//public CertificatesCredentialTypesResponse GetActiveCredentialTypes()
  //      {
  //          return new CertificatesCredentialTypesResponse { CertificatesCredentialTypes = GetCertificatesCredentialTypes().ToArray()};
  //      }
  //  }
}
