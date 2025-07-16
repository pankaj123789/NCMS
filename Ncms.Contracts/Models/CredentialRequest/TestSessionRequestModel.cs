using System;
using Ncms.Contracts.Models.Test;

namespace Ncms.Contracts.Models.CredentialRequest
{
    public class TestSessionRequestModel
    {
        public int? Id { get; set; }
        public string SessionName { get; set; }
        public int? VenueId { get; set; }
        public string Notes { get; set; }
        public DateTime? TestDate { get; set; }
        public string TestTime { get; set; }
        public int? PreparationTime { get; set; }
        public int? SessionDuration { get; set; }
        public int CredentialTypeId { get; set; }
        public int CredentialApplicationTypeId { get; set; }

        public bool AllowSelfAssign { get; set; }

        public  bool? Completed { get; set; }

        public TestSessionSkillModel[] Skills { get; set; }
		public int Capacity { get; set; }
		public bool OverrideVenueCapacity { get; set; }
        public bool NewCandidatesOnly { get; set; }
        public bool IsActive { get; set; }

        public DateTime? RehearsalDate { get; set; }
        public string RehearsalTime { get; set; }
        public string RehearsalNotes { get; set; }

        public int DefaultTestSpecificationId { get; set; }
    }
}
