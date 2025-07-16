using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class ValidatePrerequisiteResponse
    {
        public CreatePrerequisiteRequest CreatePrerequisiteRequest { get; set; }
        public List<MissingMandatoryField> MissingMandatoryFields { get; set; }
        public List<MissingMandatoryDocument> MissingMandatoryDocuments { get; set; }
    }

    public class MissingMandatoryDocument
    {
        public string PrerequisiteCredential { get; set; }
        public string ApplicationType { get; set; }
        public string MandatoryDocumentType { get; set; }
    }

    public class MissingMandatoryField
    {
        public string PrerequisiteCredential { get; set; }
        public string ApplicationType { get; set; }
        public string MandatoryInformation { get; set; }
    }

    public class CreatePrerequisiteRequest
    {
        public int ParentApplicationId { get; set; }
        public int ParentCredentialRequestId { get; set; }
        public IList<ChildPreRequisites> CredentialRequestTypes { get; set; }
        public int EnteredUserId { get; set; }
        public bool CreateApplications { get; set; }
    }
    public class ChildPreRequisites
    {
        public string CredentialRequestType { get; set; }
        public int ApplicationTypeId { get; set; }
        public bool HasValidationError { get; set; }
    }
}

