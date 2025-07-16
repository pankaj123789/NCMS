using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.ViewModels.PDSearch;

namespace MyNaati.Ui.Common
{
    public class OnlineDirectorySearch
    {
        private readonly ILookupProvider mLookupProvider;

        public OnlineDirectorySearch(ILookupProvider lookupService)
        {
            mLookupProvider = lookupService;
        }

        public PDSearchModel GetNewSearchModel(bool setDefaults, IEnumerable<int> skills = null)
        {
            var model = new PDSearchModel();
            var certificatesCredentialTypes = mLookupProvider.CertificatesCredentialTypes;

            model.CountryList = BuildCountryList();
            model.AustraliaCountryId = mLookupProvider.SystemValues.AustraliaCountryId;
            model.EnglishLanguageId = mLookupProvider.SystemValues.EnglishLanguageId;

            model.SkillList =
                certificatesCredentialTypes.Where(x => (skills ?? new[] {0}).Contains(ParseSkillId(x)))
                    .Select(x => x.SkillName)
                    .Distinct()
                    .ToList();

            model.AccreditationLevelList =
                certificatesCredentialTypes.Select(
                        x => new SelectListItem {Selected = false, Text = x.DisplayText, Value = x.SamId.ToString()})
                    .ToList();

            model.AccreditationLevelList.Insert(0, new SelectListItem { Selected = true, Text = "All", Value = "0" });
            model.CertificationDescriptorUrl = mLookupProvider.SystemValues.CertificationDescriptorUrl;
            model.ComplaintPolicyUrl = mLookupProvider.SystemValues.ComplaintPolicyUrl;

            model.LanguageList = BuildLanguages1(true, false);
            model.SecondLanguageList = BuildLanguages2(true, false);

            model.StateList = BuildAustralianStateList();

            model.FirstLanguageId = 0;
            model.SecondLanguageId = mLookupProvider.SystemValues.EnglishLanguageId;

            if (setDefaults)
            {
                model.CountryId = mLookupProvider.SystemValues.AustraliaCountryId;
            }

            return model;
        }

        private int ParseSkillId(CredentialTypeLookup x)
        {
            return x.SamId == 0 ? 0 : int.Parse(x.SamId.ToString().Substring(x.SamId.ToString().Length - 5));
        }

        private IList<SelectListItem> BuildLanguages(IEnumerable<LanguageLookup> languages, bool includeEnglishOption, bool includeAllOption)
        {

            var sourceList = languages.Where(x => includeEnglishOption || x.SamId != mLookupProvider.SystemValues.EnglishLanguageId)
                .OrderBy(x => x.DisplayText).Select(y => new SelectListItem { Selected = false, Text = y.DisplayText, Value = y.SamId.ToString() }).ToList();

            if (includeAllOption)
            {
                sourceList.Insert(0, new SelectListItem { Selected = true, Text = "All", Value = "0" });
            }
            return sourceList.ToList();
        }

        public IList<SelectListItem> BuildLanguages1(bool includeEnglishOption, bool includeAllOption)
        {
            return BuildLanguages(mLookupProvider.CertificationCredentialLanguages1, includeEnglishOption,
                includeAllOption);
        }

        public IList<SelectListItem> BuildLanguages2(bool includeEnglishOption, bool includeAllOption)
        {
            return BuildLanguages(mLookupProvider.CertificationCredentialLanguages2, includeEnglishOption,
                includeAllOption);
        }

        private IList<SelectListItem> BuildCountryList()
        {
            var allItem = new[] { new SelectListItem { Selected = false, Text = "All", Value = "0" } };
            var allCountries = mLookupProvider.Countries
                .OrderBy(c => c.DisplayText)
                .Select(x => new SelectListItem { Selected = false, Text = x.DisplayText, Value = x.SamId.ToString() });
            return allItem.Union(allCountries).ToList();
        }

        private IList<SelectListItem> BuildAustralianStateList()
        {
            var allItem = new[] { new SelectListItem { Selected = false, Text = "All", Value = "0" } };
            var australianStates = mLookupProvider.States
                .Where(s => s.IsAustralian)
                .Select(x => new SelectListItem { Selected = false, Text = x.Abbreviation, Value = x.SamId.ToString() });
            return allItem.Union(australianStates).ToList();
        }

        public IList<SelectListItem> BuildLanguageList(bool includeEnglishOption, bool includeAllOption)
        {
            var list = new List<SelectListItem>();

            if (includeAllOption)
                list.Insert(0, new SelectListItem { Selected = true, Text = "All", Value = "0" });

            var sourceList = mLookupProvider.Languages
                .Where(x => includeEnglishOption || x.SamId != mLookupProvider.SystemValues.EnglishLanguageId)
                .OrderBy(x => x.DisplayText);

            foreach (var item in sourceList)
            {
                var name = new StringBuilder();
                name.Append(item.DisplayText);

                if (name.ToString().Contains("Mandarin"))
                {
                    name.Append(" (Interpreter Only - Translator pre 1995)");
                }
                else
                {
                    if (item.SamId != mLookupProvider.SystemValues.EnglishLanguageId)
                    {
                        if (item.HasTranslatorTest && !item.HasInterpreterTest)
                        {
                            name.Append(" (Translator Only)");
                        }

                        if (!item.HasTranslatorTest && item.HasInterpreterTest)
                        {
                            name.Append(" (Interpreter Only)");
                        }
                    }
                }

                list.Add(new SelectListItem { Selected = false, Text = name.ToString(), Value = item.SamId.ToString() });
            }

            return list;
        }
    }
}