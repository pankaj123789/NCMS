using System;

namespace MyNaati.Contracts.Portal.Users
{
    
    public class EmailChangeRequest
    {
        
        public Guid UserId { get; set; }
        
        public string CurrentPrimaryEmail { get; set; }
        
        public string NewEmail { get; set; }
        
        public DateTime ExpiryDate { get; set; }
     
    }
}
