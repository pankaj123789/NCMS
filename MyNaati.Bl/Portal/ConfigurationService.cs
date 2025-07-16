using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Portal.Repositories;
using MyNaati.Contracts.Portal;

namespace MyNaati.Bl.Portal
{
   
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationRepository mConfigurationRepository;

        public ConfigurationService(IConfigurationRepository configurationRepository)
        {
            mConfigurationRepository = configurationRepository;
        }

        public bool GetShowVerifyCredentials()
        {
            SystemValue myNaatiSystemValue = mConfigurationRepository.GetSystemValueBykey("ShowVerifyCredentials");
            if (myNaatiSystemValue == null)
            {
                return false;
            }

            bool result;
            bool.TryParse(myNaatiSystemValue.Value, out result);

            return result;
        }

        public bool GetShowPhoto()
        {
            var systemValue = mConfigurationRepository.GetSystemValueBykey("ShowPhoto");
            if (systemValue == null)
            {
                return false;
            }

            bool result;
            bool.TryParse(systemValue.Value, out result);

            return result;
        }

        public int GetDaysToDelayAccreditation()
        {
            var systemValue = mConfigurationRepository.GetSystemValueBykey("DaysToDelayAccreditation");
            if (systemValue == null)
            {
                return 0;
            }

            int result;
            int.TryParse(systemValue.Value, out result);

            return result;
        }

        public int MinimumPasswordLength()
        {
            var systemValue = mConfigurationRepository.GetSystemValueBykey("MinimumPasswordLength");
            if (systemValue == null)
            {
                return 0;
            }

            int result;
            int.TryParse(systemValue.Value, out result);

            return result;
        }

        public int NumberPasswordsStore()
        {
            var systemValue = mConfigurationRepository.GetSystemValueBykey("NumberPasswordsStore");
            if (systemValue == null)
            {
                return 0;
            }

            int result;
            int.TryParse(systemValue.Value, out result);

            return result;
        }

        public int PasswordLockoutCount()
        {
            var systemValue = mConfigurationRepository.GetSystemValueBykey("PasswordLockoutCount");
            if (systemValue == null)
            {
                return 0;
            }

            int result;
            int.TryParse(systemValue.Value, out result);

            return result;
        }

        public int ExaminerExpiryDay()
        {
            var systemValue = mConfigurationRepository.GetSystemValueBykey("ExaminerExpiryDay");
            if (systemValue == null)
            {
                return 0;
            }

            int result;
            int.TryParse(systemValue.Value, out result);

            return result;
        }

        public int PractitionerExpiryDay()
        {
            var systemValue = mConfigurationRepository.GetSystemValueBykey("PractitionerExpiryDay");
            if (systemValue == null)
            {
                return 0;
            }

            int result;
            int.TryParse(systemValue.Value, out result);

            return result;
        }

        public int OtherExpiryDay()
        {
            var systemValue = mConfigurationRepository.GetSystemValueBykey("OtherExpiryDay");
            if (systemValue == null)
            {
                return 0;
            }

            int result;
            int.TryParse(systemValue.Value, out result);

            return result;
        }

        public bool GetPaymentRequiredForPDListing()
        {
            var systemValue = mConfigurationRepository.GetSystemValueBykey("PaymentRequiredForPDListing");
            if (systemValue == null)
            {
                return true;
            }

            bool result;
            bool.TryParse(systemValue.Value, out result);

            return result;
        }

        public void UpdateShowVerifyCredentials(bool newValue)
        {
            mConfigurationRepository.UpdateSystemValue("ShowVerifyCredentials", newValue.ToString());
        }

        public void UpdateDaysToDelayAccreditation(int newValue)
        {
            mConfigurationRepository.UpdateSystemValue("DaysToDelayAccreditation", newValue.ToString());
        }

        public void UpdateShowPhoto(bool newValue)
        {
            mConfigurationRepository.UpdateSystemValue("ShowPhoto", newValue.ToString());
        }

        public void UpdatePaymentRequiredForPDListing(bool newValue)
        {
            mConfigurationRepository.UpdateSystemValue("PaymentRequiredForPDListing", newValue.ToString());
        }

        public string GetSystemValue(string valueKey)
        {
            return mConfigurationRepository.GetSystemValueBykey(valueKey)?.Value;
        }
    }
}
