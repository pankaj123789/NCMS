using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SaveApiAdminRequest
    {
        public int? ApiAccessId { get; set; }
        public int Permissions { get; set; }
        public int InstitutionId { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public int ModifiedUser { get; set; }
        public bool ModifiedByNaati { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool Inactive { get; set; }
    }
}