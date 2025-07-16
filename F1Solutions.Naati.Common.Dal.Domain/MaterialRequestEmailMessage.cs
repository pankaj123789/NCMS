namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class MaterialRequestEmailMessage : EntityBase
    {
        public virtual MaterialRequest MaterialRequest { get; set; }
        public virtual EmailMessage EmailMessage { get; set; }
    }
}
