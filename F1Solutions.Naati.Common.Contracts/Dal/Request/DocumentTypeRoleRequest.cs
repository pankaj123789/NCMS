namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class DocumentTypeRoleRequest
    {
        public int UserId { get; set; }
        public bool Download { get; set; }
        public bool Upload { get; set; }
    }
}