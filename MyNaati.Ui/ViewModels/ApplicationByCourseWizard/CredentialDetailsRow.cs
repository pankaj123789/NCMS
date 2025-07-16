using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using MyNaati.Contracts.Portal;

namespace MyNaati.Ui.ViewModels.ApplicationByCourseWizard
{
    public class CredentialDetailsRow
    {
        public CredentialDetailsRow()
        {
            mLookupProvider = ServiceLocator.Resolve<ILookupProvider>();   
        }

        private const string DIRECTION_TO_ENGLISH = "{0} to English";
        private const string DIRECTION_FROM_ENGLISH = "English to {0}";
        private const string DIRECTION_BOTH_DIRECTIONS = "{0} to/from English";
        private const string DIRECTION_NONE = "None";
        private const string LEVEL_2 = "Paraprofessional";
        private const string LEVEL_3 = "Professional";
        private const string LEVEL_4 = "Advanced Professional";
        private const string LEVEL_UNKNOWN = "Unknown";
        private const string CATEGORY_UNKNOWN = "Unknown";

        private ILookupProvider mLookupProvider;

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
                else if (IsTranslator)
                    return mLookupProvider.AccreditationCategories.Single(e => e.KnownType == AccreditationCategoryKnownType.Translator).DisplayText;
                else
                    return CATEGORY_UNKNOWN;
            }
        }

        public string LevelName 
        { 
            get
            {
                switch (Level)
                {
                    case 2:
                        return LEVEL_2;
                    case 3:
                        return LEVEL_3;
                    case 4:
                        return LEVEL_4;
                    default:
                        return LEVEL_UNKNOWN;
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
        public bool IsTranslator { get; set; }
        public int Level { get; set; }
        public bool ToEnglish { get; set; }
        public bool FromEnglish { get; set; }
        public int LanguageId { get; set; }

        public static bool Equals(CredentialDetailsRow obj, CredentialDetailsRow obj2)
        {
            if (obj == null || obj2 == null)
                return false;
            return (obj.IsInterpreter == obj2.IsInterpreter
                    && obj.IsTranslator == obj2.IsTranslator
                    && obj.Level == obj2.Level
                    && obj.ToEnglish == obj2.ToEnglish
                    && obj.FromEnglish == obj2.FromEnglish
                    && obj.LanguageId == obj2.LanguageId);
        }
    }
}