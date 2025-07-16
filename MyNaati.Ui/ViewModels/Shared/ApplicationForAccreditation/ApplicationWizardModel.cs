using System;
using System.Collections.Generic;
using AutoMapper;
using F1Solutions.Global.Common.Mapping;
using MyNaati.Contracts.BackOffice.PersonalDetails;

namespace MyNaati.Ui.ViewModels.Shared.ApplicationForAccreditation
{
    public abstract class ApplicationWizardModel : BaseWizardModel
    {
        private readonly IAutoMapperHelper _autoMapperHelper;

        public ApplicationWizardModel(IAutoMapperHelper autoMapperHelper)
        {
            _autoMapperHelper = autoMapperHelper;
        }
        public int? RegisteredUserNaatiNumber { get; set; }
        public int ReferenceNumber { get; set; }
        public PersonalDetailsModel PersonalDetails { get; set; }
        public AddressModel Address { get; set; }
        public PhoneListModel PhoneList { get; set; }
        public EmailListModel EmailList { get; set; }
        public ResidencyStatusModel ResidencyStatus { get; set; }
        public PurposeOfCredentialsModel PurposeOfCredentials { get; set; }

        public void UpdateRegisteredUserInformation(PersonalDetailsGetExtendedInformationResponse extendedInformation)
        {
            Address = _autoMapperHelper.Mapper.Map<PersonalViewAddress, AddressModel>(extendedInformation.Address);
            PersonalDetails = _autoMapperHelper.Mapper.Map<PersonalDetailsExtended, PersonalDetailsModel>(extendedInformation.Person);
            PersonalDetails.FirstApplication = false;
            PhoneList.CurrentPhones = _autoMapperHelper.Mapper.Map<PersonalViewPhone[], IList<PhoneEditModel>>(extendedInformation.Phones);
            EmailList.CurrentEmails = _autoMapperHelper.Mapper.Map<PersonalViewEmail[], IList<EmailModel>>(extendedInformation.Emails);
        }

        public void UpdatePersonalDetails(PersonalDetailsModel personalDetails)
        {
            if (PersonalDetails == null)
                throw new NullReferenceException("PersonalDetails is null");
            PersonalDetails.UpdateFrom(personalDetails);
        }

        public void BuildPersonalDetailsLists()
        {
            if (PersonalDetails == null)
                throw new NullReferenceException("PersonalDetails is null");
            PersonalDetails.BuildLists();
        }

        public void UpdateAddressDetails(AddressModel address)
        {
            if (Address == null)
                throw new NullReferenceException("Address is null");
            Address = address;
        }

        public void BuildAddressLists()
        {
            if (Address == null)
                throw new NullReferenceException("Address is null");
        
        }

        public void BuildPhoneLists()
        {
            if (PhoneList == null)
                throw new NullReferenceException("PhoneList is null");
           
        }

        //return an error or nothing
        public string AddNewPhone(PhoneEditModel createModel)
        {
            if (PhoneList != null && PhoneList.CurrentPhones != null)
            {
                int phoneCount = PhoneList.CurrentPhones.Count;
                int emailCount = (EmailList == null || EmailList.CurrentEmails == null ? 0 : EmailList.CurrentEmails.Count);

                //if we have 5 already, then return an error
                if (phoneCount + emailCount >= 5)
                    return "A maximum of 5 phone numbers and email addresses can be provided.";

                PhoneList.CurrentPhones.Add(createModel);
                return string.Empty;
            }
            return "A system error occurred with the phone list.";
        }

        public void BuildEmailLists()
        {
            if (EmailList == null)
                throw new NullReferenceException("EmailList is null");
        }

        //return an error or nothing
        public string AddNewEmail(EmailModel createModel)
        {
            if (EmailList != null && EmailList.CurrentEmails != null)
            {
                int phoneCount = this.PhoneList == null || this.PhoneList.CurrentPhones == null ? 0 : this.PhoneList.CurrentPhones.Count;
                int emailCount = this.EmailList.CurrentEmails.Count;
                
                // assign the email an ID
                createModel.Id = emailCount;

                // if we have 5 already, then return an error
                if (phoneCount + emailCount >= 5)
                    return "A maximum of 5 phone numbers and email addresses can be provided.";

                var uniquenessError = this.EmailList.ValidateEmailUniqueness(createModel);
                if (!string.IsNullOrWhiteSpace(uniquenessError))
                {
                    return uniquenessError;
                }

                string preferredError = this.EmailList.EnsureOnlyOnePreferred(createModel);
                if (!string.IsNullOrEmpty(preferredError))
                {
                    return preferredError;
                }

                this.EmailList.CurrentEmails.Add(createModel);

                return null;
            }

            return "A system error occurred with the email list.";
        }

        public void UpdateResidencyStatus(ResidencyStatusModel residencyStatus)
        {
            if (ResidencyStatus == null)
                throw new NullReferenceException("ResidencyStatus is null");
            ResidencyStatus = residencyStatus;
        }

        public void BuildResidencyStatusLists()
        {
            if (ResidencyStatus == null)
                throw new NullReferenceException("ResidencyStatus is null");
            ResidencyStatus.BuildLists();
        }

        public void UpdatePurposeOfCredentials(PurposeOfCredentialsModel purposeOfCredentials)
        {
            if (PurposeOfCredentials == null)
                throw new NullReferenceException("PurposeOfCredentials is null");
            PurposeOfCredentials = purposeOfCredentials;
        }

        public void BuildPurposeOfCredentialsLists()
        {
            if (PurposeOfCredentials == null)
                throw new NullReferenceException("PurposeOfCredentials is null");
            PurposeOfCredentials.BuildLists();
        }

        public abstract void UpdateForCredentialRequests();

    }
}