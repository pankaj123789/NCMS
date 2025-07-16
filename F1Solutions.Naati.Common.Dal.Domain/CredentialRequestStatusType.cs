namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CredentialRequestStatusType : EntityBase, IDynamicLookupType
    {
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual int DisplayOrder { get; set; }
        public override string ToString()
        {
            return this.DisplayName;
        }
    }

  
}
