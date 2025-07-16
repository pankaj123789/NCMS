namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class EFTMachine : EntityBase, ILookupType
    {
        public virtual string TerminalNo { get; set; }
        public virtual Office Office { get; set; }
        public virtual bool Visible { get; set; }

        public override string ToString()
        {
            return TerminalNo;
        }
    }
}
