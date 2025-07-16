namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class RolePlayerSettingsDto
    {
        public int[] RolePlayLocations { get; set; }
        public int MaximumRolePlaySessions { get; set; }
        public  decimal Rating { get; set;}
        public bool Senior { get; set; }
        public int NaatiNumber { get; set; }
    }
}