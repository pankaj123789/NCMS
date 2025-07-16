using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
    public interface ICredentialRepository : IRepository<Credential>
    {
        IEnumerable<CredentialsDetailsDto> GetCurrentCertifiedCredentailsByEmail(int naatiNo);
        IEnumerable<CredentialsDetailsDto> GetPreviousCertifiedCredentailsByEmail(int naatiNo);
    }

    public class CredentialRepository : Repository<Credential>, ICredentialRepository
    {
        public CredentialRepository(ICustomSessionManager sessionManager)
            : base(sessionManager)
        {
        }

        public IEnumerable<CredentialsDetailsDto> GetCurrentCertifiedCredentailsByEmail(int naatiNo)
        {
            return GetAllCertifiedCredentailsByEmail(naatiNo).Where(x =>
                x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now.Date && (!x.TerminationDate.HasValue || x.TerminationDate.GetValueOrDefault().Date > DateTime.Now.Date)).ToList();
        }
        public IEnumerable<CredentialsDetailsDto> GetPreviousCertifiedCredentailsByEmail(int naatiNo)
        {
            return GetAllCertifiedCredentailsByEmail(naatiNo).Where(
                x =>
                    (x.EndDate < DateTime.Now.Date) ||
                    (x.TerminationDate != null && x.TerminationDate <= DateTime.Now.Date));
        }

        private IEnumerable<CredentialsDetailsDto> GetAllCertifiedCredentailsByEmail(int naatiNo)
        {
            return (from c in Session.Query<Credential>()
                    join ccr in Session.Query<CredentialCredentialRequest>() on c.Id equals ccr.Credential.Id
                    join cr in Session.Query<CredentialRequest>() on ccr.CredentialRequest.Id equals cr.Id
                    join ca in Session.Query<CredentialApplication>() on cr.CredentialApplication.Id equals ca.Id
                    join p in Session.Query<Person>() on ca.Person.Id equals p.Id
                    join s in Session.Query<Skill>() on cr.Skill.Id equals s.Id
                    join st in Session.Query<SkillType>() on s.SkillType.Id equals st.Id
                    join ct in Session.Query<CredentialType>() on cr.CredentialType.Id equals ct.Id
                    where ct.Certification && p.Entity.NaatiNumber == naatiNo
                    select new CredentialsDetailsDto
                    {
                        Id = c.Id,
                        EndDate = c.CertificationPeriod == null ? c.ExpiryDate : c.CertificationPeriod.EndDate,
                        StartDate = c.StartDate,
                        Language = cr.Skill.Language1.Name,
                        ToLanguage = cr.Skill.Language2.Name,
                        Direction =
                            s.DirectionType.DisplayName.Replace("[Language 1]", s.Language1.Name).Replace("[Language 2]", s.Language2.Name),
                        Skill = ct.ExternalName,
                        TerminationDate = c.TerminationDate,
                        IncludeOD = c.ShowInOnlineDirectory
                    }).ToList().GroupBy(x => x.Id).Select(x => x.First());
        }
    }
}
