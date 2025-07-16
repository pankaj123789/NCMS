define([], function () {
    function getEnum(enumObj) {
        var list = [];

        for (var prop in enumObj) {
            list.push(prop);
        }

        return $.extend({}, enumObj, { list: list });
    }

    var enums = {
        TestStatus: {
            Sat: 'Sat',
            InProgress: 'InProgress',
            UnderReview: 'UnderReview',
            Finalised: 'Finalised',
            UnderPaidReview: 'UnderPaidReview'
        },
        MarkingSchemaType: {
            Standard: 1,
            Rubric: 2
        },
        TestResultType: {
            NotKnown: 1,
            Passed: 2,
            Failed: 4,
            Invalidated: 5
        },
        TestStatusType: {
            Sat: 1,
            InProgress: 2,
            UnderReview: 3,
            UnderPaidReview: 4,
            Finalised: 5,
            PendingSupplementaryTest: 6,
            TestInvalidated: 7
        },

        PayrollStatusType: {
            ClaimForm: 1,
            NotReceived: 2,
            Received: 3,
            Ready: 4,
            InProgress: 5,
            Complete: 6
        },
        ExaminerStatus: {
            InProgress: 'InProgress',
            Submitted: 'Submitted',
            Overdue: 'Overdue'
        },
        Directions: {
            ToEnglish: 'E',
            FromEnglish: 'O',
            Both: 'B'
        },
        DateRanges: {
            None: 0,
            Custom: 1,
            Today: 7,
            Yesterday: 2,
            LastWeek: 3,
            ThisWeek: 4,
            ThisMonth: 5,
            ThisYear: 6
        },
        DirectionTypes: {
            Language1IntoLanguage2: 1,
            Language2IntoLanguage1: 2,
            Language1AndLanguage2: 3,
            Language1: 4
        },
        ExaminerTypes: {
            Original: 'Original',
            ThirdExaminer: 'ThirdExaminer',
            PaidReviewer: 'PaidReviewer'
        },
        Genders: {
            Male: 'M',
            Female: 'F',
            Unspecified: 'X'
        },
        NaatiNumberType: {
            Candidate: 1,
            Other: 2,
            Institution: 3
        },
        PanelRole: {
            ExaminerContractChair: 1,
            RolePlayer: 2,
            ExaminerContract: 4,
            ExaminerInterim: 307,
            ExaminerCasual: 612
        },
        RequestStatus: {
            Approved: 'Approved',
            Received: 'Received',
            Sent: 'Sent'
        },
        PaymentTypes: {
            Cash: 'Cash',
            Cheque: 'Cheque',
            EFTPOS: 'EFTPOS',
            AMEX: 'AMEX',
            PayPal: 'PayPal'
        },
        SearchTypes: {
            Test: 'T',
            Panel: 'P',
            TestAsset: 'A',
            TestMaterial: 'M',
            EndOfPeriod: 'E',
            FinanceQueue: 'X',
            Person: 'R',
            Organisation: 'O',
            Application: 'L',
            CredentialRequestSummary: 'C',
            Calendar: 'N',
            TestSession: 'S',
            EmailTemplate: 'Z',
            Language: 'G',
            Skill: 'K',
            EmailQueue: 'Q',
            RubricConfiguration: 'B',
            EndorsedQualification: 'W',
            TestMaterialRequest: 'I'
        },
        AccountingOperationStatus: {
            Requested: 'Requested',
            InProgress: 'InProgress',
            Failed: 'Failed',
            Successful: 'Successful'
        },
        AccountingOperationStatusId: {
            Requested: 1,
            InProgress: 2,
            Failed: 3,
            Succeeded: 4
        },
        AccountingOperationType: {
            CreateInvoice: 'Invoice',
            CreatePayment: 'Payment',
            CreateContact: 'Contact',
            UpdateContact: 'UpdateContact',
        },
        AccountingOperationTypeId: {
            CreateInvoice: 1,
            CreatePayment: 2,
            CreateContact: 3,
            UpdateContact: 4,
        },
        ApplicationTypes: {
            Transition: 1,
            Certification: 2,
            CCL: 3,
            Prerequisite: 4
        },
        ApplicationStatusTypes: {
            Draft: 1,
            Entered: 2,
            BeingChecked: 3,
            Rejected: 4,
            InProgress: 5,
            Completed: 6,
            Deleted: 7,
            AwaitingAssessmentPayment: 8,
            AwaitingApplicationPayment: 9,
            ProcessingSubmission: 10,
            ProcessingApplicationInvoice:11
        },
        DocumentTypeCategories: {
            Tests: 1,
            Applications: 2,
            General: 3,
            TestMaterial: 4,
            TestSpecification: 5
        },
        PersonType: {
            Applicant: 'Applicant',
            Practitioner: 'Practitioner',
            FormerPractitioner: 'FormerPractitioner',
            FuturePractitioner: 'FuturePractitioner',
            Examiner: 'Examiner',
            Undefined: 'Undefined',
            RolePlayer: 'RolePlayer'

        },
        ApplicationFieldTypes: {
            YesNo: 1,
            Text: 2,
            Date: 3,
            CountryLookup: 4,
            Options: 5,
            Email: 6,
            EndorsedQualificationLookup: 7,
            EndorsedQualificationStartDate: 8,
            EndorsedQualificationEndDate: 9,
            EndorseInstitutionLookup: 10,
            EndorsedLocationLookup: 11,
            EndorsedQualificationIdText: 12,
            RadioOptions: 13
        },
        ApplicationWizardSteps: {
            SelectCredential: 1,
            Notes: 2,
            ViewInvoice: 3,
            IssueCredential: 4,
            DocumentsPreview: 5,
            EmailPreview: 6,
            DeleteConfirmation: 7,
            SupplementaryTest: 8,
            IssueConcededPass: 9,
            ExistingConcededCredential: 10,
            NotFoundConcededCredential: 11,
            CheckOption: 12,
            ConfigureRefund: 13,
            ApproveRefund: 14,
            ViewMessage: 15,
            PrerequisiteSummary: 16,
            IncompletePrerequisiteCredentials: 17,
            PrerequisiteApplications: 18,
            PrerequisiteMandatoryFields: 19,
            PrerequisiteMandatoryDocumentTypes: 20,
            PrerequisiteConfirmApplicationCreation: 21,
            NoNeedToContinue: 22,
            PrerequisiteExemptions : 23,
            PrerequisiteIssueOnHoldCredentials: 24,
            ComposeEmail: 25
        },
        ApplicationWizardActions: {
            // Application actions
            Create: 1,
            NewCredentialRequest: 2,
            Submit: 3,
            StartChecking: 4,
            CancelChecking: 5,
            Reject: 6,
            FinishChecking: 7,
            Finalise: 8,
            DeleteApplication: 9,
            AssessmentInvoicePaid: 10,
            SendEmail: 23,

            // Credential Request actions
            StartAssesing: 1001,
            CancelAssessment: 1002,
            PassAssessment: 1003,
            PendAssessment: 1004,
            FailAssessment: 1005,
            FailPendingAssessment: 1006,
            CreatePaidReview: 1007,
            PassReview: 1008,
            FailReview: 1009,
            IssueCredential: 1010,
            CancelRequest: 1011,
            DeleteRequest: 1012,
            ReissueCredential: 1013,
            TestInvoicePaid: 1015,
            WithdrawRequest: 1016,
            CancelTestInvitation: 1039,
            AllocateTestSession: 1017,
            RejectTestSessionFromMyNaati: 1020,
            RejectTestSession: 1021,
            CheckIn: 1022,
            MarkAsSat: 1023,
            UndoCheckIn: 1024,
            UndoMarkAsSat: 1025,
            IssueFail: 1026,
            IssuePass: 1027,
            CreatePaidTestReview: 1028,
            IssuePaidTestReviewFail: 1029,
            IssuePaidTestReviewPass: 1030,
            IssueSupplementaryResult: 1031,
            IssueSupplementaryPaidReviewResult: 1032,
            CreateSupplementaryTest: 1033,
            WithdrawSupplementaryTest: 1034,
            SupplementaryTestInvoicePaid: 1035,
            AllocateTestSessionFromMyNaati: 1036,
            RequestRefund: 1055,
            CreditNoteProcessed: 1056,
            CreditNotePaid: 1057,
            ProcessRefund: 1058,
            ApproveRefund: 1059,
            RejectRefund: 1060,
            CreatePrerequisiteApplictions: 1063,
            IssuePracticeTestResults: 1065
        },
        EntitySearchType: {
            Person: 1,
            Institution: 2
        },
        CredentialRequestStatusTypes: {
            Draft: 1,
            Rejected: 2,
            RequestEntered: 3,
            ReadyForAssessment: 4,
            BeingAssessed: 5,
            Pending: 6,
            AssessmentFailed: 7,
            AssessmentSuccessful: 8,
            AssessmentPaidReview: 9,
            EligibleForTesting: 10,
            TestFailed: 11,
            CertificationIssued: 12,
            Canceled: 13,
            Deleted: 14,
            AwaitingTestPayment: 15,
            Withdrawn: 16,
            TestAccepted: 17,
            TestSessionAccepted: 19,
            CheckIn: 20,
            TestSat: 21,
            IssuedPassResult: 22,
            UnderPaidTestReview: 23,
            AwaitingSupplementaryTestPayment: 24,
            AwaitingApplicationPayment: 25,
            ProcessingSubmission: 26,
            ProcessingPaidReviewInvoice: 27,
            AwaitingPaidReviewPayment: 28,
            SupplementaryTestInvoiceProcessed: 29,
            TestInvalidated: 30,
            ToBeIssued: 31,
            ProcessingTestInvoice: 32,
            ProcessingRequest: 33,
            RefundRequested: 34,
            ProcessingCreditNote: 35,
            AwaitingCreditNotePayment: 36,
            RefundRequestApproved: 37,
            RefundFailed: 38,
            CertificationOnHold: 39,
            IssuePracticeTestResults: 40,
            OnHoldToBeIssued: 41,

        },
        PersonTypeId: {
            Undefined: 0,
            Applicant: 1,
            Practitioner: 2,
            FormerPractitioner: 3,
            Examiner: 4,
        },
        CertificationPeriodStatus: {
            None: 0,
            Expired: 1,
            Current: 2,
            Future: 3
        },
        CredentialStatus: {
            Unknown: 1,
            Teminated: 2,
            Expired: 3,
            Active: 4,
            Future: 5
        },
        CredentialRequestWizardSteps: {
            TestSession: 1,
            ExistingApplicants: 2,
            NewApplicants: 3,
            Notes: 4,
            PreviewInvoice: 5,
            ViewEmailAttachments: 6,
            CheckOption: 7,
            ViewInvoice: 8,
            ViewMessage: 9
        },
        TestSessionWizardSteps: {
            Details: 1,
            Skills: 2,
            MatchingApplicants: 3,
            Notes: 4,
            PreviewEmail: 5,
            CheckOption: 6,
            RehearsalDetails: 7
        },
        TestComponentBaseType: {
            Language: 1,
            Skill: 2
        },
        MarkingResultType: {
            Original: 1,
            EligableForSupplementary: 2,
            FromOriginal: 3
        },
        OdAddressVisibilityType: {
            DoNotShow: 1,
            StateOnly: 2,
            StateAndSuburb: 3,
            FullAddress: 4
        },
        NoteReferenceType: {
            None: 1,
            Application: 2,
            Test: 3
        },
        TestMaterialWizardSteps: {
            SupplementaryTestApplicants: 1,
            TestSpecification: 2,
            Skills: 3,
            TestMaterials: 4,
            Applicants: 5,
            ExaminersAndRolePlayers: 6
        },
        TestMaterialStatusType: {
            New: 1,
            ToBeUsed: 2,
            PreviouslyUsed: 3,
            UsedByApplicants: 4
        },
        ApplicantsRangeType: {
            Undefined: 0,
            None: 1,
            One: 2,
            TwoAndFive: 3,
            MoreThanFive: 4,
        },
        RecertificationStatusType: {
            EligibleForNew: 1,
            EligibleForExisting: 2,
            BeingAssessed: 3,
            Failed: 4
        },
        CredentialApplicationTypeCategory: {
            None: 1,
            Transition: 2,
            Recertification: 3
        },
        AllocateRolePlayersSteps: {
            TestSpecification: 1,
            Skill: 2,
            AllocateRolePlayers: 3,
            EmailPreview: 4,
            Notes: 5,
            SendEmail: 6
        },
        RolePlayerRoleType: {
            PrimaryRolePlayer: 1,
            SecondaryRolePlayer: 2,
            PracticeInterpreter: 3
        },
        RolePlayerStatusType: {
            Pending: 1,
            Accepted: 2,
            Rejected: 3,
            Rehearsed: 4,
            Attended: 5,
            NoShow: 6
        },
        EmailStatuses: {
            Requested: 'Requested',
            InProgress: 'InProgress',
            Failed: 'Failed',
            Successful: 'Successful'
        },
        TestResultEligibilityType: {
            Pass: 1,
            ConcededPass: 2,
            SupplementaryTest: 3
        },
        SecNoun: {
            Application: 1,
            CertificationPeriod: 2,
            Credential: 3,
            CredentialRequest: 4,
            Document: 5,
            Email: 6,
            EmailTemplate: 7,
            EndorsedQualification: 8,
            Examiner: 9,
            ExaminerMarks: 10,
            ExaminerPayment: 11,
            FinanceOther: 12,
            Invoice: 13,
            Language: 14,
            Logbook: 15,
            MaterialRequest: 16,
            Organisation: 17,
            PaidReview: 18,
            Panel: 19,
            Payment: 20,
            PayRun: 21,
            Person: 22,
            PersonFinanceDetails: 23,
            PersonMyNaatiRegistration: 24,
            RolePlayer: 25,
            RubricResult: 26,
            Skill: 27,
            SupplementaryTest: 28,
            System: 29,
            TestAsset: 30,
            TestMaterial: 31,
            TestResult: 32,
            TestSession: 33,
            TestSitting: 34,
            TestSpecification: 35,
            User: 36,
            Venue: 37,
            Entity: 38,
            Contact: 39,
            Audit: 40,
            General: 41,
            Notes: 42,
            Bill: 43,
            PersonHistory: 44,
            OrganisationHistory: 45,
            PanelMember: 46,
            Dashboard: 47,
            ApiAdministrator: 48
        },
        SecVerb: {
            Search: 0x1,
            Read: 0x2,
            Update: 0x4,
            Create: 0x8,
            Delete: 0x10,
            Manage: 0x20,
            Reactivate: 0x40,
            Upload: 0x80,
            Download: 0x100,
            Override: 0x200,
            Revert: 0x400,
            Validate: 0x800,
            Send: 0x1000,
            Assign: 0x2000,
            Issue: 0x4000,
            Finalise: 0x8000,
            Reject: 0x10000,
            Close: 0x20000,
            Invalidate: 0x40000,
            Cancel: 0x80000,
            Invite: 0x100000,
            Uninvite: 0x200000,
            Withdraw: 0x400000,
            Approve: 0x800000,
            Notify: 0x1000000,
            Notes: 0x2000000,
            Extend: 0x4000000,
            PreviewEmail: 0x8000000,
            MarKAsPaid: 0x10000000,
            Assess: 0x20000000,
            Configure: 0x40000000,
            RequestRefund: 0x80000000,
            ApproveRefund: 0x100000000,
            RejectRefund: 0x200000000,
            ProcessRefund: 0x400000000,
            AssignPastSession: 0x1000000000
        },
        MaterialRequestWizardActions: {
            CreateMaterialRequest: 3000,
            CloneMaterialRequest: 3001,
            CreateMaterialRequestRound: 3002,
            UploadFinalMaterialDocuments: 3003,
            RevertMaterialRequest: 3004,
            UpdateMaterialRequest: 3005,
            CancelMaterialRequest: 3006,
            UpdateMaterialRequestMembers: 3010,
            SubmitRoundForApproval: 4000,
            ApproveMaterialRequestRound: 4001,
            RejectMaterialRequestRound: 4002,
            RevertMaterialRequestRound: 4003,
            UpdateMaterialRequestRound: 4004
        },
        MaterialRequestWizardStep: {
            TestMaterial: 1,
            TestMaterialSource: 2,
            RoundDetails: 3,
            DocumentsUpload: 4,
            Members: 5,
            Notes: 6,
            SendEmailCheckOption: 7,
            EmailPreview: 8,
            ExistingDocuments: 9,
            Coordinator: 10,
            RoundLinks: 11,
            Panel: 12

        },
        MaterialRoundMembmershipType: {
            Coordinator: 1,
            Collaborator: 2,
        },
        MaterialRequestRoundStatusType: {
            SentForDevelopment: 1,
            AwaitingAproval: 2,
            Rejected: 3,
            Approved: 4,
            Cancelled: 5
        },
        MaterialRequestStatusType: {
            InProgress: 1,
            AwaitingFinalisation: 2,
            Finalised: 3,
            Cancelled: 4
        },
        IssueCredentialStatusType: {
            Match: 1,
            Unmatch: 2
        },
        TestMaterialDomain: {
            Undefined: 1
        },
        DocumentType: {
            Undefined: 1,
            UnmarkedTestAsset: 2,
            MarkedTestAsset: 3,
            EnglishMarking: 4,
            TestMaterial: 5,
            ReviewReport: 6,
            GeneralTestDocument: 7,
            General: 8,
            Identification: 9,
            WorkPracticeEvidence: 10,
            Transcript: 11,
            TrainingEvidence: 12,
            AuslanProficiencyEvidence: 13,
            EnglishProficiencyEvidence: 14,
            EthicalCompetencyEvidence: 15,
            InterculturalCompetencyEvidence: 16,
            ApplicationsOther: 17,
            ChuchotageEvidence: 18,
            CertificateTemplate: 19,
            Certificate: 20,
            ApplicationForm: 21,
            CredentialLetterTemplate: 22,
            CredentialLetter: 23,
            PurchaseOrder: 24,
            AiicMembershipEvidence: 25,
            AitcMembershipEvidence: 26,
            WorkPractice: 27,
            ProfessionalDevelopmentActivity: 28,
            Invoice: 29,
            CandidateTestMaterial: 30,
            ExaminerTestMaterial: 31,
            ProfessionalDevelopmentEvidence: 32,
            CandidateCoverSheet: 33,
            CandidateInformation: 34,
            Script: 35,
            ProblemSheet: 36,
            MedicalCertificate: 37,
            NaatiEmployment: 38,
            PersonOther: 39,
            CandidateBrief: 40,
            TranscriptOrProofOfEnrolment: 41,
            SourceTemplate: 42,
            BlankTemplate: 43,
            MaterialRequestChecklist: 44,
            Guideline: 45,
            DraftDocument: 46,
            MaterialRequestSubmissionChecklist: 47
        },
        RefundMethodTypeName: {
            Undefined: 1,
            CreditCard: 2,
            DirectDeposit: 3
        },
        RefundCalculationResultType: {
            NotCalculated : 0,
            RefundAvailable : 1,
            NoRefund :2
        },
        NotificationType: {
            DownloadTestMaterial: 1,
            ErrorMessage: 2
        },
        TestMaterialTypeName: {
            Test: 1,
            Source: 2
        }
    };

    var result = {
        getName: function (enumType, enumValue) {
            var enumObj = enums[enumType];

            for (var prop in enumObj) {
                if (enumObj[prop] === enumValue) {
                    return prop;
                }
            }

            return null;
        }
    };

    for (var e in enums) {
        result[e] = getEnum(enums[e]);
    }

    return result;
});
