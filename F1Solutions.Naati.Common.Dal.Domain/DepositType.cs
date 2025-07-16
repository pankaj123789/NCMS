namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class DepositType : LegacyEntityBase, ILookupType
    {
        public DepositType(int id)
            : base(id)
        {
        }

        protected DepositType()
        {
        }

        public virtual string Type { get; set; }

        public override string ToString()
        {
            return Type;
        }
    }
}
