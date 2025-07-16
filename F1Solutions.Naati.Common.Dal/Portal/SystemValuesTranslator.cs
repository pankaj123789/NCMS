using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.SystemValues;

namespace F1Solutions.Naati.Common.Dal.Portal
{
   

    public class SystemValuesTranslator : ISystemValuesTranslator
    {
        private readonly ISystemValuesCacheQueryService _systemValuesCacheQueryService;


        public SystemValuesTranslator(ISystemValuesCacheQueryService systemValuesCacheQueryService)
        {
            _systemValuesCacheQueryService = systemValuesCacheQueryService;
        }
		
		public int EnglishLanguageId => GetInt("EnglishLanguageId");

        public int AustraliaCountryId => GetInt("DefaultCountryId");

        public int NewZealandCountryId => 153;

        public string MailServerAddress => _systemValuesCacheQueryService.GetSystemValue("MailServerAddress").Value;

		public string CertificationDescriptorUrl => _systemValuesCacheQueryService.GetSystemValue("CertificationDescriptorUrl").Value;

		public string ComplaintPolicyUrl => _systemValuesCacheQueryService.GetSystemValue("ComplaintPolicyUrl").Value;

		public int PDListingProductId => GetInt("PDListingProductId");

        public int PDListingOverseasProductId => GetInt("PDListingOverseasProductId");

        public int OnlineOfficeId => GetInt("OnlineOfficeId");

        public int CredentialRequestLimit => GetInt("CredentialRequestLimit");

        public int SharedOnlineUserId => GetInt("SharedOnlineUserId");

        public int OnlineEFTMachineId => GetInt("OnlineEFTMachineId");

        public int OnlineOrderTypeId => GetInt("OnlineOrderTypeId");

        public int RecognitionMethodId => GetInt("RecognitionMethodId");

        public int TranslatorCategoryId => GetInt("TranslatorCategoryId");

        public int LanguageAideCategoryId => GetInt("LanguageAideCategoryId");

        public int InterpreterCategoryId => GetInt("InterpreterCategoryId");

        public int AccreditationTypeId => GetInt("AccreditationTypeId");

        public int RecognitionTypeId => GetInt("RecognitionTypeId");

        public string WiisePaymentAccount => _systemValuesCacheQueryService.GetSystemValue("WiisePaymentAccount").Value;

        public string ShowMessageOfTheDay => _systemValuesCacheQueryService.GetSystemValue("ShowMessageOfTheDay").Value;

        public string MyNaatiAvailable => _systemValuesCacheQueryService.GetSystemValue("MyNaatiAvailable").Value;

        public string MessageOfTheDay => _systemValuesCacheQueryService.GetSystemValue("MessageOfTheDay").Value;
        public int MFAExpiryDays => GetInt("MFAExpiryDays");

        public int MFACodeTimeoutSeconds => GetInt("MFACodeTimeoutSeconds");

        public string GmailWhitelistUrl => _systemValuesCacheQueryService.GetSystemValue("GmailWhitelistUrl").Value;

        public int EmailAccessCodeValidityMinutes => GetInt("EmailAccessCodeValidityMinutes");

        public int MfaAndAccessCodeExpiryHours => GetInt("MfaAndAccessCodeExpiryHours");

        private int GetInt(string keyToFind)
        {
           return int.Parse(_systemValuesCacheQueryService.GetSystemValue(keyToFind).Value);
        }

        /// <summary>
        /// A value of 1 is true, all others are false
        /// </summary>
        /// <param name="keyToFind"></param>
        /// <returns></returns>
        private bool GetBool(string keyToFind,bool refresh = false)
        {
            return int.Parse(_systemValuesCacheQueryService.GetSystemValue(keyToFind,refresh).Value).Equals(1) ? true : false;

        }


        public string PdCatalogue => _systemValuesCacheQueryService.GetSystemValue("PdCatalogue").Value;

		#region Product Specification Ids

		public int StampFeeProductRubberAustraliaId => GetInt("StampFeeProductRubberAustraliaId");

        public int StampFeeProductRubberOverseasId => GetInt("StampFeeProductRubberOverseasId");

        public int StampFeeProductSelfInkingAustraliaId => GetInt("StampFeeProductSelfInkingAustraliaId");

        public int StampFeeProductSelfInkingOverseasId => GetInt("StampFeeProductSelfInkingOverseasId");

        public int IdCardFeeProductAustraliaId => GetInt("IdCardFeeProductAustraliaId");

        public int IdCardFeeProductOverseasId => GetInt("IdCardFeeProductOverseasId");

        public int CertificateFeeProductAustraliaId => GetInt("CertificateFeeProductAustraliaId");

        public int CertificateFeeProductOverseasId => GetInt("CertificateFeeProductOverseasId");

        public int FormMFeeId => GetInt("FormMFeeId");

        public int DefaultRecertificationForm => GetInt("DefaultRecertificationForm");

        #endregion Product Specification Ids

        public bool DisablePayPalUi => GetBool("DisablePayPalUi", true);

        public bool ThrowPayPalSystemError => GetBool("ThrowPayPalSystemError", true);

        public void RefreshAllCache()
        {
            _systemValuesCacheQueryService.RefreshAllCache();
        }
	}
}