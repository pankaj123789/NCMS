using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Dal.Portal
{
    public interface ISystemValuesTranslator
    {
        int EnglishLanguageId { get; }

        int AustraliaCountryId { get; }

        int NewZealandCountryId { get; }

        string MailServerAddress { get; }

        string CertificationDescriptorUrl { get; }

        string ComplaintPolicyUrl { get; }

        int PDListingProductId { get; }

        int PDListingOverseasProductId { get; }

        int OnlineOfficeId { get; }

        int CredentialRequestLimit { get; }

        int SharedOnlineUserId { get; }

        int OnlineEFTMachineId { get; }

        int OnlineOrderTypeId { get; }

        int RecognitionMethodId { get; }

        int TranslatorCategoryId { get; }

        int LanguageAideCategoryId { get; }

        int InterpreterCategoryId { get; }

        int AccreditationTypeId { get; }

        int RecognitionTypeId { get; }

        string WiisePaymentAccount { get; }

        string PdCatalogue { get; }

        int StampFeeProductRubberAustraliaId { get; }

        int StampFeeProductRubberOverseasId { get; }

        int StampFeeProductSelfInkingAustraliaId { get; }

        int StampFeeProductSelfInkingOverseasId { get; }

        int IdCardFeeProductAustraliaId { get; }

        int IdCardFeeProductOverseasId { get; }

        int CertificateFeeProductAustraliaId { get; }

        int CertificateFeeProductOverseasId { get; }

        int FormMFeeId { get; }

        int DefaultRecertificationForm { get; }

        bool DisablePayPalUi { get; }

        bool ThrowPayPalSystemError { get; }

        void RefreshAllCache();

        string ShowMessageOfTheDay { get; }

        string MyNaatiAvailable { get; }

        string MessageOfTheDay { get; }

        int MFAExpiryDays { get; }

        int MFACodeTimeoutSeconds { get; }

        string GmailWhitelistUrl { get; }

        int EmailAccessCodeValidityMinutes { get; }

        int MfaAndAccessCodeExpiryHours { get; }
    }
}