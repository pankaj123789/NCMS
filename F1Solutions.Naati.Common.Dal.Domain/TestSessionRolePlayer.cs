using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestSessionRolePlayer : EntityBase
    {
        public virtual TestSession TestSession { get; set; }
        public virtual RolePlayer RolePlayer { get; set; }
        public virtual RolePlayerStatusType RolePlayerStatusType { get; set; }
		private IList<TestSessionRolePlayerDetail> mDetails;
		public virtual IEnumerable<TestSessionRolePlayerDetail> Details => mDetails;
		public virtual bool Rehearsed { get; set; }
        public virtual bool Attended { get; set; }
        public virtual bool Rejected { get; set; }
        public virtual DateTime StatusChangeDate { get; set; }
        public virtual User StatusChangeUser { get; set; }

		public TestSessionRolePlayer()
		{
			mDetails = new List<TestSessionRolePlayerDetail>();
		}
	}
}
