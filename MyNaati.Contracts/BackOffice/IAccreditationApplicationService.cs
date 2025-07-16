using F1Solutions.Naati.Common.Contracts.Dal.Portal;

namespace MyNaati.Contracts.BackOffice
{
    
    public interface IAccreditationApplicationService
    {
        
        GetCourseFeesResponse GetCourseFees();

        
        GetTestingFeesResponse GetTestingFees();

        
        TestExistsResponse TestExists(TestExistsRequest request);
    }

    
    public class GetCourseFeesResponse
    {
        
        public ApplicationFee[] ApplicationFees { get; set; }
    }

    
    public class GetTestingFeesResponse
    {
        
        public ApplicationFee[] ApplicationFees { get; set; }

        
        public TestingFee[] TestingFees { get; set; }
    }

    
    public class TestExistsRequest
    {
        
        public int LanguageId { get; set; }

        
        public bool? ToEnglish { get; set; }

        
        public AccreditationLevel Level { get; set; }

        
        public AccreditationCategoryKnownType Category { get; set; }
    }

    
    public class TestExistsResponse
    {
        
        public bool TestFound { get; set; }
    }

    
    public class ApplicationFee
    {
        
        public bool IsForFirstApplication { get; set; }

        
        public decimal? PriceAustralia { get; set; }

        
        public decimal? PriceOtherCountries { get; set; }
    }

    
    public class TestingFee
    {
        
        public string SkillString { get; set; }

        
        public bool? ToEnglish { get; set; }

        
        public decimal? PriceAustralianCitizens { get; set; }

        
        public decimal? PriceAustralianOther { get; set; }

        
        public decimal? PriceNewZealand { get; set; }

        
        public decimal? PriceOtherCountries { get; set; }
    }
}
