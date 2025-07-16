using System.Collections.Generic;

namespace Ncms.Contracts.Models.TestSpecification
{
    public class CredentialType : BaseModelClass
    {
        public CredentialType()
        {
            TestSpecifications = new List<TestSpecification>();
        }
        public string InternalName { get; set; }
        public string ExternalName { get; set; }
        public int DisplayOrder { get; set; }
        public bool Simultaneous { get; set; }
        public int SkillTypeId { get; set; }
        public bool Certification { get; set; }
        public bool AllowBackdating { get; set; }
        public int TestSessionBookingAvailabilityWeeks { get; set; }
        public int TestSessionBookingClosedWeeks { get; set; }
        public int TestSessionBookingRejectHours { get; set; }
        public bool AllowAvailabilityNotice { get; set; }
        public List<TestSpecification> TestSpecifications { get; set; }
        public int? DefaultExpiry { get; set; }
    }
}
