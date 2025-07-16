namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class MaterialRequestNote : EntityBase
    {
        public virtual MaterialRequest MaterialRequest { get; set; }
        public virtual Note Note { get; set; }
    }
}
