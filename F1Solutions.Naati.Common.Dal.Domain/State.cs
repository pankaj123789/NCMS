
namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class State : EntityBase, ILookupType
    {
        public virtual string Abbreviation { get; set; }
        public virtual string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
