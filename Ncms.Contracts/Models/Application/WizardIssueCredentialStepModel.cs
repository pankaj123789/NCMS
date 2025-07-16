using System;
using System.Collections.Generic;
using Ncms.Contracts.Models.Person;

namespace Ncms.Contracts.Models.Application
{
    public class WizardIssueCredentialStepModel
    {

        public string PractitionerNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? SelectedCertificationPeriodId { get; set; }
        public bool ShowInOnlineDirectory { get; set; }
        public bool AllowEditCertificationPeriodStartDate { get; set; }
        public bool DisallowEditCredentialStartDate { get; set; }
        public IList<CertificationPeriodModel> CertificationPeriods { get; set; }
        public int CertificationPeriodDefaultDuration { get; set; }
    }

    public class IssueCredentialDataModel : WizardIssueCredentialStepModel
    {

        public DateTime? CertificationPeriodStart { get; set; }
        public DateTime? CertificationPeriodEnd { get; set; }
    }
}