using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    [Serializable]
    public class SystemValuesDto
    {
        public string ProdDatabaseName { get; set; }
        public string IDCardPath { get; set; }
        public string BaseReportPath { get; set; }
        public string CertificateFeeProductAustraliaId { get; set; }
        public string CertificateFeeProductOverseasId { get; set; }
        public string IdCardFeeProductAustraliaId { get; set; }
        public string IdCardFeeProductOverseasId { get; set; }
        public string DefaultCountryId { get; set; }
        public string StampFeeProductRubberAustraliaId { get; set; }
        public string StampFeeProductRubberOverseasId { get; set; }
        public string StampFeeProductSelfInkingAustraliaId { get; set; }
        public string StampFeeProductSelfInkingOverseasId { get; set; }
        public string PDListingOverseasProductId { get; set; }
        public string ProductOrderCorrespondenceCategoryId { get; set; }
        public string OrderStandardLetterCategoryId { get; set; }
        public string OnlineOfficeId { get; set; }
        public string SharedOnlineUserId { get; set; }
        public string AccreditationTypeId { get; set; }
        public string RecognitionTypeId { get; set; }
        public string PDReminderFirstEmailLeadTimeInDays { get; set; }
        public string PDReminderFirstEmailTemplateId { get; set; }
        public string PDReminderSecondEmailLeadTimeInDays { get; set; }
        public string PDReminderSecondEmailTemplateId { get; set; }
        public string AutomatedCorrespondenceUserId { get; set; }
        public string SendPDReminderEmails { get; set; }
        public string ScanContentTypeCategory { get; set; }
        public string SharePointServerURL { get; set; }
        public string ProcessingNoteCorrespondenceCategoryId { get; set; }
        public string OfficialCredentialRecordCorrespondenceCategoryId { get; set; }
        public string FormMFeeId { get; set; }
        public string DefaultEmailSenderName { get; set; }
        public string DefaultEmailSenderAddress { get; set; }
        public string OnlineEFTMachineId { get; set; }
        public string OnlineOrderTypeId { get; set; }
        public string WiisePaymentAccount { get; set; }
    }
}