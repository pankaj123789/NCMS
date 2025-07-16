namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestSessionRolePlayerDetail : EntityBase
    {
        public virtual TestSessionRolePlayer TestSessionRolePlayer { get; set; }
        public virtual TestComponent TestComponent { get; set; }
        public virtual Skill Skill { get; set; }
        public virtual Language Language { get; set; }
        public virtual RolePlayerRoleType RolePlayerRoleType { get; set; }
    
	}
}
