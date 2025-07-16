namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetPersonAttachmentsRequest
    {
        public int? PersonId { get; set; }
        public int? StoredFileId { get; set; }
        public DocumentTypeRoleRequest UserRestriction { get; set; }
    }
}