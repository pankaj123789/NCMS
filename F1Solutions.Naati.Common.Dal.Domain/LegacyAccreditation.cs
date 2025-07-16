using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class LegacyAccreditation : EntityBase
    {
        public virtual int LegacyAccreditationId { get; set; }
        public virtual int AccreditationId { get; set; }
        public virtual string Level { get; set; }
        public virtual string Category { get; set; }
        public virtual string Direction { get; set; }
        public virtual string Language1 { get; set; }
        public virtual string Language2 { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime? ExpiryDate { get; set; }
        public virtual bool IncludeInOD { get; set; }
        public virtual int NAATINumber { get; set; }
    }
}
