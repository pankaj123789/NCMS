namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class SystemValue : EntityBase
    {
        public SystemValue()
        {
        }

        public virtual string ValueKey { get; set; }
        public virtual string Value { get; set; }
    }
}
