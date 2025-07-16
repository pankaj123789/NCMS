using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.Attributes;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Contracts.Models.MaterialRequest;

namespace Ncms.Contracts.Models.Application
{
    public class LookupTypeModel
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
    }

    public class DocumentLookupTypeModel : LookupTypeModel
    {
        [LookupDisplay(LookupType.DocumentType)]
        public int DocumentTypeId { get; set; }
        public long Size { get; set; }
        public string FileType { get; set; }
        public string UploadedBy { get; set; }
        public bool ExaminersAvailable { get; set; }
        public bool MergeDocument { get; set; }
    }

    public class PanelMembershipLookupModel : LookupTypeModel
    {
        public int NaatiNumber { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }

        public IEnumerable<MaterialRequestTaskModel> Tasks { get; set; }
        public bool PreSelected { get; set; }
        public bool IsCoordinatorCredentialType { get; set; }
        public bool IsCoordinator { get; set; }

    }
    public class LookupTypeDetailedModel : LookupTypeModel
    {
        public string Name { get; set; }
    }

    public class CredentialLookupTypeModel : LookupTypeModel
    {
        public int DisplayOrder { get; set; }
        public int CategoryId { get; set; }
    }

    public class TestSpecificationLookupTypeModel : LookupTypeModel
    {
        public bool Active { get; set; }
    }
}
