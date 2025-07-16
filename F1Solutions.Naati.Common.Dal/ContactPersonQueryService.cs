using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;

namespace F1Solutions.Naati.Common.Dal
{
    
    public class ContactPersonQueryService : IContactPersonQueryService
    {
        
        public IEnumerable<ContactPersonDto> GetContactPersons(int institutionId)
        {
            return NHibernateSession.Current.Query<ContactPerson>()
                .Where(x => x.Institution.Id == institutionId).Select(ToContactPersonDto());
        }

        public IEnumerable<ContactPersonDto> GetContactPersonsByNaatiNumber(int naatiNo)
        {
            var institutionIds = from entity in NHibernateSession.Current.Query<NaatiEntity>()
                                 join institution in NHibernateSession.Current.Query<Institution>() on entity.Id equals institution.Entity.Id
                                 where entity.NaatiNumber == naatiNo
                                 select institution.Id;
            var id = institutionIds.FirstOrDefault();

            return NHibernateSession.Current.Query<ContactPerson>()
                .Where(x => x.Institution.Id == id).Select(ToContactPersonDto());
        }

        public IEnumerable<ContactPersonDto> GetContactPersonsById(int contactPersonId)
        {
            return
                NHibernateSession.Current.Query<ContactPerson>()
                    .Where(x => x.Id == contactPersonId)
                    .Select(ToContactPersonDto());

        }
        private static Expression<Func<ContactPerson, ContactPersonDto>> ToContactPersonDto()
        {
            return x => new ContactPersonDto
            {
                Id = x.Id,
                Name = x.Name,
                InstitutionId = x.Institution == null ? 0 : x.Institution.Id,
                Email = x.Email,
                Description = x.Description,
                Phone = x.Phone,
                PostalAddress = x.PostalAddress,
                Inactive = x.Inactive
            };
        }
        public void UpdateContactPerson(ContactPersonDto contactPersonDto)
        {
            var institution = NHibernateSession.Current.Get<Institution>(contactPersonDto.InstitutionId);
            if (institution == null)
            {
                throw new WebServiceException($"Institution not found (Id {contactPersonDto.InstitutionId})");
            }

            var contactPerson = NHibernateSession.Current.Get<ContactPerson>(contactPersonDto.Id);

            contactPerson.Institution = institution;
            contactPerson.Description = contactPersonDto.Description;
            contactPerson.Email = contactPersonDto.Email;
            contactPerson.Name = contactPersonDto.Name;
            contactPerson.Phone = contactPersonDto.Phone;
            contactPerson.PostalAddress = contactPersonDto.PostalAddress;
           

            NHibernateSession.Current.Update(contactPerson);
            NHibernateSession.Current.Flush();
        }

        public void InsertContactPerson (ContactPersonDto contactPersonDto)
        {
            var institution = NHibernateSession.Current.Get<Institution>(contactPersonDto.InstitutionId);
                if (institution == null)
                {
                    throw new WebServiceException($"Institution not found (Id {contactPersonDto.InstitutionId})");
                }

            var contactPerson = new ContactPerson
            {
                Institution = institution,
                Description = contactPersonDto.Description,
                Email = contactPersonDto.Email,
                Name = contactPersonDto.Name,
                Phone = contactPersonDto.Phone,
                PostalAddress = contactPersonDto.PostalAddress
            };

            NHibernateSession.Current.Save(contactPerson);
            NHibernateSession.Current.Flush();
        }

        public void SetContactPersonInactive(int contactPersonId)
        {
            var contactPerson = NHibernateSession.Current.Query<ContactPerson>()
                .SingleOrDefault(x => x.Id == contactPersonId);
            if (contactPerson != null) contactPerson.Inactive = true;

            NHibernateSession.Current.Save(contactPerson);
            NHibernateSession.Current.Flush();
        }
    }
}
