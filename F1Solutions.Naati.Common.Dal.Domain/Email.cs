namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class Email : EntityBase
    {

        public virtual string EmailAddress { get; set; }
        public virtual NaatiEntity Entity { get; set; }        
        public virtual string Note { get; set; }
        public virtual bool IncludeInPD { get; set; }
        public virtual bool IsPreferredEmail { get; set; }
        public virtual bool Invalid { get; set; }

        public virtual bool ExaminerCorrespondence { get; set; }

        public override IAuditObject RootAuditObject
        {
            get
            {
                return Entity.RootAuditObject;
            }
        }
    }
}
