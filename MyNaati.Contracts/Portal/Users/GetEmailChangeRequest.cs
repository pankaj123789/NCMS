using System;

namespace MyNaati.Contracts.Portal.Users
{
    
    public class GetEmailChangeRequest
    {
        
        public int Reference { get; set; }

        
        public DateTime ExpiryDate { get; set; }
    }
}
