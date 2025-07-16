using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class 
        SystemValueScriptGenerator : BaseScriptGenerator
    {

        private readonly string _sendBriefInterval = "0 1 * * *";
        private readonly string _issueResultsInterval = "0 6 * * *";
        private readonly string _syncReportLogsInterval = "0 5 * * *";
        private readonly string _deleteTemporaryFilesInterval = "0 2 * * *";
        private readonly string _deletePodHistoryInterval = "0 22 1 * *";
        private readonly string _temporaryFilesRetentionHours = "1";
        private readonly string _maxPodHistoryRetentionDays = "30";
        private readonly string _useOfficeToSendEmails = "true";
        private readonly string _autoRecertificationWaitDays = "10";
        private readonly string _autoRecertificationMaxApplicationPercentage = "90";
        private readonly string _myNaatiEmailFromAddress = "noreply@naati.com.au";
        private readonly string _sendRecertificationReminderInterval = "12 7 * * *";
        private readonly string _sendTestSessionReminderInterval = "12 8 * * *";
        private readonly string _sendTestSessionAvailabilityNoticeInterval = "12 9 * * *";
        private readonly string _processNcmsRefundInterval = "23 8 * * *";
        private readonly string _refreshNcmsNotificationsInterval = "27 * * * *";
        private readonly string _downloadNcmsTestAssetsInterval = "20 18 * * *";
        private readonly string _deleteNcmsBankDetailsInterval = "38 23 * * *";
        //every day at 6:30pm
        private readonly string _testSittingsWithoutMaterialReminderInterval = "30 18 * * *";
        private readonly string _processFileDeletesPastExpiryDateInterval = "00 0,1,2,3,4,5,6,21,22,23 * * *";
        private readonly string _processFileDeletesHardDeleteInterval = "00 1 * * *";


        // private readonly string _createNcmsTelevicUsers = "20 6 * * *";



        public SystemValueScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
            if (ScriptRunner.CurrentEnvironment == ScriptSEnvironmentName.Test)
            {
                _useOfficeToSendEmails = "false";
                _sendBriefInterval = "3,18,33,48 * * * *";
                _issueResultsInterval = "5,35 * * * *";
                _autoRecertificationWaitDays = "0";
                _myNaatiEmailFromAddress = "test.noreply@altf4solutions.onmicrosoft.com";
                _temporaryFilesRetentionHours = "1";
                _maxPodHistoryRetentionDays = "1";
                _deletePodHistoryInterval = "0 18 * * *";
                _processNcmsRefundInterval = "15 * * * *";
            }
            if (ScriptRunner.CurrentEnvironment == ScriptSEnvironmentName.Dev)
            {
                _useOfficeToSendEmails = "false";
                _autoRecertificationWaitDays = "0";
                _autoRecertificationMaxApplicationPercentage = "30";
                _myNaatiEmailFromAddress = "test.noreply@altf4solutions.onmicrosoft.com";
                _syncReportLogsInterval = "0 5 * * *";
                _temporaryFilesRetentionHours = "1";
                _maxPodHistoryRetentionDays = "1";
                _deletePodHistoryInterval = "0 18 * * *";
                _processNcmsRefundInterval = "0 23 * * *";
            }

            if (ScriptRunner.CurrentEnvironment == ScriptSEnvironmentName.Uat)
            {
                _autoRecertificationWaitDays = "1";
                _myNaatiEmailFromAddress = "uat.noreply@altf4solutions.onmicrosoft.com";
                _temporaryFilesRetentionHours = "1";
                _maxPodHistoryRetentionDays = "2";
                _deletePodHistoryInterval = "0 18 * * *";
                _myNaatiEmailFromAddress = "uat.noreply@naati.com.au";
            }
        }

        public override string TableName => "tblSystemValue";
        public override IList<string> Columns => new[] {
                                                        "SystemValueId",
                                                        "ValueKey",
                                                        "Value",
                                                       };

        public override void RunScripts()
        {
            CreateTableRow(new[] { "1", "ProdDatabaseName", "NAATI" });
            CreateTableRow(new[] { "2", "IDCardPath", "G:\\IDCard Exports" });
            CreateTableRow(new[] { "3", "BaseReportPath", "SSRS Reports" });
            CreateTableRow(new[] { "4", "OfficialCredentialRecordName", "Official Credential Record" });
            CreateTableRow(new[] { "5", "CertificateFeeProductAustraliaId", "3520" });
            CreateTableRow(new[] { "6", "CertificateFeeProductOverseasId", "3521" });
            CreateTableRow(new[] { "7", "IdCardFeeProductAustraliaId", "3522" });
            CreateTableRow(new[] { "8", "IdCardFeeProductOverseasId", "3523" });
            CreateTableRow(new[] { "9", "StampFeeProductRubberAustraliaId", "2689" });
            CreateTableRow(new[] { "10", "StampFeeProductRubberOverseasId", "2690" });
            CreateTableRow(new[] { "11", "StampFeeProductSelfInkingAustraliaId", "3524" });
            CreateTableRow(new[] { "12", "StampFeeProductSelfInkingOverseasId", "3525" });
            CreateTableRow(new[] { "13", "PDListingOverseasProductId", "2620" });
            CreateTableRow(new[] { "14", "OnlineOfficeId", "316" });
            CreateTableRow(new[] { "15", "OnlineEFTMachineId", "212" });
            CreateTableRow(new[] { "16", "SharedOnlineUserId", "1965" });
            CreateTableRow(new[] { "17", "OnlineOrderTypeId", "3" });
            CreateTableRow(new[] { "18", "PostalContactTypeId", "3" });
            CreateTableRow(new[] { "19", "AccreditationTypeId", "1" });
            CreateTableRow(new[] { "20", "RecognitionTypeId", "2" });
            CreateTableRow(new[] { "21", "ProductOrderCorrespondenceCategoryId", "15" });
            CreateTableRow(new[] { "22", "OrderStandardLetterCategoryId", "6" });
            CreateTableRow(new[] { "23", "ScanContentTypeCategory", "SAM Content Types" });
            CreateTableRow(new[] { "24", "PDReminderFirstEmailLeadTimeInDays", "80" });
            CreateTableRow(new[] { "25", "PDReminderSecondEmailLeadTimeInDays", "80" });
            CreateTableRow(new[] { "26", "PDReminderFirstEmailTemplateId", "1" });
            CreateTableRow(new[] { "27", "PDReminderSecondEmailTemplateId", "1" });
            CreateTableRow(new[] { "28", "AutomatedCorrespondenceUserId", "2009" });
            CreateTableRow(new[] { "29", "DefaultCountryId", "13" });
            CreateTableRow(new[] { "30", "SendPDReminderEmails", "False" });
            CreateTableRow(new[] { "31", "SharePointServerURL", "http://nsp001:41000/SAMDocumentRepository" });
            CreateTableRow(new[] { "32", "ProcessingNoteCorrespondenceCategoryId", "1" });
            CreateTableRow(new[] { "33", "OfficialCredentialRecordCorrespondenceCategoryId", "16" });
            CreateTableRow(new[] { "34", "FormMFeeId", "3610" });
            CreateTableRow(new[] { "35", "SendSmsToOverseasNumbers", "False" });
            CreateTableRow(new[] { "36", "DefaultEmailSenderName", "NAATI" });
            CreateTableRow(new[] { "37", "DefaultEmailSenderAddress", "noreply@naati.com.au" });
            DeleteTableRow("39");
            DeleteTableRow("40");
            CreateTableRow(new[] { "41", "FinanceCorrespondenceCategoryId", "22" });
            CreateTableRow(new[] { "42", "SamDatabaseName", "NAATI" });
            CreateTableRow(new[] { "43", "ExaminerRoles", "1,4,307,612,622" });
            CreateTableRow(new[] { "47", "CertificationPeriodNextPeriod", "12" });
            CreateTableRow(new[] { "48", "CertificationPeriodDefaultDuration", "3" });
            CreateTableRow(new[] { "49", "CertificationPeriodRecertifyMonths", "3" });
            CreateTableRow(new[] { "50", "CertificationDescriptorUrl", "https://www.naati.com.au/certification/the-certification-system/" });
            CreateTableRow(new[] { "51", "ExaminerPanelTypes", "1" });
            //CreateTableRow(new[] { "52", "TestSessionBookingAvailability", "52" });
            //CreateTableRow(new[] { "53", "TestSessionBookingClosed", "1" });
            CreateTableRow(new[] { "54", "MicrosoftGraphAccessToken", "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" });
            CreateTableRow(new[] { "55", "ProcessingPendingAccountingOperations", "0" });
            CreateTableRow(new[] { "56", "ProcessingPendingApplications", "0" });
            CreateTableRow(new[] { "57", "MicrosoftGraphAppKey", "TUIw0sLP11rYCd1YGPobiH0W1vhUOSnfUef3HOn+7jhrrTyCBnb5qzfn3A/jafkcDEg6QcwcLuNQwVNxHVj9880xnZVq+YkAZrnOZKz7CpoC+0Q4+ZwbkPh9RsXjCbnXilVu+a1TViLPyMhRl2SlEvxYJD2i7txNyn3Y3a+xPlci7DTx" });
            CreateTableRow(new[] { "58", "ApplicationsCheckingInterval", "10,30,50 * * * *" });
            DeleteTableRow("59");
            ///MaxBatchSize set to 60 TFS 172415
            CreateOrUpdateTableRow(new[] { "60", "AccountingOperationsMaxBatchSize", "60" });
            CreateTableRow(new[] { "61", "AccountingOperationsMinWaitingMinutes", "2" });
            CreateTableRow(new[] { "62", "DisableBatchingFrom", "03:55" });
            CreateTableRow(new[] { "63", "DisableBatchingTo", "05:00" });
            CreateTableRow(new[] { "64", "RecertificationTotalPointsPerYear", "40" });
            CreateTableRow(new[] { "65", "PdCatalogue", "https://www.naati.com.au/wp-content/uploads/2020/01/Professional-Development-Catalogue.pdf" });
            CreateOrUpdateTableRow(new[] { "66", "ComplaintPolicyUrl", "https://www.naati.com.au/policies/" });
            CreateTableRow(new[] { "67", "PDTransitionStartDate", "2014-01-01T00:00:00" });
            CreateTableRow(new[] { "68", "CertificationPeriodRecertifyExpiry", "3" });
            //CreateTableRow(new[] { "69", "TestSessionBookingReject", "3" });
            CreateTableRow(new[] { "71", "DefaultRecertificationForm", "7" });
            CreateTableRow(new[] { "72", "CredentialRequestLimit", "6" });
            CreateTableRow(new[] { "73", "PaidTestReviewAvailableDays", "30" });
            CreateOrUpdateTableRow(new[] { "74", "UseOffice365ToSendEmail", _useOfficeToSendEmails });
            CreateTableRow(new[] { "75", "SendSuccessfulBatchReport", "false" });
            CreateTableRow(new[] { "76", "DatabaseUpdaterLastRun", String.Empty });
            CreateTableRow(new[] { "77", "SupplementaryTestAvailableDays", "30" });
            CreateOrUpdateTableRow(new[] { "78", "SendCandidateBriefsCheckingInterval", _sendBriefInterval });
            CreateOrUpdateTableRow(new[] { "79", "SendCandidateBriefs", "0" });
            CreateOrUpdateTableRow(new[] { "80", "IssueTestResultsAndCredentialsCheckingInterval", _issueResultsInterval });
            CreateOrUpdateTableRow(new[] { "81", "IssueTestResultsAndCredentials", "0" });
            CreateOrUpdateTableRow(new[] { "82", "ProcessPendingEmailsInterval", "*/10 * * * *" });
            CreateOrUpdateTableRow(new[] { "83", "ProcessPendingEmails", "0" });
            CreateOrUpdateTableRow(new[] { "84", "EmailBatchingSize", "500" });
            CreateOrUpdateTableRow(new[] { "85", "AutoReceritificationBatchPaymentWaitDays", _autoRecertificationWaitDays });
            CreateOrUpdateTableRow(new[] { "86", "AutoReceritificationMaxApplicationPercentage", _autoRecertificationMaxApplicationPercentage });
            CreateTableRow(new[] { "87", "PercentageOfCredentialIssued", "0" });
            CreateTableRow(new[] { "88", "EmailRetryLimitHours", "24" });
            CreateOrUpdateTableRow(new[] { "89", "AllowPaymentByVisa", "true" });
            CreateOrUpdateTableRow(new[] { "90", "AllowPaymentByMasterCard", "true" });
            CreateOrUpdateTableRow(new[] { "91", "AllowPaymentByAmex", "true" });
            CreateOrUpdateTableRow(new[] { "92", "AllowPaymentByDinersClub", "false" });
            CreateOrUpdateTableRow(new[] { "93", "AllowPaymentByJcb", "false" });
            CreateOrUpdateTableRow(new[] { "94", "ShowLastSubmittedExaminerMarkDays", "30" });
            CreateOrUpdateTableRow(new[] { "95", "PaidReviewTermsAndConditions", "https://www.naati.com.au/policies/terms-and-conditions/" });
            CreateOrUpdateTableRow(new[] { "96", "SelectTestTermsAndConditionsUrl", "https://www.naati.com.au/policies/terms-and-conditions/" });
            CreateOrUpdateTableRow(new[] { "97", "SupplementaryTestTermsAndConditions", "https://www.naati.com.au/policies/terms-and-conditions/" });
            CreateOrUpdateTableRow(new[] { "98", "MaterialRequestCoordinatorLoadingPercentage", "10.0" });
            CreateOrUpdateTableRow(new[] { "99", "PractitionerNumberBlackList", "0O,O0,5S,S5" });


            //From MyNaati
            CreateOrUpdateTableRow(new[] { "100", "ShowVerifyCredentials", "True" });
            CreateOrUpdateTableRow(new[] { "101", "DaysToDelayAccreditation", "7" });
            CreateOrUpdateTableRow(new[] { "102", "ShowPhoto", "True" });
            CreateOrUpdateTableRow(new[] { "103", "PaymentRequiredForPDListing", "False" });
            CreateOrUpdateTableRow(new[] { "104", "EmailFromAddress", _myNaatiEmailFromAddress });
            CreateOrUpdateTableRow(new[] { "105", "MinimumPasswordLength", "10" });
            CreateOrUpdateTableRow(new[] { "106", "NumberPasswordsStore", "24" });
            CreateOrUpdateTableRow(new[] { "107", "PasswordLockoutCount", "5" });
            CreateOrUpdateTableRow(new[] { "108", "ExaminerExpiryDay", "90" });
            CreateOrUpdateTableRow(new[] { "109", "PractitionerExpiryDay", "90" });
            CreateOrUpdateTableRow(new[] { "110", "OtherExpiryDay", "365" });
            CreateTableRow(new[] { "111", "BuildVersion", "" });
            CreateTableRow(new[] { "112", "RunMigrations", "0" });

            CreateTableRow(new[] { "113", "ProcessSyncReportLogsInterval", _syncReportLogsInterval });
            CreateOrUpdateTableRow(new[] { "114", "ExecuteNcmsReports", "0" });
            CreateOrUpdateTableRow(new[] { "115", "SyncNcmsReportLogs", "0" });
            CreateTableRow(new[] { "116", "MyNaatiApiPrivateKey", "0" });
            CreateTableRow(new[] { "117", "MyNaatiApiPublicKey", "0" });
            CreateTableRow(new[] { "118", "NcmsApiPublicKey", "0" });
            CreateTableRow(new[] { "119", "NcmsApiPrivateKey", "0" });
            CreateOrUpdateTableRow(new[] { "120", "MinDownloadKbRate", "500" });
            CreateOrUpdateTableRow(new[] { "121", "DeleteTemporaryFiles", "0" });
            CreateTableRow(new[] { "122", "DeleteTemporaryFilesInterval", _deleteTemporaryFilesInterval });
            CreateTableRow(new[] { "123", "TemporaryFilesRetentionHours", _temporaryFilesRetentionHours });
            CreateTableRow(new[] { "124", "MaxPodHistoryRetentionDays", _maxPodHistoryRetentionDays });
            CreateOrUpdateTableRow(new[] { "125", "DeletePodDataHistory", "0" });
            CreateTableRow(new[] { "126", "DeletePodDataHistoryInterval", _deletePodHistoryInterval });
            CreateOrUpdateTableRow(new[] { "127", "MaxPodHistoryLength", "10" });           
            CreateOrUpdateTableRow(new[] { "128", "MyNaatiRefreshSystemCacheInterval", "57 * * * *" });           
            CreateOrUpdateTableRow(new[] { "129", "MyNaatiRefreshSystemCache", "0" });
            CreateOrUpdateTableRow(new[] { "130", "MyNaatiRefreshCookieCacheInterval", "54 * * * *" });
            CreateOrUpdateTableRow(new[] { "131", "MyNaatiRefreshCookieCache", "0" });
            CreateOrUpdateTableRow(new[] { "132", "MyNaatiRefreshAllUsersCacheInterval", "52 * * * *" });
            CreateOrUpdateTableRow(new[] { "133", "MyNaatiRefreshAllUsersCache", "0" });
            CreateOrUpdateTableRow(new[] { "134", "RefreshNcmsCookieCacheInterval", "51 * * * *" });
            CreateOrUpdateTableRow(new[] { "135", "RefreshNcmsCookieCache", "0" });
            CreateOrUpdateTableRow(new[] { "136", "MyNaatiRefreshPendingUsersCache", "0" });
            CreateOrUpdateTableRow(new[] { "137", "RefreshNcmsPendingUsers", "0" });
            CreateOrUpdateTableRow(new[] { "138", "SendRecertificationReminderInterval", _sendRecertificationReminderInterval });
            CreateOrUpdateTableRow(new[] { "139", "SendRecertificationReminder", "0" });
            CreateOrUpdateTableRow(new[] { "140", "SendRecertificationReminderTimePeriodsDays", "14,90" });
            CreateOrUpdateTableRow(new[] { "141", "SendTestSessionReminderInterval", _sendTestSessionReminderInterval });
            CreateOrUpdateTableRow(new[] { "142", "SendTestSessionReminder", "0" });
            CreateOrUpdateTableRow(new[] { "143", "SendTestSessionReminderTimePeriodsDays", "10" });
            CreateOrUpdateTableRow(new[] { "144", "SendTestSessionAvailabilityNoticeInterval", _sendTestSessionAvailabilityNoticeInterval });
            CreateOrUpdateTableRow(new[] { "145", "SendTestSessionAvailabilityNotice", "0" });
            CreateOrUpdateTableRow(new[] { "146", "ProcessNcmsRefundInterval", _processNcmsRefundInterval });
            CreateOrUpdateTableRow(new[] { "147", "ProcessNcmsRefund", "0" });
            DeleteTableRow("148");
            DeleteTableRow("149");
            CreateOrUpdateTableRow(new[] { "150", "RefreshNcmsNotificationsInterval", _refreshNcmsNotificationsInterval });
            CreateOrUpdateTableRow(new[] { "151", "RefreshNcmsNotifications", "0" });

            CreateOrUpdateTableRow(new[] { "152", "Culture", "en-AU" });
            CreateOrUpdateTableRow(new[] { "153", "UICulture", "en-AU" });
            CreateTableRow(new[] { "154", "SecurePayAccessToken", "" });
            //CreateTableRow(new[] { "155", "TelevicAccessToken", "" });

            CreateOrUpdateTableRow(new[] { "156", "DownloadNcmsTestAssetsInterval", _downloadNcmsTestAssetsInterval });
            CreateOrUpdateTableRow(new[] { "157", "DownloadNcmsTestAssets", "" });
            CreateTableRow(new[] { "158", "SecurePayMigrationDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:00.000") });

            //CreateOrUpdateTableRow(new[] { "159", "DownloadForTelevicPageSize", "100" });
            //CreateOrUpdateTableRow(new[] { "160", "DownloadForTelevicRelativeStartTime", "-24" });
            //CreateOrUpdateTableRow(new[] { "161", "DownloadForTelevicRelativeEndTime", "-12" });
            //CreateOrUpdateTableRow(new[] { "161", "TelevicDownloadPageSize", "100" });
            CreateOrUpdateTableRow(new[] { "162", "RefreshNcmsAllUsersCache", "" });
            //CreateOrUpdateTableRow(new[] { "163", "CreateNcmsTelevicUsersInterval", _createNcmsTelevicUsers });
            //CreateOrUpdateTableRow(new[] { "164", "CreateNcmsTelevicUsers", "" });
            //CreateOrUpdateTableRow(new[] { "165", "CreateTelevicUserSessionHours", "240" });
            CreateOrUpdateTableRow(new[] { "166", "DonwnloadTestMaterialExpirationHours", "1" });
            CreateOrUpdateTableRow(new[] { "167", "DonwnloadTestMaterialErrorExpirationHours", "168" });
            CreateOrUpdateTableRow(new[] { "168", "DeleteNcmsBankDetails", "0" });
            CreateOrUpdateTableRow(new[] { "169", "DeleteNcmsBankDetailsInterval", _deleteNcmsBankDetailsInterval  });
            CreateOrUpdateTableRow(new[] { "170", "RefundBankDetailsFlushingDays", "90"  });
            CreateOrUpdateTableRow(new[] { "171", "DeleteNcmsBankDetailsBatchSize", "500"  });
            CreateOrUpdateTableRow(new[] { "172", "PayPalSurcharge", ".015" }); //1.5 percent
            CreateOrUpdateTableRow(new[] { "173", "PayPalGlCode", "44410" });
            CreateOrUpdateTableRow(new[] { "174", "DisablePayPalUi", "0" });
            CreateOrUpdateTableRow(new[] { "175", "ThrowPayPalSystemError", "0" });
            DeleteTableRow("176");
            DeleteTableRow("177");
            DeleteTableRow("178");
            CreateTableRow(new[] { "179", "WiisePaymentAccount", "f78c5fb3-1765-4be2-99e7-0e511a868e25" });
            CreateTableRow(new[] { "180", "WiiseAccessToken", "" });
            CreateTableRow(new[] { "181", "WiiseTenantId", "" });
            CreateTableRow(new[] { "182", "WiiseOperationsCheckingInterval", "1,20,40 * * * *" });

            CreateOrUpdateTableRow(new[] { "183", "AccountingCutOffDate", "2021-07-01" });
            CreateOrUpdateTableRow(new[] { "184", "RefundFormURL", "https://www.naati.com.au/wp-content/uploads/2020/01/Refund-Application-Form.pdf" });
            CreateTableRow(new[] { "185", "MessageOfTheDay", "" });
            CreateTableRow(new[] { "186", "ShowMessageOfTheDay", "false" });
            CreateTableRow(new[] { "187", "MyNaatiAvailable", "true" });
            DeleteTableRow("188");
            CreateOrUpdateTableRow(new[] { "189", "TestMaterialIncludedSkillTypes", "'Live Scenarios'" });
            CreateOrUpdateTableRow(new[] { "190", "TestSittingsWithoutMaterialReminderInterval", _testSittingsWithoutMaterialReminderInterval });

            //this one is for BackgroundTask JobToken
            CreateOrUpdateTableRow(new[] { "191", "SendTestSittingsWithoutMaterialReminder", "0" });
            CreateOrUpdateTableRow(new[] { "192", "TestSittingsWithoutMaterialReminderEmailAddresses" , "" });
            CreateOrUpdateTableRow(new[] { "193", "TestSittingsWithoutMaterialReminderDays", "14" });
            CreateOrUpdateTableRow(new[] { "194", "RolePlayerAvailable", "false" });
            CreateOrUpdateTableRow(new[] { "195", "MFAExpiryDays", "14" }); //Task 216233
            CreateOrUpdateTableRow(new[] { "196", "MFACodeTimeoutSeconds", "120" }); //divides by 60. Will be rounded down if not exact
            CreateTableRow(new[] { "197", "ProcessFileDeletesPastExpiryDateInterval", _processFileDeletesPastExpiryDateInterval });
            CreateTableRow(new[] { "198", "ProcessFileDeletesPastExpiryDate", "0" });
            CreateTableRow(new[] { "199", "ProcessFileDeletesPastExpiryDateRetentionDays", "14" });
            CreateOrUpdateTableRow(new[] { "200", "ProcessFileDeletesPastExpiryDateBatchSize", "1000" });
            CreateOrUpdateTableRow(new[] { "201", "ProcessFileDeletesPastExpiryDateStoredFileBasePath", @"C:\storage\FileStorage\" });
            CreateTableRow(new[] { "202", "ProcessFileDeletesPastExpiryDateIncludePreviouslyQueued", "0" });
            CreateTableRow(new[] { "203", "ProcessFileDeletesPastExpiryDateCreateStoredFiles", "0" });
            CreateTableRow(new[] { "204", "WiiseErrorsThatNeedToBeRetried", "The server committed a protocol violation|Sorry, we just updated this page" });
            CreateTableRow(new[] { "205", "ProcessFileDeletesHardDeleteInterval", _processFileDeletesHardDeleteInterval });
            CreateTableRow(new[] { "206", "ProcessFileDeletesHardDelete", "0" });
            CreateTableRow(new[] { "207", "TestResultExaminerRubricMarkingHistory_LastRun", "2000-01-01T00:00:00" }); //reset on every run so ony latest is processed
            CreateTableRow(new[] { "208", "TestResultExaminerStandardMarkingHistory_LastRun", "2000-01-01T00:00:00" }); //reset on every run so ony latest is processed
            CreateTableRow(new[] { "209", "GmailWhitelistUrl", "https://www.naati.com.au/adding-the-naati-domain-to-gmail-whitelist/" });
            CreateTableRow(new[] { "210", "MfaAndAccessCodeExpiryHours", "24" });
            CreateTableRow(new[] { "211", "EmailAccessCodeValidityMinutes", "15" });
        }

        public override void RunDescendantOrderScripts()
        {
            DeleteTableRow("38");
            DeleteTableRow("52");
            DeleteTableRow("53");
            DeleteTableRow("69");

        }
    }
}