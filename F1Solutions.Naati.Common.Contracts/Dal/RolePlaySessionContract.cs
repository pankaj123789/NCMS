using System;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class RolePlaySessionContract
    {
        public int Id { get; set; }
        public int TestSessionId { get; set; }
        public string TestSessionName { get; set; }
        public string CredentialType { get; set; }
        public string RolePlay { get; set; }
        public DateTime TestSessionDate { get; set; }
        public string TestLocationName { get; set; }
        public string LanguageName { get; set; }
        public DateTime? RehearsalDate { get; set; }
        public string RehearsalNotes { get; set; }
        public bool Attended { get; set; }
        public bool Rehearsed { get; set; }
        public bool Rejected { get; set; }
    }
}