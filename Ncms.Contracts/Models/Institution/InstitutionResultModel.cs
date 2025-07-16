using System;

namespace Ncms.Contracts.Models.Institution
{
    public class InstitutionResultModel
    {
        public int Id { get; set; }
        public int? NaatiNumber { get; set; }
        public string Name { get; set; }
        public string PrimaryContactNo { get; set; }
        public string PrimaryEmail { get; set; }
        public int? NoOfContacts { get; set; }

    }
    public class EndorsedQualificationSearchResultModel
    {
        public int EndorsedQualificationId { get; set; }
        public int InstitutionId { get; set; }
        public int InstitutionNaatiNumber { get; set; }
        public string InstitutionName { get; set; }
        public string Location { get; set; }
        public string Qualification { get; set; }
        public int CredentialTypeId { get; set; }
        public string CredentialTypeInternalName { get; set; }
        public string CredentialTypeExternalName { get; set; }
        public DateTime EndorsementPeriodFrom { get; set; }
        public DateTime EndorsementPeriodTo { get; set; }
        public bool Active { get; set; }
    }

    public class EndorsedQualificationModel
    {
        public int EndorsedQualificationId { get; set; }
        public int InstitutionId { get; set; }
        public int CredentialTypeId { get; set; }
        public string Qualification { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }
        public DateTime EndorsementPeriodFrom { get; set; }
        public DateTime EndorsementPeriodTo { get; set; }
        public bool Active { get; set; }
    }

}