using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class AddTestSpecificationRequest
    {
        public string Title { get; set; }
        public int CredentialTypeId { get; set; }
        public int ModifiedUser { get; set; }
        public bool ModifiedByNaati { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
