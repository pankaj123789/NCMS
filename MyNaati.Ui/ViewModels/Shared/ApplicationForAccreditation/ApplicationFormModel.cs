using System;
using System.Collections.Generic;
using System.Linq;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;

namespace MyNaati.Ui.ViewModels.Shared.ApplicationForAccreditation
{
    /// <summary>
    /// This model interprets and decorates the data on the source model as 
    /// necessary for our application form view to be as dumb as possible.
    /// </summary>
    public class ApplicationFormModel
    {
        public ApplicationFormModel(ApplicationWizardModel wizardModel, ILookupProvider lookupProvider)
        {
            PersonalDetails = new PersonalDetailsForPrintModel(wizardModel, lookupProvider);
        }

        public PersonalDetailsForPrintModel PersonalDetails { get; set; }
    }

    public class PersonalDetailsForPrintModel
    {
        private ApplicationWizardModel mWizardModel;
        private ILookupProvider mLookupProvider;

        public PersonalDetailsForPrintModel(ApplicationWizardModel wizardModel, ILookupProvider lookupProvider)
        {
            mWizardModel = wizardModel;
            mLookupProvider = lookupProvider;
        }

        public bool IsRegisteredUser
        {
            get
            {
                return mWizardModel.RegisteredUserNaatiNumber != null;
            }
        }

        public string NaatiNumber
        {
            get
            {
                return IsRegisteredUser ? mWizardModel.RegisteredUserNaatiNumber.Value.ToString() : string.Empty;
            }
        }

        public string IsFirstApplication
        {
            get
            {
                var val = mWizardModel.PersonalDetails.FirstApplication;
                if (val == null)
                    return "";

                return val.Value ? "Yes" : "No";
            }
        }

        public string FullName
        {
            get
            {
                return string.Format("{0} {1} {2}",
                    Title, GivenNames, FamilyName)
                    .TrimDuplicateSpaces()
                    .Trim();
            }
        }

        public string Title
        {
            get { return mLookupProvider.PersonTitles.Single(t => t.SamId == mWizardModel.PersonalDetails.TitleId).DisplayText; }
        }

        public string GivenNames
        {
            get { return (mWizardModel.PersonalDetails.GivenName + " " + mWizardModel.PersonalDetails.OtherNames).Trim(); }
        }

        public string AlternateName
        {
            get { return (mWizardModel.PersonalDetails.AlternativeGivenName + " " + mWizardModel.PersonalDetails.AlternativeFamilyName).Trim(); }
        }

        public string FamilyName
        {
            get { return mWizardModel.PersonalDetails.FamilyName; }
        }

        public string DateOfBirth
        {
            get { return mWizardModel.PersonalDetails.DateOfBirth.ToStringWithFormat("dd/MM/yyyy", ""); }
        }

        public string CountryOfBirth
        {
            get
            {
                return mWizardModel.PersonalDetails.CountryId.ToStringWithLookup(
              id => mLookupProvider.Countries.Single(c => c.SamId == id).DisplayText,
              "");
            }
        }

        public string AddressContent
        {
            get
            {
                if (mWizardModel.Address.IsAustralia)
                {
                    return string.Format("{0}{1}{2}",
                        mWizardModel.Address.StreetDetails,
                        Environment.NewLine,
                        mWizardModel.Address.SuburbName).TrimDuplicateSpaces();
                }

                return string.Format("{0}{1}{2}",
                    mWizardModel.Address.StreetDetails,
                    Environment.NewLine,
                    mWizardModel.Address.CountryName).TrimDuplicateSpaces();
            }
        }

        public IEnumerable<ContactDetail> PhoneNumbers
        {
            get
            {
                foreach (var phone in mWizardModel.PhoneList.CurrentPhones)
                {
                    yield return new ContactDetail
                    {
                      
                        Value = phone.FormattedNumber
                    };
                }
            }
        }

        public IEnumerable<ContactDetail> Emails
        {
            get
            {
                return mWizardModel.EmailList.CurrentEmails
                    .Select(e => new ContactDetail
                    {
                        Value = e.Email
                    });
            }
        }

        public class ContactDetail
        {
            public string Value { get; set; }
        }
    }
}
