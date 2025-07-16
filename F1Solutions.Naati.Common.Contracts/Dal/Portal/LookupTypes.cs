using System;
using System.Linq;
using System.Runtime.Serialization;

namespace F1Solutions.Naati.Common.Contracts.Dal.Portal
{
    
    public class LookupItemBase
    {
        
        public int SamId { get; set; }

        
        public string DisplayText { get; set; }
    }

     
    public class LanguageLookup : LookupItemBase
    {
    }

    
    public class CredentialTypeLookup : LookupItemBase
    {
        
        public string SkillName { get; set; }
    }

    
    public class AccreditationCategory : LookupItemBase
    {
        
        public AccreditationCategoryKnownType KnownType { get; set; }
    }

    
    public enum AccreditationCategoryKnownType
    {
        [EnumMember]
        Translator = 1,

        [EnumMember]
        Interpreter = 2,

        [EnumMember]
        LanguageAide = 4,
    }

    
    public class Language : LookupItemBase
    {
        
        public bool HasTranslatorTest { get; set; }

        
        public bool HasInterpreterTest { get; set; }

        
        public bool ParaprofessionalTranslator { get; set; }

        
        public bool ProfessionalTranslator { get; set; }

        
        public bool AdvancedTranslator { get; set; }

        
        public bool ParaprofessionalInterpreter { get; set; }

        
        public bool ProfessionalInterpreter { get; set; }

        
        public bool IsEoiLanguage { get; set; }

        
        public bool IsIndigenousLanguage { get; set; }
    }

    
    public class AccreditationLevel : LookupItemBase
    {
        
        public bool IsLanguageAide { get; set; }
    }

    
    public class AccreditationType : LookupItemBase
    {
        
    }

    
    public class State : LookupItemBase
    {
        
        public string Abbreviation { get; set; }

        
        public bool IsAustralian { get; set; }
    }

    
    public class Country : LookupItemBase
    {
        
        public bool IsHomeCountry { get; set; }
    }

	public class TestLocation : LookupItemBase
	{
	}

	public class Postcode : LookupItemBase
    {
        
        public int SuburbId { get; set; }

        
        public string Suburb { get; set; }

        
        public string Code { get; set; }

        
        public string State { get; set; }
    }

    
    public class SystemValue
    {
        
        public string Key { get; set; }
        
        public string Value { get; set; }
    }

    
    public class PersonTitle : LookupItemBase
    {
        
        public bool IsStandardTitle { get; set; }
    }

    
    public class OdAddressVisibilityTypeLookup : LookupItemBase
    {
    }

    
    public class Institution : LookupItemBase
    {
        //
        //public bool IsDomestic { get; set; }

        
        public bool HasAustralianAddress { get; set; }

        
        public string AbbreviatedName { get; set; }

        
        public DateTime? LatestEndDateForApprovedCourse { get; set; }

        
        public bool HasIndigenousLanguagesOnly { get; set; }
    }

    
    public class Course : LookupItemBase
    {
        
        public int InstitutionId { get; set; }

        
        public LanguageApproval[] ApprovedForLanguages { get; set; }

        public DateTime? LatestApprovalEndDate
        {
            get
            {
                if (ApprovedForLanguages == null || ApprovedForLanguages.Length == 0)
                    return null;

                return ApprovedForLanguages.Select(l => l.EndDate).Max();
            }
        }

        
        public class LanguageApproval
        {
            
            public int LanguageId { get; set; }

            
            public DateTime? StartDate { get; set; }

            
            public DateTime? EndDate { get; set; }
        }
    }
}