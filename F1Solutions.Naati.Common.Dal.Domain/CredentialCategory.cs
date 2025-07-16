namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialCategory : EntityBase, IDynamicLookupType
    {
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual int? WorkPracticePoints { get; set; }
        public virtual string WorkPracticeUnits { get; set; }

        public override string ToString()
        {
            return this.DisplayName;
        }
    }
}
