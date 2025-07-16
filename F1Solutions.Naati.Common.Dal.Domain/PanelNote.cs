namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class PanelNote : EntityBase
    {
        public virtual Panel Panel { get; set; }
        public virtual Note Note { get; set; }
    }
}
