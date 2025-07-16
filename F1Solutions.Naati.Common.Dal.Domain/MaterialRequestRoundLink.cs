namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class MaterialRequestRoundLink : EntityBase
    {
        public virtual MaterialRequestRound MaterialRequestRound { get; set; }
        public virtual string Link { get; set; }

        public virtual User User { get; set; }
        public virtual Person Person { get; set; }
        public virtual bool NcmsAvailable { get; set; }
        
    }
}
