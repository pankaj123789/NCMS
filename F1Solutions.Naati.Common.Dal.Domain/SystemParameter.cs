namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class SystemParameter : LegacyEntityBase
    {
        public SystemParameter(int id)
            : base(id)
        {
        }

        protected SystemParameter()
        {
        }

        public virtual int DefaultCacheExpiryMinutes { get; set; }
        public virtual int DefaultKeyAllocation { get; set; }
        public virtual int CertificateProductId { get; set; }
        public virtual int IDCardProductId { get; set; }
        public virtual int StampProductId { get; set; }
        public virtual int DefaultMaxiumOpportunities { get; set; }
        public virtual int DefaultDueDatePeriod { get; set; }
        public virtual int RecognitionAndLanguageAideLevelId { get; set; }
        public virtual int TestingMethodId { get; set; }
        public virtual int CourseMethodId { get; set; }
        public virtual int RevalidationMethodId { get; set; }
        public virtual int DeafInterpreterRecognitionMethodId { get; set; }
        public virtual int QualificationWithoutMigrationMethodId { get; set; }
        public virtual int QualificationWithMigrationMethodId { get; set; }
        public virtual int LanguageAideMethodId { get; set; }
        public virtual int RecognitionMethodId { get; set; }
        public virtual int InterpreterCategoryId { get; set; }
        public virtual int LanguageAideCategoryId { get; set; }
        public virtual int TranslatorCategoryId { get; set; }
        public virtual int IntialApplicationFeeProductID { get; set; }
        public virtual int ReninstatementApplicationFeeProductId { get; set; }
        public virtual int CourseParaprofessionalAssementFeeProductId { get; set; }
        public virtual int CourseProfessionalAssementFeeProductId { get; set; }
        public virtual int CourseAdvancedAssementFeeProductId { get; set; }
        public virtual int QualificationAsssementFeeProductId { get; set; }
        public virtual int QualificationNonCitizenAsssementFeeProductId { get; set; }
        public virtual int ScheduledTestProductCategoryId { get; set; }
        public virtual int StandardLetterCorrespondenceCategoryId { get; set; }
        public virtual int DefaultOfficeId { get; set; }
        public virtual int PDListingProductId { get; set; }
        public virtual int ParaprofessionalLevelId { get; set; }
        public virtual int ProfessionalLevelId { get; set; }
        public virtual int AdvancedLevelId { get; set; }
        public virtual int ReminderCheckInterval { get; set; }
        public virtual int DefaultAccreditationResultDays { get; set; }
        public virtual int PassResultTypeId { get; set; }
        public virtual int ConcededPassResultTypeId { get; set; }
        public virtual int FailResultTypeId { get; set; }
        public virtual int JobStandardLetterCategoryId { get; set; }
        public virtual string TestMaterialPath { get; set; }
        public virtual string ReportXMLPath { get; set; }
        public virtual int NAATINewsProductId { get; set; }
        public virtual int DefaultSubscriptionPeriod { get; set; }
        public virtual int CreateInvitationTimeout { get; set; }
        public virtual string PaymentMethodDirectDeposit { get; set; }
        public virtual string PaymentMethodCashCheque { get; set; }
        public virtual string PaymentMethodAMEX { get; set; }
        public virtual string JournalMemoPrefix { get; set; }
        public virtual string TaxCode { get; set; }
        public virtual string MYOBDirectory { get; set; }
        public virtual int ApplicationStandardLetterCategoryId { get; set; }
        public virtual int ResultStandardLetterCategoryId { get; set; }
        public virtual int EnglishLanguageId { get; set; }
        public virtual int AuslanLanguageId { get; set; }
        public virtual int OverseasEventVenueId { get; set; }
        public virtual string OutstandingInvoiceStartDate { get; set; }
        public virtual int AMEXCardTypeId { get; set; }
        public virtual int DefaultCountryId { get; set; }
        public virtual int CurrentFinancialYearId { get; set; }
        public virtual int CertRecStandardLetterCategoryId { get; set; }
        public virtual int CertAccStandardLetterCategoryID { get; set; }
        public virtual int IDAccStandardLetterCategoryID { get; set; }
        public virtual int IDRecStandardLetterCategoryID { get; set; }
        public virtual int StmpStandardLetterCategoryID { get; set; }
        public virtual bool EnableApplicationScanCheck { get; set; }
        public virtual int rptNewZealandCode { get; set; }
        public virtual string MailServerAddress { get; set; }
        public virtual string LDAPServerAddress { get; set; }
        public virtual int WorkshopProductTypeId { get; set; }
        public virtual int StampProductTypeId { get; set; }
        public virtual int IDCardProductTypeId { get; set; }
        public virtual int CertificateProductTypeId { get; set; }
    }
}
