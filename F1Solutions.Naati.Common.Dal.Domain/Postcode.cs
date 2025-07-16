namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class Postcode : EntityBase
    {
        public virtual string PostCode { get; set; }
        public virtual Suburb Suburb { get; set; }

        public override string ToString()
        {
            return PostCode;
        }
    }
}
