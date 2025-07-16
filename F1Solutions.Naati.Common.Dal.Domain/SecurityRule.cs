
namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class SecurityRule : EntityBase
    {
        public virtual SecurityRole SecurityRole { get; set; }
        public virtual SecurityNoun SecurityNoun { get; set; }
        public virtual long SecurityVerbMask { get; set; }
    }
}
