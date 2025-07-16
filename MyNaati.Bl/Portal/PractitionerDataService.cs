using System;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using MyNaati.Contracts.BackOffice.AccreditationResults;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using MyNaati.Contracts.Portal;

namespace MyNaati.Bl.Portal
{
    public class PractitionerDataService
    {
        private readonly IPersonalDetailsService mPersonalDetailsService;
        private readonly IAccreditationResultService mAccreditationResultService;
        private readonly IConfigurationService mConfigurationService;

        private const string PRACTITIONER_NOT_FOUND = "Practitioner not found";
        private const string CREDENTIAL_NOT_FOUND = "No credentials found for this practitioner";
        private const string FEATURE_DISABLED = "NAATI is unable to display this practitioner's credentials. Please contact NAATI if you require verification";

        public PractitionerDataService(IPersonalDetailsService personalDetailsService,
            IAccreditationResultService accreditationResultService, IConfigurationService configurationService)
        {
            mPersonalDetailsService = personalDetailsService;
            mAccreditationResultService = accreditationResultService;
            mConfigurationService = configurationService;
        }

        public Practitioner GetPractitioner(string naatiNumber)
        {
            var request = new NaatiNumberRequest
            {
                NaatiNumber = Convert.ToInt32(naatiNumber)
            };

            var personResponse = mPersonalDetailsService.GetPersonByNaatiNo(request);
            if (personResponse.Person == null)
                throw new Exception(PRACTITIONER_NOT_FOUND);

            if (!personResponse.Person.AllowVerifyOnline)
                throw new Exception(FEATURE_DISABLED);

            var currentCredentials = mAccreditationResultService.GetCurrentCredentialsForPerson(request);
            var previousCredentials = mAccreditationResultService.GetPreviousCredentialsForPerson(request);

            if ((currentCredentials == null || !currentCredentials.Credentials.Any()) &&
                (previousCredentials == null || !previousCredentials.Credentials.Any()))
                throw new Exception(CREDENTIAL_NOT_FOUND);

            AddressDetail address = null;
            var addressesResponse = mPersonalDetailsService.GetAddresses(request);
            if (addressesResponse != null && addressesResponse.Addresses.Any(a => a.IsPreferred))
            {
                var preferredAddress = addressesResponse.Addresses.First(a => a.IsPreferred);
                address = new AddressDetail
                {
                    State = preferredAddress.State,
                    Country = preferredAddress.Country
                };
            }

            var skills = string.Join("<br />", currentCredentials.Credentials.Select(GetSkillString).ToArray());
            var expiredCredential = string.Join("<br />",
                previousCredentials.Credentials.Length > 0
                    ? previousCredentials.Credentials.Select(GetSkillString).ToArray()
                    : new[] { "Nil" });
            var showPhoto = personResponse.Person.ShowPhotoOnline && mConfigurationService.GetShowPhoto() && personResponse.Person.Photo != null;

            var practitioner = new Practitioner
            {
                Id = Convert.ToInt32(naatiNumber),
                PractitionerNumber = personResponse.Person.PractitionerNumber,
                GivenName = personResponse.Person.GivenName,
                Surname = personResponse.Person.Surname,
                Title = personResponse.Person.Title,
                Deceased = personResponse.Person.Deceased,
                Skills = skills,
                Address = address,
                ShowPhotoOnline = showPhoto,
                IsRevalidationScheme = mAccreditationResultService.IsRevalidationScheme(request),
                ExpiredCredentialSkills = expiredCredential,
                Photo = showPhoto ? personResponse.Person.Photo : null
            };

            return practitioner;
        }

        private string GetSkillString(Credential s)
        {
            var startDate = $"<br />Valid from: {s.StartDate:dd/MM/yyyy} <br />Valid to: ";
            var expiryDate = (s.ExpiryDate.HasValue ? $"{s.ExpiryDate.Value:dd/MM/yyyy}" : "Onwards ");
            return $"{s.Skill} ({s.Direction}){startDate}{expiryDate} ";
        }
    }
}