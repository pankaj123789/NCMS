using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Dal.Portal.Repositories;
using MyNaati.Contracts.BackOffice.AccreditationResults;
using MyNaati.Contracts.BackOffice.PersonalDetails;
using Credential = MyNaati.Contracts.BackOffice.AccreditationResults.Credential;
using IAccreditationResultService = MyNaati.Contracts.BackOffice.AccreditationResults.IAccreditationResultService;

namespace MyNaati.Bl.BackOffice
{
    public class AccreditationResultService : IAccreditationResultService
    {
        private readonly IAccreditationResultRepository mAccreditationResultRepository;

        public AccreditationResultService(IAccreditationResultRepository accreditationResultRepository)
        {
            mAccreditationResultRepository = accreditationResultRepository;
        }

        public PersonCredentialsResponse GetCurrentCredentialsForPerson(NaatiNumberRequest request)
        {
            var credentials = mAccreditationResultRepository.GetCurrentCertifiedCredentailsByEmail(request.NaatiNumber);
            return new PersonCredentialsResponse() { Credentials = credentials.Select(ConvertToCredentialViewModel).ToArray() };
        }

        public PersonCredentialsResponse GetPreviousCredentialsForPerson(NaatiNumberRequest request)
        {
            var credentials = mAccreditationResultRepository.GetPreviousCertifiedCredentailsByEmail(request.NaatiNumber);
            return new PersonCredentialsResponse() { Credentials = credentials.Select(ConvertToCredentialViewModel).ToArray() };
        }

        private Credential ConvertToCredentialViewModel(CredentialsDetailsDto model)
        {
            return new Credential()
            {
                AccreditationResultId = model.Id,
                ToLanguage = model.ToLanguage,
                ExpiryDate = model.TerminationDate ?? model.EndDate,
                StartDate = model.StartDate,
                Direction = model.Direction,
                Skill = model.Skill,
                Language = model.Language
            };
        }

        public bool IsRevalidationScheme(NaatiNumberRequest request)
        {
            return mAccreditationResultRepository.IsRevalidationScheme(request.NaatiNumber);
        }
    }
}