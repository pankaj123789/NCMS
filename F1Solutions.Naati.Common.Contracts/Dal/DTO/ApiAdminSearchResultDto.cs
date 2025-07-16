using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class ApiAdminSearchResultDto
    {
        public int ApiAccessId { get; set; }
        public string Name { get; set; }
        public int InstitutionId { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public int Permissions { get; set; }
        public bool InActive {get;set;}
        public int OrgNaatiNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifiedUserId { get; set; }
    }
}