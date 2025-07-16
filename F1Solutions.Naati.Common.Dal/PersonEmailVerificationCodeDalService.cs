using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Dal
{
    public class PersonEmailVerificationCodeDalService : IPersonEmailVerificationCodeDalService
    {

        public PersonEmailVerificationCodeDalService()
        {

        }

        public GenericResponse<DateTime?> GetEmailCodeExpireStartDate(int naatiNumber)
        {
            var person = NHibernateSession.Current.Query<Person>().FirstOrDefault(x => x.Entity.NaatiNumber == naatiNumber);

            if (person == null)
            {
                return new GenericResponse<DateTime?>() { Success = false, Errors = new List<string>() { $"Person does not exist for {naatiNumber}." } };
            }

            return person.EmailCodeExpireStartDate;
        }

        public GenericResponse<int> GetLastEmailCode(int naatiNumber)
        {
            var person = NHibernateSession.Current.Query<Person>().FirstOrDefault(x => x.Entity.NaatiNumber == naatiNumber);

            if (person == null)
            {
                return new GenericResponse<int>() { Success = false, Errors = new List<string>() { $"Person does not exist for Id {naatiNumber}." } };
            }

            return person.LastEmailCode;
        }


        public GenericResponse<string> SetLastEmailCode(int naatiNumber, int emailCode)
        {
            var person = NHibernateSession.Current.Query<Person>().FirstOrDefault(x => x.Entity.NaatiNumber == naatiNumber);

            if (person == null)
            {
                return new GenericResponse<string> { Success = false, Errors = new List<string>() { $"Person does not exist for Id {naatiNumber}." } };
            }

            person.LastEmailCode = emailCode;
            person.EmailCodeExpireStartDate = DateTime.Now;

            NHibernateSession.Current.Flush();
            return person.PrimaryEmailAddress;
        }
    }
}