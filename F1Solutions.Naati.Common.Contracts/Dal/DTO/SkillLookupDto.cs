namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class SkillLookupDto : LookupTypeDto
    {
        public int CredentialTypeId { get; set; }
        public int DirectionTypeId { get; set; }
        public int Language1Id { get; set; }
        public int Language2Id { get; set; }
    }
}