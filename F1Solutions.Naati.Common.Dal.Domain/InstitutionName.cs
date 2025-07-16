using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class InstitutionName : EntityBase
    {

        public virtual Institution Institution { get; set; }
        public virtual string Name { get; set; }
        public virtual string TradingName { get; set; }
        public virtual string Abbreviation { get; set; }
        public virtual DateTime EffectiveDate { get; set; }

        public override IAuditObject RootAuditObject
        {
            get { return Institution.RootAuditObject; }
        }
        public virtual void ChangeInsitution(Institution institution)
        {
            Institution = institution;
        }
    }
}
