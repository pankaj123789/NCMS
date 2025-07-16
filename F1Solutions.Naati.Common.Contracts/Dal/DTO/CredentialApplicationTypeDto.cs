using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialApplicationTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public CredentialApplicationTypeCategoryName Category { get; set; }
        public bool Online { get; set; }
        public bool RequiresChecking { get; set; }
        public bool RequiresAssessment { get; set; }
        public bool BackOffice { get; set; }
        public virtual bool PendingAllowed { get; set; }
        public virtual bool AssessmentReviewAllowed { get; set; }
        public IList<CredentialApplicationFieldDto> CredentialApplicationFields { get; set; }
        public IList<CredentialApplicationTypeDocumentTypeDto> CredentialApplicationTypeDocumentTypes { get; set; }
    }
}
