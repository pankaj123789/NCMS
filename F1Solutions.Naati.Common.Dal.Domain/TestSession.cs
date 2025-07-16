using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestSession : EntityBase
    {
        private IList<TestSitting> mTestSittings = new List<TestSitting>();
        private IList<TestSessionSkill> mTestSessionSkills = new List<TestSessionSkill>();
        private IEnumerable<TestSessionRolePlayer> mTestSessionRolePlayers = new List<TestSessionRolePlayer>();

        public virtual Venue Venue { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime TestDateTime { get; set; }
        public virtual int? ArrivalTime { get; set; }
        public virtual int? Duration { get; set; }
        public virtual CredentialType CredentialType { get; set; }
        public virtual bool Completed { get; set; }
        public virtual string PublicNote { get; set; }
        public virtual bool AllowSelfAssign { get; set; }
        public virtual IList<TestSitting> TestSittings => mTestSittings;
        public virtual IList<TestSessionSkill> TestSessionSkills => mTestSessionSkills;
		public virtual bool OverrideVenueCapacity { get; set; }
        public virtual bool? NewCandidatesOnly { get; set; }
        public virtual int? Capacity { get; set; }
		public virtual DateTime? RehearsalDateTime { get; set; }
		public virtual string RehearsalNotes { get; set; }
		public virtual IEnumerable<TestSessionRolePlayer> TestSessionRolePlayers => mTestSessionRolePlayers;
        public virtual bool AllowAvailabilityNotice { get; set; }
        public virtual TestSpecification DefaultTestSpecification { get; set; }
        public override IAuditObject RootAuditObject => this;
        public virtual bool IsActive { get; set; }
    }
}
