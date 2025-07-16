using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.Common;

namespace F1Solutions.Naati.Common.Contracts.Dal.Portal
{
    
    public class EPortalSamUserSearchCriteria
    {
        
        public int? NaatiNumber { get; set; }

        
        public string GivenName { get; set; }

        
        public string FamilyName { get; set; }

        
        public string Email { get; set; }

        
        public DateTime CreationDateFrom { get; set; }

        
        public DateTime CreationDateTo { get; set; }

        
        public int Start { get; set; }

        
        public int Length { get; set; }

        
        public Dictionary<string, SortDirection> Sort { get; set; }
    }
}
