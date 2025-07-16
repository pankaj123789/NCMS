using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Contracts.Models.Application
{
    public class CredentialApplicationTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public CredentialApplicationTypeCategoryName Category { get; set; }
        public bool Online { get; set; }
        public bool RequiresChecking { get; set; }
        public bool RequiresAssessment { get; set; }
        public bool BackOffice { get; set; }
        public bool PendingAllowed { get; set; }
        public bool AssessmentReviewAllowed { get; set; }
        public IList<CredentialApplicationFieldModel> CredentialApplicationFields { get; set; }
        public IList<CredentialApplicationTypeDocumentTypeModel> CredentialApplicationTypeDocumentTypes { get; set; }
        public CredentialApplicationTypeName CredentialApplicationType => (CredentialApplicationTypeName)Id;
    }
}
