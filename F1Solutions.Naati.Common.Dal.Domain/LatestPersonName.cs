namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class LatestPersonName : EntityBase
    {
        public virtual Person Person { get; set; }
        public virtual PersonName PersonName { get; set; }
    }
}
