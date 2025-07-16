namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class Title : EntityBase, ILookupType
    {
        public virtual string TitleName { get; set; }
        public virtual bool StandardTitle { get; set; }

        public override string ToString()
        {
            return TitleName;
        }
    }
}
