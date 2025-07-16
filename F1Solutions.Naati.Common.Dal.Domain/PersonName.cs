using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class PersonName : EntityBase
    {
        public const string SurnameNotStated = "[Not Stated]";
        public virtual Person Person { get; protected internal set; }
        public virtual string GivenName { get; set; }
        public virtual string AlternativeGivenName { get; set; }
        public virtual string OtherNames { get; set; }
        public virtual string Surname { get; set; }
        public virtual string AlternativeSurname { get; set; }
        public virtual Title Title { get; set; }
        public virtual DateTime EffectiveDate { get; set; }

        public override IAuditObject RootAuditObject
        {
            get
            {
                return this.Person.RootAuditObject;
            }
        }

        public virtual void ChangePerson(Person person)
        {
            Person = person;
        }
    }
}