using System;

namespace Ncms.Contracts.Models.Application
{
    public class CreateCredentialDocumentsRequestModel
    {
        public int CredentialTypeId { get; set; }
        public int NaatiNumber { get; set; }
        public int CredentialId { get; set; }
        public int ApplicationId { get; set; }
        public int UserId { get; set; }
        public DateTime CredentialStartDate { get; set; }
        public DateTime? CredentialExpiryDate { get; set; }
        public string Skill { get; set; }
        public string CredentialName { get; set; }
        public string PractitionerNumber { get; set; }
    }

    public class DocumentsPreviewRequestModel
    {

        public int ActionId { get; set; }
        public int ApplicationId { get; set; }
        public int CredentialRequestId { get; set; }
        public DateTime CredentialStartDate { get; set; }
        public DateTime? CredentialExpiryDate { get; set; }
        public string PractitionerNumber { get; set; }

    }
}
