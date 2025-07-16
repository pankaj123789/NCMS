namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class RolePlayerTestLocation : EntityBase
    {
        public virtual RolePlayer RolePlayer { get; set; }
        public virtual TestLocation TestLocation { get; set; }
	}
}
