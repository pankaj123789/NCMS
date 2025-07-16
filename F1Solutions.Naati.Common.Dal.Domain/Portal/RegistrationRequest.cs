using System;

namespace F1Solutions.Naati.Common.Dal.Domain.Portal
{
    public class RegistrationRequest : EntityBase
    {
        public virtual int NaatiNumber { get; set; }

        public virtual string GivenName { get; set; }
        public virtual string FamilyName { get; set; }
        public virtual DateTime DateOfBirth { get; set; }
        public virtual string EmailAddress { get; set; }
        public virtual DateTime DateRequested { get; set; }
        public virtual int? Gender { get; set; }
        public virtual string Title { get; set; }
    }
}
