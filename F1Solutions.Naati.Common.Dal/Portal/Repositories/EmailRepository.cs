using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
    public interface IEmailRepository : IRepository<Email>
    {
        IList<Email> FindEmailsEligibleForPDListing(int naatiNumber);
        IList<Email> FindEmailsEligibleForPDWizard(int naatiNumber);
        Email GetPrimaryEmail(string email);

        IList<Email> ExistingEmails(string email);

        void RemoveIsPreferred(int naatiNumber);
        void RemoveIncludeInPdFlag(int naatiNumber);

        /// <summary>
        /// Used in MyNaati Personal Details
        /// </summary>
        /// <param name="emailRowId"></param>
        /// <param name="inlcudeInOd"></param>
        /// <returns></returns>
        bool SetOdEmailVisibility(int emailRowId, bool inlcudeInOd);
    }

    public class EmailRepository : SecuredRepository<Email>, IEmailRepository
    {
        public EmailRepository(ICustomSessionManager sessionManager, IDataSecurityProvider dataSecurityProvider)
            : base(sessionManager, dataSecurityProvider)
        {
        }

        public IList<Email> FindEmailsEligibleForPDListing(int naatiNumber)
        {
            var entityId = GetEntityId(naatiNumber);
            return Session.Query<Email>()
                .Where(email => email.Entity.Id == entityId)
                .ToList();
        }

        public IList<Email> FindEmailsEligibleForPDWizard(int naatiNumber)
        {
            var entityId = GetEntityId(naatiNumber);
            return Session.Query<Email>().Where(email => email.Entity.Id == entityId).ToList();
        }

        private int? GetEntityId(int naatiNumber)
        {
            var result = Session.Query<Email>()
                .FirstOrDefault(x => x.Entity.NaatiNumber == naatiNumber)
                ?.Entity.Id;

            return result;
        }

        public Email GetPrimaryEmail(string email)
        {
            return Session.Query<Email>()
                .SingleOrDefault(e => e.EmailAddress == email && !e.Invalid && e.IsPreferredEmail);
        }

        public IList<Email> ExistingEmails(string email)
        {
            return Session.Query<Email>().Where(e => e.EmailAddress == email && !e.Invalid).ToList();
        }

        public void RemoveIsPreferred(int naatiNumber)
        {
            IList<Email> emails = FindEmailsEligibleForPDListing(naatiNumber);

            foreach (Email email in emails)
            {
                email.IsPreferredEmail = false;
                SaveOrUpdate(email);
            }
        }

        public void RemoveIncludeInPdFlag(int naatiNumber)
        {
            IList<Email> emails = FindEmailsEligibleForPDListing(naatiNumber);

            foreach (Email email in emails.Where(x=>x.IncludeInPD))
            {
                email.IncludeInPD = false;
                SaveOrUpdate(email);
            }
        }

        protected override string GetPrimaryEmailOfRecordOwner(Email record)
        {
            return record.Entity.PrimaryEmail?.EmailAddress;
        }

        public bool SetOdEmailVisibility(int emailRowId, bool inlcudeInOd)
        {
            try
            {
                Email email = Get(emailRowId);
                email.IncludeInPD = inlcudeInOd;
                Session.Flush();
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
