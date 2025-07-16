using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using MyNaati.Contracts.BackOffice.Language;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;

namespace MyNaati.Ui.ViewModels.ApplicationByTestingWizard
{
    public class TestDetailsModel
    {
        private ILookupProvider mLookupProvider;

        public TestDetailsModel()
        {
            mLookupProvider = ServiceLocator.Resolve<ILookupProvider>();
        }

        // The list of tests that the user is requesting.
        [MinimumItemCount(1, ErrorMessage = "You must add at least one test.")]
        [MaximumItemCount(5, ErrorMessage = "A maximum of 5 tests can be requested on a single application.")]
        public List<TestDetailsRow> TestRequestsList { get; set; }

        // The list of tests that a user can choose from.
        public List<TestDetails> RequestTestTypeList { get; set; }

        public IList<SelectListItem> LanguageTypeSelectListItems { get; set; }

        //This is here so we can refer to them in calls to HtmlHelper in the View
        //This whole object will actually be posted to the Json address update method,
        //since it seems hard to wrangle EditorFor into loading from one viewmodel 
        //and saving to another. Better solutions invited!
        public TestDetails TestForCreate { get; set; }
                
        public bool TestLocationIsOther { get; set; }

        public bool ApplicationTypeIsExpressionOfInterest { get; set; }

        public void BuildLists()
        {
            LanguageTypeSelectListItems = BuildLanguageTypeSelectList();
        }

        public void BuildLists(TestDetails.TestCombinationTypes testType)
        {
            LanguageTypeSelectListItems = BuildLanguageTypeSelectList(testType);
        }

        public IList<SelectListItem> BuildLanguageTypeSelectList()
        {
            // Only select languages that have a test available.
            IList<SelectListItem> languageTypeItems =
                mLookupProvider.Languages.Where((l => !l.IsIndigenousLanguage && (l.HasInterpreterTest || l.HasTranslatorTest)))
                    .Select(l => new SelectListItem() { Text = l.DisplayText, Value = l.SamId.ToString() }).OrderBy(i => i.Text).ToList();

            // or have are an eoi combination
            var EoiLanguages = mLookupProvider.EoiLanguages;

            foreach (EoiLanguageTransferObject language in EoiLanguages)
            {
                if (!languageTypeItems.Any(sli => sli.Value == language.LanguageId.ToString()))
                {
                    languageTypeItems.Add(new SelectListItem
                    {
                        Text = language.LanguageDisplay,
                        Value = language.LanguageId.ToString()
                    });
                }
            }


            return languageTypeItems;
        }

