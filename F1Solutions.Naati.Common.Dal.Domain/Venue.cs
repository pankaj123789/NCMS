using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class Venue : EntityBase
    {

        private readonly IList<TestSession> mTestSessions = new List<TestSession>();
        public virtual TestLocation TestLocation { get; set; }
        public virtual string Address { get; set; }
        public virtual int Capacity { get; set; }
        public virtual string Name { get; set; }
        public virtual string PublicNotes { get; set; }
        public virtual string Coordinates { get; set; }
        public virtual bool Inactive { get; set; }
        public virtual User ModifiedUser { get; set; }
        public virtual bool ModifiedByNaati { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        protected override string AuditName => nameof(Venue);
        public override IAuditObject RootAuditObject => TestLocation;
        public virtual IEnumerable<TestSession> TestSessions => mTestSessions;

        public override string ToString()
        {
            return Name;
        }
    }
}
