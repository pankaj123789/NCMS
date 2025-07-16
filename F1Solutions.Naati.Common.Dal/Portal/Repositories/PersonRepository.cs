using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Domain.Extensions;

namespace F1Solutions.Naati.Common.Dal.Portal.Repositories
{
    public interface IPersonRepository : IRepository<Person>
    {
         Person FindByNaatiNumber(int naatiNumber);
        IEnumerable<Person> FindByNaatiNumber(IEnumerable<int> naatiNumbers);
        IEnumerable<Person> FindByNameAndBirthDate(string givenName, string familyName, DateTime BirthDate);
        Person FindByPractitionerNo(string practitionerNo);
        Person FindByPersonId(int personId);
    }

    public class PersonRepository : Repository<Person>, IPersonRepository
    {
        public PersonRepository(ICustomSessionManager sessionManager) : base(sessionManager)
        {
        }

        public Person FindByNaatiNumber(int naatiNumber)
        {
            //  Added the OrDefault because the registration process needs to be able to search
            //  for Customer numbers that do not exist.
            return Session.Query<Person>()
                .SingleOrDefault(p => p.Entity.NaatiNumber == naatiNumber);
        }

        public Person FindByPractitionerNo(string practitionerNo)
        {
            return Session.Query<Person>()
                .FirstOrDefault(
                    x =>
                        x.PractitionerNumber.ToUpper() == practitionerNo.ToUpper());
        }
        public Person FindByPersonId(int personId)
        {
            return Session.Query<Person>()
                .FirstOrDefault(
                    x =>
                        x.Id == personId);
        }

        public IEnumerable<Person> FindByNaatiNumber(IEnumerable<int> naatiNumbers)
        {
            if (naatiNumbers == null || !naatiNumbers.Any())
            {
                throw new ArgumentNullException("naatNumbers");
            }

            return Session.Query<Person>().Where(p => naatiNumbers.Contains(p.Entity.NaatiNumber)).ToList();
        }

        public IEnumerable<Person> FindByNameAndBirthDate(string givenName, string familyName, DateTime birthDate)
        {
            // used to check if there is a person record already in SAM with this name and DOB
            // make sure we have valid arguments
            if (givenName.IsNullOrEmpty() || familyName.IsNullOrEmpty() || birthDate.IsNull())
            {
                if (givenName.IsNullOrEmpty())
                {
                    throw new ArgumentException("givenName invalid");
                }

                if (familyName.IsNullOrEmpty())
                {
                    throw new ArgumentException("lastName invalid");
                }

                if (birthDate.IsNull())
                {
                    throw new ArgumentException("birthDate invalid");
                }
            }

            // filter by first name, last name, and date of birth
            var personResults = Session.Query<PersonName>().Where(pn =>
            pn.GivenName.ToLower() == givenName.ToLower() &&
            pn.Surname.ToLower() == familyName.ToLower() &&
            pn.Person.BirthDate.Value.Date == birthDate.Date
            ).ToList();

            List<int> ids = personResults.Select(pn => pn.Person.Id).Distinct().ToList();
            List<PersonName> personResponse = new List<PersonName>();
            ids.ForEach(i =>
            {
                personResponse.Add(personResults.First(p => p.Person.Id == i));
            });

            return personResponse.Select(p => p.Person);
        }
    }
}
