using F1Solutions.Naati.Common.Contracts.Bl;

namespace MyNaati.Contracts.Portal
{
    
    public interface IConfigurationService : IInterceptableservice
    {
        
        bool GetShowVerifyCredentials();

        
        void UpdateShowVerifyCredentials(bool newValue);

        
        int GetDaysToDelayAccreditation();

        
        void UpdateDaysToDelayAccreditation(int newValue);

        
        bool GetShowPhoto();

        
        int NumberPasswordsStore();

        
        int ExaminerExpiryDay();

        
        int PractitionerExpiryDay();

        
        int OtherExpiryDay();

        
        int PasswordLockoutCount();

        
        int MinimumPasswordLength();

        
        void UpdateShowPhoto(bool newValue);

        
        bool GetPaymentRequiredForPDListing();

        
        void UpdatePaymentRequiredForPDListing(bool newValue);

        string GetSystemValue(string valueKey);
    }
}