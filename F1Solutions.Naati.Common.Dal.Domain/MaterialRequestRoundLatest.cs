namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class MaterialRequestRoundLatest : EntityBase
    {
        public virtual MaterialRequest MaterialRequest { get; set; }
        public virtual MaterialRequestRound MaterialRequestRound { get; set; }
    }
}
