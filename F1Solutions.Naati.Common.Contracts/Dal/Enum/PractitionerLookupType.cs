using System.ComponentModel;

namespace F1Solutions.Naati.Common.Contracts.Dal.Enum
{
    public enum PractitionerLookupType
    {
        ActiveCredentialsLanguages1,
        ActiveCredentialsLanguages2,
        ActiveCredentialsTypesByRequest,
        ActiveCredentialsCountries,
        ActiveCredentialsStates
    }

    public enum ApiPublicPractitionerCountLookupType
    {
        [Description("ByCredentialTypeId")]
        ByCredentialTypeId = 1,
        [Description("ByCountryId")]
        ByCountryId = 2,
        [Description("ByStateId")]
        ByStateId = 3
    }

    public enum ApiPublicLookupType
    {
        Country = 1,
        State = 2,
        CredentialType = 3,
        Postcode = 4,
        TestLocation = 5
    }
}