using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using NHibernate.Linq;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
    public interface IAddressRepository : IRepository<Address>
    {
        IList<Address> FindAddressesEligibleForPDListing(int naatiNo);
        IList<Address> FindAllAddresses(int naatiNo);
        void RemoveIsPreferred(int naatiNo);
        void MarkAsInvalid(int addressId);
        bool IsPrimaryContact(int addressId);

        List<Address> GetAddresListOfEntity(int entityId);
        void RemoveIncludeInPdFlag(int naatiNo);

        bool IsDuplicateAddress(PersonalEditAddress personalEditAddress, int naatiNo);
        
        OdAddressVisibilityType GetOdAddressVisibilityType(int visibilityTypeId);

        /// <summary>
        /// Used to just set the OdAddressType (In the MyDetails summary screen)
        /// </summary>
        /// <param name="addressRowId"></param>
        /// <param name="odAddressVisibilityTypeId"></param>
        /// <returns></returns>
        bool SetOdAddressVisibilityTypeId(int addressRowId, int odAddressVisibilityTypeId);
    }

    public class AddressRepository : SecuredRepository<Address>, IAddressRepository
    {
        public AddressRepository(ICustomSessionManager sessionManager, IDataSecurityProvider dataSecurityProvider)
            : base(sessionManager, dataSecurityProvider)
        {
        }

        public IList<Address> FindAddressesEligibleForPDListing(int naatiNo)
        {
            var entityId = GetEntityId(naatiNo);

            return Session.Query<Address>()
                .Where(address => address.Entity.Id == entityId)
                .Where(address => address.Invalid == false)
                .Where(address => address.EndDate == null || address.EndDate >= DateTime.Today)
                .Fetch(x => x.Postcode)
                .ThenFetch(x => x.Suburb)
                .ToList();
        }
  
        public List<Address> GetAddresListOfEntity(int entityId)
        {
            return Session.Query<Address>()
                .Where(address => address.Entity.Id == entityId)
                .ToList();
        }

        public void RemoveIncludeInPdFlag(int naatiNo)
        {
            IList<Address> addresses = FindAllAddresses(naatiNo);
            var odAddressVisibilityType = GetOdAddressVisibilityType((int)OdAddressVisibilityTypeName.DoNotShow);

            foreach (Address address in addresses.Where(a => a.OdAddressVisibilityType.Id != (int)OdAddressVisibilityTypeName.DoNotShow))
            {
                address.OdAddressVisibilityType = odAddressVisibilityType;
                SaveOrUpdate(address);
            }
        }

        public void RemoveIsPreferred(int naatiNo)
        {
            IList<Address> addresses = FindAllAddresses(naatiNo);

            foreach (Address address in addresses.Where(a => a.PrimaryContact))
            {
                address.PrimaryContact = false;
                SaveOrUpdate(address);
            }
        }

        public bool IsDuplicateAddress(PersonalEditAddress editingAddress, int naatiNo)
        {
            var entityId = GetEntityId(naatiNo);
            return Session.Query<Address>()
                .Any(x => x.Entity.Id == entityId && x.Id != editingAddress.AddressId && x.Invalid == false
                          && x.StreetDetails == editingAddress.StreetDetails && x.Country.Id == editingAddress.CountryId
                          && (editingAddress.PostcodeId == 0 || x.Postcode.Id == editingAddress.PostcodeId));

        }

        private int? GetEntityId(int naatiNo)
        {
            var result = Session.Query<Person>()
                .FirstOrDefault(x => x.Entity.NaatiNumber == naatiNo)
                ?.Entity.Id;

            return result;
        }

        public void MarkAsInvalid(int addressId)
        {
            Address address = Get(addressId);
            if (address != null)
            {
                address.EndDate = DateTime.Today.AddDays(-1);
                address.Invalid = true;
                SaveOrUpdate(address);
            }
        }

        public bool IsPrimaryContact(int addressId)
        {
            Address address = Get(addressId);
            return (address != null && address.PrimaryContact);
        }

        protected override string GetPrimaryEmailOfRecordOwner(Address record)
        {
            return record.Entity.PrimaryEmail?.EmailAddress;
        }

        public IList<Address> FindAllAddresses(int naatiNo)
        {
            var entityId = GetEntityId(naatiNo);
            return Session.Query<Address>()
                .Where(address => address.Entity.Id == entityId)
                .Fetch(x => x.Postcode)
                .ThenFetch(x => x.Suburb)
                .ToList();
        }

        public OdAddressVisibilityType GetOdAddressVisibilityType(int visibilityTypeId)
        {
            return Session.Get<OdAddressVisibilityType>(visibilityTypeId);
        }

        public bool SetOdAddressVisibilityTypeId(int addressRowId, int odAddressVisibilityTypeId)
        {
            try
            {
                Address address = Get(addressRowId);

                var entityId = address.Entity.Id;

                var addressType = GetOdAddressVisibilityType(odAddressVisibilityTypeId);
                address.OdAddressVisibilityType = addressType;

                //only one address can be in online directory
                if(odAddressVisibilityTypeId > 1)
                {
                    var doNotUseAddressType = GetOdAddressVisibilityType(1);

                    var otherAddresses = Session.Query<Address>()
                   .Where(address => address.Entity.Id == entityId).ToList();
                    foreach(var otherAddress in otherAddresses)
                    {
                        if (otherAddress.Id != addressRowId)
                        {
                            otherAddress.OdAddressVisibilityType = doNotUseAddressType;
                        }
                    }
                }
                NHibernateSession.Current.Flush();
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}