        public IList<SelectListItem> BuildLanguageTypeSelectList(TestDetails.TestCombinationTypes testType)
        {
            bool paraprofessionalTranslator = (testType == TestDetails.TestCombinationTypes.ParaprofessionalTranslatorANZOnly);
            bool professionalTranslator = (testType == TestDetails.TestCombinationTypes.ProfessionalTranslatorToEnglish ||
                                           testType == TestDetails.TestCombinationTypes.ProfessionalTranslatorFromEnglish);
            bool advancedTranslator = (testType == TestDetails.TestCombinationTypes.AdvancedTranslatorToEnglishANZOnly ||
                                       testType == TestDetails.TestCombinationTypes.AdvancedTranslatorFromEnglishANZOnly);
            bool paraprofessionalInterpreter = (testType == TestDetails.TestCombinationTypes.ParaprofessionalInterpreterANZOnly);
            bool professionalInterpreter = (testType == TestDetails.TestCombinationTypes.ProfessionalInterpreter);

            // Only select languages that have a test available.
            IList<SelectListItem> languageTypeItems = mLookupProvider.Languages.Where(
                l => !l.IsIndigenousLanguage &&
                     ((l.ParaprofessionalTranslator && paraprofessionalTranslator) ||
                      (l.ProfessionalTranslator && professionalTranslator) ||
                      (l.AdvancedTranslator && advancedTranslator) ||
                      (l.ParaprofessionalInterpreter && paraprofessionalInterpreter) ||
                      (l.ProfessionalInterpreter && professionalInterpreter)))
                .Select(l => new SelectListItem() { Text = l.DisplayText, Value = l.SamId.ToString() }).OrderBy(i => i.Text).ToList();
            var EoiLanguages = mLookupProvider.EoiLanguages;
            int level = 0;
            int category;
            switch (testType)
            {
                case TestDetails.TestCombinationTypes.ParaprofessionalTranslatorANZOnly:
                case TestDetails.TestCombinationTypes.ParaprofessionalInterpreterANZOnly:
                    level = 2;
                    break;
                case TestDetails.TestCombinationTypes.ProfessionalTranslatorToEnglish:
                case TestDetails.TestCombinationTypes.ProfessionalTranslatorFromEnglish:
                case TestDetails.TestCombinationTypes.ProfessionalInterpreter:
                    level = 3;
                    break;
                case TestDetails.TestCombinationTypes.AdvancedTranslatorToEnglishANZOnly:
                case TestDetails.TestCombinationTypes.AdvancedTranslatorFromEnglishANZOnly:
                    level = 4;
                    break;
            }
            switch (testType)
            {
                case TestDetails.TestCombinationTypes.ParaprofessionalInterpreterANZOnly:
                case TestDetails.TestCombinationTypes.ProfessionalInterpreter:
                    category = 1;
                    break;
                default:
                    category = 3;
                    break;
            }

            foreach (EoiLanguageTransferObject language in EoiLanguages)
            {
                if (language.CategoryId == category &&
                    language.LevelId == level &&
                    !languageTypeItems.Any(sli => sli.Value == language.LanguageId.ToString()))
                {
                    languageTypeItems.Add(new SelectListItem
                    {
                        Text = language.LanguageDisplay,
                        Value = language.LanguageId.ToString()
                    });
                }
            }
            languageTypeItems = languageTypeItems.OrderBy(i => i.Text).ToList();


            return languageTypeItems;
        }
    }

    public class TestDetailsRow
    {
        private ILookupProvider mLookupProvider;

        public TestDetailsRow()
        {
            mLookupProvider = ServiceLocator.Resolve<ILookupProvider>();   
        }

        private const string DIRECTION_TO_ENGLISH = "{0} to English";
        private const string DIRECTION_FROM_ENGLISH = "English to {0}";
        private const string DIRECTION_BOTH_DIRECTIONS = "{0} to/from English";
        private const string DIRECTION_NONE = "None";
        private const string LEVEL_2 = "Paraprofessional {0}";
        private const string LEVEL_3 = "Professional {0}";
        private const string LEVEL_4 = "Advanced {0}";
        private const string LEVEL_UNKNOWN = "Unknown";

        private string DirectionFormatString
        {
            get
            {
                if (ToEnglish && FromEnglish)
                    return DIRECTION_BOTH_DIRECTIONS;
                if (ToEnglish)
                    return DIRECTION_TO_ENGLISH;
                if (FromEnglish)
                    return DIRECTION_FROM_ENGLISH;
                return DIRECTION_NONE;
            }
        }

        public string Direction
        {
            get { return string.Format(DirectionFormatString, LanguageName); }
        }

        public string Skill { 
            get
            {
                if (IsInterpreter)
                    return mLookupProvider.AccreditationCategories.Single(e => e.KnownType == AccreditationCategoryKnownType.Interpreter).DisplayText;
                else
                    return mLookupProvider.AccreditationCategories.Single(e => e.KnownType == AccreditationCategoryKnownType.Translator).DisplayText;
            }
        }

        public string LevelName 
        { 
            get
            {
                switch (Level)
                {
                    case 2:
                        return string.Format(LEVEL_2, Skill);
                    case 3:
                        return string.Format(LEVEL_3, Skill);
                    case 4:
                        return string.Format(LEVEL_4, Skill);
                    default:
                        return string.Format(LEVEL_UNKNOWN, Skill);
                }
            }
        }

