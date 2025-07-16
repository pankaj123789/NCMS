using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Portal
{
    
    public class ePortalSamUser
    {
        
        public int NaatiNumber { get; set; }

        
        public string Email { get; set; }

        
        public string FullName { get; set; }

        
        public bool Deceased { get; set; }

        
        public DateTime? WebAccountCreateDate { get; set; }
    }
}
