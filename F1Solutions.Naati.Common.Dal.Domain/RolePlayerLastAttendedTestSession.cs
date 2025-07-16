namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class RolePlayerLastAttendedTestSession : RolePlayer
    {

        public virtual RolePlayer RolePlayer { get; set; }
        public virtual TestSession LastAttendedTestSession { get; set; }
    }
}
