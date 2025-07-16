using System;

namespace MyNaati.Contracts.Portal
{
    
    public class ePortalUser
    {
        
        public string Username { get; set; }

        
        public bool IsLocked { get; set; }

        
        public bool IsActive { get; set; }

        
        public string Email { get; set; }

        
        public DateTime CreationDate { get; set; }

        
        public Guid UserId { get; set; }

        
        public DateTime LastPasswordChangedDate { get; set; }

        
        public DateTime LastLoginDate { get; set; }
    }
}
