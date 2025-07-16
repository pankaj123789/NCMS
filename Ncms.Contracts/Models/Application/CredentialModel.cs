using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Contracts.Models.Person;

namespace Ncms.Contracts.Models.Application
{
    public class CredentialModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public bool ShowInOnlineDirectory { get; set; }
        public bool Certification { get; set; }
        public string CredentialTypeInternalName { get; set; }
        public string SkillDisplayName { get; set; }
        public CertificationPeriodModel CertificationPeriod { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public RecertificationStatus RecertificationStatus { get; set; }
        public int SkillId { get; set; }
        public int CredentialTypeId { get; set; }
        public int CategoryId { get; set; }

        public IEnumerable<int> StoredFileIds { get; set; }
    }
}