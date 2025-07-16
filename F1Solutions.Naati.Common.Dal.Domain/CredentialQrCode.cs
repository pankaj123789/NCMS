using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialQrCode : EntityBase
    {
        public virtual Credential Credential { get; set; }
        public  virtual DateTime IssueDate { get; set; }
        public  virtual Guid QrCodeGuid { get; set; }
        public virtual DateTime? InactiveDate { get; set; }
        public virtual int ModifiedBy { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
    }
}
