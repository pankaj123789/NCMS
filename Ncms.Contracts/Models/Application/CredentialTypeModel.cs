using System.Collections.Generic;

namespace Ncms.Contracts.Models.Application
{
    public class CredentialTypeModel
    {
        public int Id { get; set; }
        public string ExternalName { get; set; }
        public string InternalName { get; set; }
        
        public int DisplayOrder { get; set; }
        public bool Simultaneous { get; set; }
        public int SkillType { get; set; }
        public bool Certification { get; set; }
        public IEnumerable<CredentialApplicationTypeCredentialTypeModel> CredentialApplicationTypeCredentialTypes { get; set; }
        public int? DefaultExpiry { get; set; }
        public int? ActiveTestSpecificationId { get; set; }

        public bool AutomaticIssuing { get; set; }
        public double? MaxScoreDifference { get; set; }
        public int? MarkingSchemaTypeId { get; set; }
        public int TestSessionBookingAvailabilityWeeks { get; set; }
        public int TestSessionBookingClosedWeeks { get; set; }
        public int TestSessionBookingRejectHours { get; set; }
        public bool AllowAvailabilityNotice { get; set; }
    }
}