//This will be auto load by bundle.config
/*jshint -W020 */
ENUMS = (function () {
    function getEnum(enumObj) {
        var list = [];

        for (var prop in enumObj) {
            list.push(prop);
        }

        return $.extend({}, enumObj, { list: list });
    }

    var enums = {
        AnswerTypes: {
            RadioOptions: 1,
            CheckOptions: 2,
            Input: 3,
            Date: 4,
            PersonVerification: 5,
            PersonDetails: 6,
            LanguageSelector: 7,
            CredentialSelector: 8,
            TestLocation: 9,
            ProductSelector: 10,
            DocumentUpload: 11,
            PersonPhoto: 12,
            CountrySelector: 13,
            Email: 14,
            TestSessions: 15,
            CredentialSelectorUpgradeAndSameLevel: 16,
            Fees: 17,
            RecertificationCredentialSelector: 18,
            TestDetails: 19,
            PaymentControl: 20,
            SupplementaryTestTasks: 21,
            EndorsedQualification: 19,
            EndorsedQualificationInstitution: 20,
            EndorsedQualificationLocation: 21,
            Information: 22
        },
        AnswerFunctionTypes: {
            Delete: 1,
            Redirect: 2,
            Submit: 3
        },
        OnlineForms: {
            CCL: 1,
            Certification: 2,
            CertificationPractitioner: 3,
            CCLV2: 4,
            EmptyCCLV2: 5,
            CLA: 6,
            Recertification: 7,
            AdvancedAndSpecialist: 8,
            PracticeTest: 9 
        },
        QuestionLogicTypes: {
            AnswerOption: 1,
            CredentialType: 2,
            CredentialRequestPathType: 3,
            PdPoints: 4,
            WorkPractice: 5,
            Skill: 6
        },
        WorkPracticeStatus: {
            Current: 1,
            Future: 2,
            Recertified: 3
        },
        PaymentMethods: {
            CreditCard: 1,
            DirectDeposit: 2,
            CashCheque: 3,
            PayPal: 4
        },
        CreditCardType: {
            Mastercard: 1,
            Visa: 2
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
        MaterialRequestPanelMembershipType: {
            Coordinator: 1,
            Collaborator: 2,
        },
        TestMaterialDomain: {
            Unspecified: 1
        },
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
})();
