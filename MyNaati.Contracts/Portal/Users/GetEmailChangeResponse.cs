using System;
using System.Collections.Generic;

namespace MyNaati.Contracts.Portal.Users
{
    
    public class GetEmailChangeResponse
    {
        
        public IEnumerable<EmailChangeItem> Items { get; set; }
    }

    
    public class EmailChangeItem
    {
        
        public int Id { get; set; }

        
        public int Reference { get; set; }

        
        public string PrimaryEmail { get; set; }

        
        public string SecondaryEmail { get; set; }

        
        public DateTime Expiry { get; set; }
      
    }
}
