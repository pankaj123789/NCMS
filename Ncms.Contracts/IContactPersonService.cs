using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using Ncms.Contracts.Models;

namespace Ncms.Contracts
{
    public interface IContactPersonService
    {
        GenericResponse<IEnumerable<ContactPersonModel>> GetContactPersons(int institutionId);
        GenericResponse<bool> SetContactPersonInactive(int contactPersonId);
        GenericResponse<bool> InsertContactPerson(ContactPersonModel model);
        GenericResponse<bool> UpdateContactPerson(ContactPersonModel model);
        GenericResponse<IEnumerable<ContactPersonModel>> GetContactPersonsByNaatiNumber(int naatiNo);
        GenericResponse<ContactPersonModel> GetContactPersonsById(int contactPersonId);
    }

    public class ContactPersonModel
    {
        public int InstitutionId { get; set; }
     
        public string Name { get; set; }
      
        public string Email { get; set; }
      
        public string Phone { get; set; }
       
        public string PostalAddress { get; set; }
      
        public string Description { get; set; }
      
        public bool Inactive { get; set; }
        public int ContactPersonId { get; set; }
    }
  
}
