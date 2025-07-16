using System.Collections.Generic;
using SortDirection = F1Solutions.Naati.Common.Contracts.Dal.Portal.Common.SortDirection;

namespace F1Solutions.Naati.Common.Contracts.Dal.Portal
{
    
    public class PractitionerSearchCriteria
    {
        //
        //public int? AccreditationCategoryId { get; set; }
        
        public int? FirstLanguageId { get; set; }

        
        public int? SecondLanguageId { get; set; }

        
        public int? AccreditationLevelId { get; set; }

        
        public bool? ToEnglish { get; set; }

        
        public int? StateId { get; set; }

        
        public int? CountryId { get; set; }

        
        public string Postcode { get; set; }

        //
        //public string Suburb { get; set; }

        
        public string Surname { get; set; }

        
        public int PageNumber { get; set; }

        
        public int PageSize { get; set; }

        
        public SortDirection SortOrder { get; set; }

        
        public PDSortMember SortMember { get; set; }

        
        public int RandomSearchSeed { get; set; }

        
        public IList<int> Skills { get; set; }
    }

    public enum PDSortMember
    {
        None,
        Level,
        State,
        Suburb
    }
}