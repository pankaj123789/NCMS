using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class RolePlayer : EntityBase
    {
        public virtual Person Person { get; set; }
        public virtual int SessionLimit { get; set; }
		public virtual decimal? Rating { get; set; }
		public virtual bool Senior { get; set; }
		private IList<RolePlayerTestLocation> mLocations;
        private IList<RolePlayerLastAttendedTestSession> mLastAttendedTestSessions;
		public virtual IEnumerable<RolePlayerTestLocation> Locations => mLocations;
		private IList<TestSessionRolePlayer> mSessions;
		public virtual IEnumerable<TestSessionRolePlayer> Sessions => mSessions;
		public virtual IEnumerable<RolePlayerLastAttendedTestSession> LastAttendedTestSessions => mLastAttendedTestSessions;

		public RolePlayer()
		{
			mLocations = new List<RolePlayerTestLocation>();
			mSessions = new List<TestSessionRolePlayer>();
			mLastAttendedTestSessions = new List<RolePlayerLastAttendedTestSession>();
		}

		public virtual void AddLocation(RolePlayerTestLocation location)
		{
			mLocations.Add(location);
		}

		public virtual void RemoveLocation(RolePlayerTestLocation location)
		{
			mLocations.Remove(location);
		}
	}
}
