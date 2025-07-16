using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using MyNaati.Contracts.BackOffice.Language;

namespace MyNaati.Contracts.Portal
{
    public interface ILookupProvider
    {
        [Obsolete("Do not use, To be deleted")]
        IEnumerable<AccreditationCategory> AccreditationCategories { get; }
        IEnumerable<Language> Languages { get; }
        IEnumerable<LanguageLookup> CertificationCredentialLanguages1 { get; }
        IEnumerable<LanguageLookup> CertificationCredentialLanguages2 { get; }
        [Obsolete("Do not use, To be deleted")]
        IEnumerable<EoiLanguageTransferObject> EoiLanguages { get; }
        [Obsolete("Do not use, To be deleted")]
        IEnumerable<CredentialTypeLookup> CertificatesCredentialTypes { get; }
        [Obsolete("Do not use, To be deleted")]
        IEnumerable<State> States { get; }
        IEnumerable<Country> Countries { get; }
        IEnumerable<TestLocation> TestLocations { get; }
        IEnumerable<Postcode> Postcodes { get; }
        IEnumerable<PersonTitle> PersonTitles { get; }
        IEnumerable<OdAddressVisibilityTypeLookup> OdAddressVisibilityTypes { get; }
        [Obsolete("Do not use, To be deleted")]
        IEnumerable<Institution> Institutions { get; }
        [Obsolete("Do not use, To be deleted")]
        IEnumerable<Course> Courses { get; }
        ISystemValuesTranslator SystemValues { get; }

        void RefreshAllCache();
    }
}
