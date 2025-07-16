using System;

namespace Ncms.Contracts.Models.Test
{
    public class TestSessionSearchResultModel
    {
        public string Skill { get; set; }
        public  DateTime? TestDate { get; set; }
        public string Venue { get; set; }
        public  int TestSessionId { get; set; }
//        public  int CredentialRequestId { get; set; }
        //public  int SkillId { get; set; }
        public  int CredentialTypeId { get; set; }
        public  string CredentialTypeInternalName { get; set; }
        public  int Allocated { get; set; }
        public  string TestLocationName { get; set; }
        public  string TestLocationStateName { get; set; }
        public  string SessionName { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; }
    }
}
