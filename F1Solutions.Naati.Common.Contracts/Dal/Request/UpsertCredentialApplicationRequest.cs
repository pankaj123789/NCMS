using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using static F1Solutions.Naati.Common.Contracts.Dal.CreatePrerequisiteApplicationsDalModel;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class UpsertCredentialApplicationRequest
    {
        public UpsertCredentialApplicationRequest()
        {
            Attachments = new List<CreateOrReplaceApplicationAttachmentRequest>();
        }

        public int ApplicationId { get; set; }
        public int ApplicationTypeId { get; set; }
        public int ApplicationStatusTypeId { get; set; }
        public int EnteredUserId { get; set; }
        public int ReceivingOfficeId { get; set; }
        public DateTime StatusChangeDate { get; set; }
        public int StatusChangeUserId { get; set; }
        public int OwnedByUserId { get; set; }
        public DateTime EnteredDate { get; set; }
        public int NaatiNumber { get; set; }
        public bool OwnedByApplicant { get; set; }
        public int SponsorInstitutionId { get; set; }
        public IEnumerable<ApplicationFieldData> Fields { get; set; }
        public IEnumerable<CredentialRequestData> CredentialRequests { get; set; }
        public IEnumerable<ApplicationNoteData> Notes { get; set; }
        public IEnumerable<PersonNoteData> PersonNotes { get; set; }
        public IEnumerable<StandardTestComponentContract> StandardTestComponents { get; set; }
        public IEnumerable<RubricTestComponentContract> RubricTestComponents { get; set; }
        public IEnumerable<PdActivityData> PdActivities { get; set; }
        public int PreferredTestLocationId { get; set; }
        public int SponsorInstitutionContactPersonId { get; set; }
        public int SponsorInstitutionNaatiNumber { get; set; }
        public RecertificationDto Recertification { get; set; }
        public IEnumerable<ProcessFeeDto> ProcessedFees { get; set; }
        public bool? AutoCreated { get; set; }
        public List<CreateOrReplaceApplicationAttachmentRequest> Attachments { get; set; }
    }
}