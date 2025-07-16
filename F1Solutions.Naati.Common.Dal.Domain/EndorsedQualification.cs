using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class EndorsedQualification : EntityBase
    {
        public virtual  Institution Institution { get; set; }
        public virtual  string Location { get; set; }
        public virtual  string Qualification { get; set; }
        public virtual CredentialType CredentialType { get; set; }
        public virtual DateTime EndorsementPeriodFrom { get; set; }
        public virtual DateTime EndorsementPeriodTo { get; set; }
        public virtual string Notes { get; set; }
        public virtual bool Active { get; set; }
        public override IAuditObject RootAuditObject => Institution;
    }
}
