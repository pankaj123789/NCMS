using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Domain;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
    public interface IPhoneRepository : IRepository<Phone>
    {
        IList<Phone> FindPhonesEligibleForPDListing(int naatiNumber);
        IList<Phone> FindPhonesEligibleForPDWizard(int naatiNumber);
        void RemoveIsPreferred(int naatiNumber);
        void RemoveIncludeInPdFlag(int naatiNumber);
        IList<Phone> GetPhoneNumbers(int naatiNumber);
        /// <summary>
        /// Used in MyNaati Personal Details
        /// </summary>
        /// <param name="phoneRowId"></param>
        /// <param name="inlcudeInOd"></param>
        /// <returns></returns>
        bool SetOdPhoneVisibility(int phoneRowId, bool inlcudeInOd);
    }

    public class PhoneRepository : SecuredRepository<Phone>, IPhoneRepository
    {
        private ISystemValuesTranslator mSystemValuesTranslator;

        public PhoneRepository(ICustomSessionManager sessionManager, IDataSecurityProvider dataSecurityProvider, ISystemValuesTranslator systemValuesTranslator)
            : base(sessionManager, dataSecurityProvider)
        {
            mSystemValuesTranslator = systemValuesTranslator;
        }

        public IList<Phone> FindPhonesEligibleForPDListing(int naatiNumber)
        {
            var entityId = GetEntityId(naatiNumber);
            return Session.Query<Phone>()
                .Where(phone => phone.Entity.Id == entityId)
                .ToList();
        }

        private int? GetEntityId(int naatiNumber)
        {
            var result = Session.Query<Email>()
                .FirstOrDefault(x => x.Entity.NaatiNumber == naatiNumber)
                ?.Entity.Id;

            return result;
        }

        public IList<Phone> FindPhonesEligibleForPDWizard(int naatiNumber)
        {
            var entityId = GetEntityId(naatiNumber);
            return Session.Query<Phone>()
                .Where(phone =>phone.Entity.Id == entityId).ToList();
        }

        public void RemoveIsPreferred(int naatiNumber)
        {
            IList<Phone> phones = FindPhonesEligibleForPDWizard(naatiNumber);

            foreach (Phone phone in phones)
            {
                phone.PrimaryContact = false;
                SaveOrUpdate(phone);
            }
        }

        public void RemoveIncludeInPdFlag(int naatiNumber)
        {
            IList<Phone> phones = FindPhonesEligibleForPDWizard(naatiNumber);

            foreach (Phone phone in phones.Where(x=> x.IncludeInPD))
            {
                phone.IncludeInPD = false;
                SaveOrUpdate(phone);
            }
        }

        public IList<Phone> GetPhoneNumbers(int naatiNumber)
        {
            var entityId = GetEntityId(naatiNumber);
            return Session.Query<Phone>()
                .Where(phone => phone.Entity.Id == entityId)
                .ToList();
        }

        protected override string GetPrimaryEmailOfRecordOwner(Phone record)
        {
            return record.Entity.PrimaryEmail?.EmailAddress;
        }

        public bool SetOdPhoneVisibility(int phoneRowId, bool inlcudeInOd)
        {
            try
            {
                Phone phone = Get(phoneRowId);
                phone.IncludeInPD = inlcudeInOd;
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
