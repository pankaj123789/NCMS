using System;
using System.Collections.Generic;
using Ncms.Contracts.Models.CredentialRequest;

namespace Ncms.Contracts.Models.Test
{
    public class TestSessionModel
    {
        public int TestSessionId { get; set; }
        public string Name { get; set; }
        public int VenueId { get; set; }
        public string VenueName { get; set; }
        public int VenueCapacity { get; set; }
        public string VenueAddress { get; set; }
        public DateTime TestDate { get; set; }
      
        public string TestTime { get; set; }
        public int? ArrivalTime { get; set; }
        public int? Duration { get; set; }
        public int CredentialTypeId { get; set; }
        public int CredentialApplicationTypeId { get; set; }
        public string PublicNotes { get; set; }

        public string RehearsalNotes { get; set; }
        public DateTime? RehearsalDate { get; set; }
        public string RehearsalTime { get; set; }
        public bool Completed { get; set; }
        public bool AllowSelfAssign { get; set; }

        public int DefaultTestSpecificationId { get; set; }
        public int MarkingSchemaTypeId { get; set; }
        public bool HasRolePlayers { get; set; }
        public bool NewCandidatesOnly { get; set; }

        public IEnumerable<TestSessionApplicantModel> Applicants { get; set; }
    }

    public class TestSessionSkillModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TestSessionId { get; set; }
        public int SkillId { get; set; }
        public int? MaximumCapacity { get; set; }
        public bool Selected { get; set; }
    }

    public class TestSessionSkillValidationModel
    {
        public IList<object> Errors { get; set; }
        public IList<object> Warnings { get; set; }
    }
}
