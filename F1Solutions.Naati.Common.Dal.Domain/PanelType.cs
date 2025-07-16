namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class PanelType : EntityBase, ILookupType
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual bool AllowCredentialTypeLink { get; set; }

        public override string ToString()
        {
            return Description;
        }
    }
}
