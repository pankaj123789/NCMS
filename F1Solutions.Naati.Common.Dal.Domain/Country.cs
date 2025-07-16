namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class Country : EntityBase
    {
        public virtual string Name { get; set; }
        public virtual string Code { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
