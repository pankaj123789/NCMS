namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class WorkPracticeCredentialRequest : EntityBase
    {
        public virtual WorkPractice WorkPractice { get; set; }
        public virtual CredentialRequest CredentialRequest { get; set; }
    }
}