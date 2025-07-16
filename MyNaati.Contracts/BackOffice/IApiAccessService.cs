using System;
using F1Solutions.Naati.Common.Contracts.Bl;

namespace MyNaati.Contracts.BackOffice
{
    
    public interface IApiAccessService : IInterceptableservice
    {
       
        ApiAccessResponse GetApiAccess(string publicKey);
    }

    
    public class ApiAccessResponse
    {
        
        public int InstitutionId { get; set; }
        
        public string PrivateKey { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime ModifiedDate { get; set; }
        
        public int ModifiedUserId { get; set; }
        
        public bool Inactive { get; set; }
        
        public string InstitutionName { get; set; }

        public int Permissions { get; set; }
    }
}
