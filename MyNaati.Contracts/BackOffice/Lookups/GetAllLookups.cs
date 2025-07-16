using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using MyNaati.Contracts.BackOffice.Language;

namespace MyNaati.Contracts.BackOffice.Lookups
{
    
    public class GetAllLookupsRequest
    {
    }

    
    public class GetAllLookupsResponse
    {
        
        public AccreditationCategory[] AccreditationCategories { get; set; }

        
        public F1Solutions.Naati.Common.Contracts.Dal.Portal.Language[] Languages { get; set; }

        
        public LanguageLookup[] CertificatesLanguages1 { get; set; }

        public TestLocation[] TestLocations { get; set; }

		public LanguageLookup[] CertificatesLanguages2 { get; set; }

        
        public EoiLanguageTransferObject[] EoiLanguages { get; set; }

        
        public AccreditationLevel[] AccreditationLevels { get; set; }

        
        public CredentialTypeLookup[] CertificatesCredentialTypes { get; set; }

        
        public AccreditationType[] AccreditationTypes { get; set; }

        
        public State[] States { get; set; }

        
        public Country[] Countries { get; set; }

        
        public LookupItemBase[] ContactTypes { get; set; }

        
        public Postcode[] Postcodes { get; set; }

        
        public PersonTitle[] PersonTitles { get; set; }

        
        public OdAddressVisibilityTypeLookup[] OdAddressVisibilityTypes { get; set; }

        
        public Institution[] Institutions { get; set; }

        
        public Course[] Courses { get; set; }
    }

    
    public class CertificatesCredentialTypesResponse
    {
        
        public CredentialTypeLookup[] CertificatesCredentialTypes { get; set; }
    }

}
