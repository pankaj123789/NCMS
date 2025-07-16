using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CredentialDto
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public bool ShowInOnlineDirectory { get; set; }
        public bool Certification { get; set; }
        public string CredentialTypeInternalName { get; set; }
        public string CredentialTypeExternalName { get; set; }
        public string CredentialCategoryName { get; set; }
        public string SkillDisplayName { get; set; }
        public int SkillId { get; set; }
        public int CredentialTypeId { get; set; }
        public int CategoryId { get; set; }
        public IEnumerable<int> StoredFileIds { get; set; }
        public CertificationPeriodDto CertificationPeriod { get; set; }
    }
}