        public string LanguageName
        {
            get
            {
                return mLookupProvider.Languages
                    .Where(p => p.SamId == LanguageId)
                    .Select(p => p.DisplayText)
                    .SingleOrDefault();
            }
        }

        public bool IsInterpreter { get; set; }
        public int Level { get; set; }
        public bool ToEnglish { get; set; }
        public bool FromEnglish { get; set; }
        public int LanguageId { get; set; }

        public int Id { get; set; }

        /// <summary>
        /// Compares on value equality, but not id.
        /// </summary>
        public static bool Equals(TestDetailsRow obj, TestDetailsRow obj2)
        {
            if (obj == null || obj2 == null)
                return false;
            return (obj.IsInterpreter == obj2.IsInterpreter
                    && obj.Level == obj2.Level
                    && obj.ToEnglish == obj2.ToEnglish
                    && obj.FromEnglish == obj2.FromEnglish
                    && obj.LanguageId == obj2.LanguageId);
        }
    }

    public class TestDetails
    {
        public TestDetails()
        {}

        [DisplayName("Language")]
        [Required(ErrorMessage = "You must select a language.")]
        public int? LanguageId { get; set; }

        [NotZero(ErrorMessage = "You must select a credential type.")]
        public TestCombinationTypes RequestTestType { get; set; }

        public TestDetailsRow BuildTest()
        {
            var testDetails = new TestDetailsRow();

            // To english
            switch (RequestTestType)
            {
                case TestCombinationTypes.ParaprofessionalTranslatorANZOnly:
                case TestCombinationTypes.ProfessionalTranslatorToEnglish:
                case TestCombinationTypes.AdvancedTranslatorToEnglishANZOnly:
                case TestCombinationTypes.ParaprofessionalInterpreterANZOnly:
                case TestCombinationTypes.ProfessionalInterpreter:
                    testDetails.ToEnglish = true;
                    break;
            }

            // From english
            switch (RequestTestType)
            {
                case TestCombinationTypes.ParaprofessionalTranslatorANZOnly:
                case TestCombinationTypes.ProfessionalTranslatorFromEnglish:
                case TestCombinationTypes.AdvancedTranslatorFromEnglishANZOnly:
                case TestCombinationTypes.ParaprofessionalInterpreterANZOnly:
                case TestCombinationTypes.ProfessionalInterpreter:
                    testDetails.FromEnglish = true;
                    break;
            }

            // Level
            switch (RequestTestType)
            {
                case TestCombinationTypes.ParaprofessionalTranslatorANZOnly:
                case TestCombinationTypes.ParaprofessionalInterpreterANZOnly:
                    testDetails.Level = 2;
                    break;
                case TestCombinationTypes.ProfessionalTranslatorToEnglish:
                case TestCombinationTypes.ProfessionalTranslatorFromEnglish:
                case TestCombinationTypes.ProfessionalInterpreter:
                    testDetails.Level = 3;
                    break;
                case TestCombinationTypes.AdvancedTranslatorToEnglishANZOnly:
                case TestCombinationTypes.AdvancedTranslatorFromEnglishANZOnly:
                    testDetails.Level = 4;
                    break;
            }

            // Translator?
            switch (RequestTestType)
            {
                case TestCombinationTypes.ParaprofessionalInterpreterANZOnly:
                case TestCombinationTypes.ProfessionalInterpreter:
                    testDetails.IsInterpreter = true;
                    break;
            }

            testDetails.LanguageId = LanguageId.Value;

            return testDetails;
        }

        public enum TestCombinationTypes
        {
            None,
            ParaprofessionalTranslatorANZOnly,
            ProfessionalTranslatorToEnglish,
            ProfessionalTranslatorFromEnglish,
            AdvancedTranslatorToEnglishANZOnly,
            AdvancedTranslatorFromEnglishANZOnly,
            ParaprofessionalInterpreterANZOnly,
            ProfessionalInterpreter
        }
    }
}