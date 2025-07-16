using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.QueryServices
{
    
    public interface IContactPersonQueryService : IQueryService
    {
        
        IEnumerable<ContactPersonDto> GetContactPersons(int institutionId);

        
        void SetContactPersonInactive(int contactPersonId);

        
        void InsertContactPerson(ContactPersonDto contactPersonDto);

        
        void UpdateContactPerson(ContactPersonDto contactPersonDto);

        
        IEnumerable<ContactPersonDto> GetContactPersonsByNaatiNumber(int naatiNo);

        
        IEnumerable<ContactPersonDto> GetContactPersonsById(int contactPersonId);
    }
}