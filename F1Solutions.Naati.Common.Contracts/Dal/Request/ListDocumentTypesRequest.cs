using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class ListDocumentTypesRequest
    {
        public DocumentTypeCategoryTypeName? Category { get; set; }
        public DocumentTypeRoleRequest UserRestriction { get; set; }
    }
}