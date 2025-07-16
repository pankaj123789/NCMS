using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class TestSessionRolePlayerDto 
    {
        public int TestSessionRolePlayerId { get; set; }
        public  int TestSessionId { get; set; }
        public  int RolePlayerId { get; set; }
        public bool Attended { get; set; }
        public bool Rehearsed { get; set; }
        public bool Rejected { get; set; }
        public int RolePlayerStatusId { get; set; }
        public DateTime StatusChangeDate { get; set; }
        public int StatusChangeUserId { get; set; }
        public DateTime? RehearsalDate { get; set; }
        public DateTime TestSessionDate { get; set; }
        public string TestSessionName { get; set; }
        public string TestSessionLocation { get; set; }
        public string VenueName { get; set; }
        public string RolePlayerStatus { get; set; }
        public string RolePlayerStatusDisplayName { get; set; }
        public string CredentialTypeExternalName { get; set; }
        public string RehearsalNotes { get; set; }
        public string PublicNote { get; set; }
        public string VenueAddress { get; set; }
        public string VenueCoordinates { get; set; }
        public int NaatiNumber { get; set; }
        public IList<TestSessionRolePlayerDetailDto> Details { get; set; }
        public ObjectStatusTypeName ObjectStatus { get; set; }
    }
}