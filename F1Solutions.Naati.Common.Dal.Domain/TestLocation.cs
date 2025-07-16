using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestLocation : EntityBase
    {
        private readonly IList<Venue> mVenues = new List<Venue>();
        private  IEnumerable<RolePlayerTestLocation> mRolePlayerTestLocations = Enumerable.Empty<RolePlayerTestLocation>();
        

        public virtual int OfficeId { get; set; }
        public virtual Office Office { get; set; }
        public virtual int CountryId { get; set; }
        public virtual string Name { get; set; }
        protected override string AuditName => nameof(TestLocation);
        public override IAuditObject RootAuditObject => Office.RootAuditObject;

        public virtual IEnumerable<Venue> Venues => mVenues;
        public virtual IEnumerable<RolePlayerTestLocation> RolePlayerTestLocations => mRolePlayerTestLocations;
        public override string ToString()
        {
            return Name;
        }
       // public virtual bool IsAutomated { get; set; }
    }
}

