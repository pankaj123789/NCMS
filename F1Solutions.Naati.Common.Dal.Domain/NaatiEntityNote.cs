namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class NaatiEntityNote : EntityBase
    {
        public virtual NaatiEntity Entity { get; set; }
        public virtual Note Note { get; set; }
    }
}
