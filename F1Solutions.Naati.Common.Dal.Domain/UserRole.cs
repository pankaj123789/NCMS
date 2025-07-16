namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class UserRole : EntityBase
    {
        public virtual User User { get; set; }
        public virtual SecurityRole SecurityRole { get; set; }

        public override IAuditObject RootAuditObject
        {
            get
            {
                return User.RootAuditObject;
            }
        }
    }
}
