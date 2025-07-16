
namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestSessionSkill : EntityBase
    {
        public virtual TestSession TestSession{ get; set; }
        public virtual Skill Skill { get; set; }
        public virtual int? Capacity { get; set; }
    }
}
