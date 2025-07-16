namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class ContactPersonDto
    {
        public int Id { get; set; }
        public int InstitutionId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PostalAddress { get; set; }
        public string Description { get; set; }
        public bool Inactive { get; set; }
    }
}