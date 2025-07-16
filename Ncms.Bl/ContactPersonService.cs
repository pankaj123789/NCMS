using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using Ncms.Contracts;
using Ncms.Contracts.Models;

namespace Ncms.Bl
{
    public class ContactPersonService : IContactPersonService
    {
        private readonly IContactPersonQueryService _contactPersonQueryService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public ContactPersonService(IContactPersonQueryService contactPersonQueryService, IAutoMapperHelper autoMapperHelper)
        {
            _contactPersonQueryService = contactPersonQueryService;
            _autoMapperHelper = autoMapperHelper;
        }

        public GenericResponse<IEnumerable<ContactPersonModel>> GetContactPersons(int institutionId)
        {
            var list =
                _contactPersonQueryService.GetContactPersons(institutionId)
                    .Where(x => !x.Inactive)
                    .Select(ToContactPersonModel());

            return new GenericResponse<IEnumerable<ContactPersonModel>>(list);
        }

        public GenericResponse<IEnumerable<ContactPersonModel>> GetContactPersonsByNaatiNumber(int naatiNo)
        {
            var list =
                _contactPersonQueryService.GetContactPersonsByNaatiNumber(naatiNo)
                    .Where(x => !x.Inactive)
                    .Select(ToContactPersonModel());

            return new GenericResponse<IEnumerable<ContactPersonModel>>(list);
        }

        public GenericResponse<ContactPersonModel> GetContactPersonsById(int contactPersonId)
        {
            var contactPerson =
                _contactPersonQueryService.GetContactPersonsById(contactPersonId).Select(ToContactPersonModel()).FirstOrDefault();

            return contactPerson;
        }

        private static Func<ContactPersonDto, ContactPersonModel> ToContactPersonModel()
        {
            return x => new ContactPersonModel
            {
                ContactPersonId = x.Id,
                InstitutionId = x.InstitutionId,
                Description = x.Description,
                Email = x.Email,
                Name = x.Name,
                Phone = x.Phone,
                PostalAddress = x.PostalAddress
            };
        }

        public GenericResponse<bool> SetContactPersonInactive(int contactPersonId)
        {
            _contactPersonQueryService.SetContactPersonInactive(contactPersonId);
            return true;
        }

        public GenericResponse<bool> InsertContactPerson(ContactPersonModel model)
        {
            _contactPersonQueryService.InsertContactPerson(_autoMapperHelper.Mapper.Map<ContactPersonDto>(model));
            return true;
        }

        public GenericResponse<bool> UpdateContactPerson(ContactPersonModel model)
        {
            _contactPersonQueryService.UpdateContactPerson(new ContactPersonDto
            {
                Id = model.ContactPersonId,
                InstitutionId = model.InstitutionId,
                Description = model.Description,
                Email = model.Email,
                Name = model.Name,
                Phone = model.Phone,
                PostalAddress = model.PostalAddress
            });
            return true;
        }
    }
}
