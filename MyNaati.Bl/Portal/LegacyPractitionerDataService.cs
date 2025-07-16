using System;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using MyNaati.Contracts.BackOffice.AccreditationResults;
using MyNaati.Contracts.BackOffice.Common;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using IAccreditationResultService = MyNaati.Contracts.BackOffice.Legacy.IAccreditationResultService;
using IPersonalDetailsService = MyNaati.Contracts.BackOffice.Legacy.IPersonalDetailsService;

namespace MyNaati.Bl.Portal
{
    public class LegacyPractitionerDataService
    {
        private readonly IPersonalDetailsService mPersonalDetailsService;
        private readonly IAccreditationResultService mAccreditationResultService;

        private const string PRACTITIONER_NOT_FOUND = "Practitioner not found";
        private const string CREDENTIAL_NOT_FOUND = "No credentials found for this practitioner";
        private const string FEATURE_DISABLED = "NAATI is unable to display this practitioner's credentials. Please contact NAATI if you require verification";

        public LegacyPractitionerDataService(IPersonalDetailsService legacyPersonalDetailsService,
            IAccreditationResultService legacyAccreditationResultService)
        {
            mPersonalDetailsService = legacyPersonalDetailsService;
            mAccreditationResultService = legacyAccreditationResultService;
        }

        public Practitioner GetPractitioner(string naatiNumber)
        {
            int daysToDelayAccreditation = 7; // from system value in ePortal prod
            var request = new PersonNaatiNumberRequest
            {
                NaatiNumber = Convert.ToInt32(naatiNumber),
                DaysToDelayAccreditation = daysToDelayAccreditation
            };

            PersonalDetailsGetPersonResponse personResponse = mPersonalDetailsService.GetPerson(request);
            if (personResponse.Person == null)
                throw new Exception(PRACTITIONER_NOT_FOUND);

            if (!personResponse.Person.AllowVerifyOnline)
                throw new Exception(FEATURE_DISABLED);

            PersonCredentialsResponse credentialsResponse = mAccreditationResultService.GetCredentialsForPersonIncludeExpiry(request, false);
            if (credentialsResponse == null || !credentialsResponse.Credentials.Any())
                throw new Exception(CREDENTIAL_NOT_FOUND);

            PersonalDetailsGetAddressesResponse addressesResponse = mPersonalDetailsService.GetAddresses(request);
            AddressDetail address = null;
            if (addressesResponse != null && addressesResponse.Addresses.Any(a => a.IsPreferred))
            {
                PersonalAddress preferredAddress = addressesResponse.Addresses.First(a => a.IsPreferred);
                address = new AddressDetail
                {
                    State = preferredAddress.State,
                    Country = preferredAddress.Country
                };
            }

            string[] skillsArray = credentialsResponse.Credentials.Where(x => x.ExpiryDate == null || x.ExpiryDate > DateTime.Now).Select(GetSkillString).ToArray();
            string skills = String.Join("<br />", skillsArray);

            Credential[] expiredCredentials = credentialsResponse.Credentials.Where(x => x.ExpiryDate != null && x.ExpiryDate < DateTime.Now).ToArray();

            string[] expiredCredentialArray = expiredCredentials.Select(GetExpiredCredentialSkillString).ToArray();
            string expiredCredential = String.Join("<br />", expiredCredentialArray);

            var practitioner = new Practitioner
            {
                Id = request.NaatiNumber,
                GivenName = personResponse.Person.GivenName,
                Surname = personResponse.Person.Surname,
                Title = personResponse.Person.Title,
                Deceased = personResponse.Person.Deceased,
                Skills = skills,
                Address = address,
                ShowPhotoOnline = false, // system value was false in ePortal prod
                IsRevalidationScheme = mAccreditationResultService.IsRevalidationScheme(request),
                ExpiredCredentialSkills = expiredCredential
            };

            return practitioner;
        }

        private string GetSkillString(Credential s)
        {
            string startDate = string.Format("<br />Valid from: {0}{1}", s.ResultDate.ToString("dd/MM/yyyy"), " Valid to: ");
            string expiryDate = (s.ExpiryDate.HasValue ? string.Format("{0}", s.ExpiryDate.Value.ToString("dd/MM/yyyy")) : " Onwards ");
            return string.Format("{0} ({1}){2}{3} ", s.Level, Credential.GetDirection(s), startDate, expiryDate);
        }

        private string GetExpiredCredentialSkillString(Credential s)
        {
            string startDate = string.Format("<br />Valid from: {0}{1}", s.ResultDate.ToString("dd/MM/yyyy"), " Valid to: ");
            string expiryDate = (s.ExpiryDate.HasValue ? string.Format("{0}", s.ExpiryDate.Value.ToString("dd/MM/yyyy")) : " Onwards ");
            return string.Format("{0} ({1}){2}{3} ", s.Level, Credential.GetDirection(s), startDate, expiryDate);
        }
    }
}