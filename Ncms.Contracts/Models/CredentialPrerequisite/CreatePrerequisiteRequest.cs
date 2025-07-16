using System;
using System.Collections.Generic;

namespace Ncms.Contracts.Models.CredentialPrerequisite
{
    public class ValidatePrerequisiteResponse
    {
        public CreatePrerequisiteRequest CreatePrerequisiteRequest { get;set;}
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

    public class PrerequisiteExemptionRequest
    {
        public IList<PrerequisiteExemption> PrerequisiteExemptions { get; set; }
    }

    public class PrerequisiteExemption
    {
        public int PrerequisiteExemptionId { get; set; }
        public string PrerequisiteCredentialName { get; set; }
        public string PrerequisiteSkill { get; set; }
        public string ExemptedCredentialName { get; set; }
        public int ExemptedCredentialTypeId { get; set; }
        public string ExemptedCredentialSkill { get; set; }
        public int ExemptedCredentialSkillId { get; set; }
        public int PersonId { get; set; }
        public DateTime? ExemptionStartDate { get; set; }
        public DateTime? ExemptionEndDate { get; set; }
        public string ExemptionGrantedByUser { get; set; }
        public int ExemptionGrantedByUserId { get; set; }
        public bool Checked { get; set; }
    }
}
